using Conector4.General;
using Conector4.General.Elastic_Search;
using Conector4.General.Rabbit;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conector4.Presence.Controllers.Produtos
{
    public class crProdutosSku
    {
        private readonly PresenceApiHelper _api;
        private readonly clsConfig _config;
        private ElasticLogger _logger = new ElasticLogger();
        private RabbitProducer _producer = new RabbitProducer();

        public crProdutosSku(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
            _config = new clsConfig();
        }

        /// <summary>
        /// Busca produto SKU por código na API
        /// </summary>
        public async Task<(bool Success, string Content, string Error)> Get(string codigo, int offset = 0)
        {
            if (_api == null)
            {
                return (false, null, "PresenceApiHelper não inicializado");
            }

            try
            {
                var body = new { codigo = codigo };
                var result = await _api.PostAsync("PRODUTOS_SKU", body, offset);

                if (result == null)
                    return (false, null, "Resultado nulo da API");

                if (result.Success)
                    return (true, result.Content, null);

                return (false, null, result.Error);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync("Error", $"crProdutosSku.Get error: {ex.Message}");
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Busca múltiplos SKUs e publica no RabbitMQ
        /// </summary>
        public async Task<(int Sucesso, int Falhas, List<string> CodigosComErro)> BuscarEPublicarCodigos(
            List<string> codigos,
            string nomeFila = "Presence.ProdutoSKU",
            IProgress<(int Atual, int Total, string Codigo)> progresso = null)
        {
            int sucesso = 0;
            int falhas = 0;
            List<string> codigosComErro = new List<string>();

            await _logger.LogAsync("Info", $"Iniciando busca de {codigos.Count} SKUs para publicação no RabbitMQ");

            for (int i = 0; i < codigos.Count; i++)
            {
                string codigo = codigos[i];

                try
                {
                    progresso?.Report((i + 1, codigos.Count, codigo));

                    var result = await Get(codigo, 0);

                    if (result.Success && !string.IsNullOrEmpty(result.Content))
                    {
                        await _producer.PublishAsync(nomeFila, result.Content);
                        sucesso++;
                        await _logger.LogAsync("Info", $"SKU {codigo} publicado com sucesso na fila {nomeFila}");
                    }
                    else
                    {
                        falhas++;
                        codigosComErro.Add(codigo);
                        await _logger.LogAsync("Warning", $"SKU {codigo} não encontrado ou erro: {result.Error}");
                    }

                    await Task.Delay(50);
                }
                catch (Exception ex)
                {
                    falhas++;
                    codigosComErro.Add(codigo);
                    await _logger.LogAsync("Error", $"Erro ao processar SKU {codigo}: {ex.Message}");
                }
            }

            await _logger.LogAsync("Info", $"Busca de SKUs concluída. Sucesso: {sucesso}, Falhas: {falhas}");

            return (sucesso, falhas, codigosComErro);
        }

        /// <summary>
        /// Insere ou atualiza produtos_sku no banco de dados
        /// </summary>
        public async Task<int> Insert(dynamic skuData)
        {
            int registrosInseridos = 0;

            try
            {
                await using var conn = new NpgsqlConnection(_config.getConfigBD());
                await conn.OpenAsync();

                await _logger.LogAsync("INFO", "Iniciando importação de Produtos SKU...");

                // Tratar diferentes formatos de JSON
                System.Text.Json.JsonElement jsonElement;
                
                if (skuData is string jsonString)
                {
                    jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonString);
                }
                else if (skuData is System.Text.Json.JsonElement je)
                {
                    jsonElement = je;
                }
                else
                {
                    await _logger.LogAsync("WARNING", "Formato de dados não reconhecido em Insert SKU");
                    return 0;
                }

                // Verificar se tem propriedade "data"
                System.Text.Json.JsonElement itemsElement;
                
                if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Object && 
                    jsonElement.TryGetProperty("data", out var dataProperty))
                {
                    // Formato: { "data": [...] }
                    itemsElement = dataProperty;
                    await _logger.LogAsync("INFO", "JSON com propriedade 'data' detectado");
                }
                else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    // Formato: [...]
                    itemsElement = jsonElement;
                    await _logger.LogAsync("INFO", "JSON como array direto detectado");
                }
                else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    // Formato: { "sku": "...", ... } - objeto único
                    await _logger.LogAsync("INFO", "JSON como objeto único detectado");
                    
                    // Processar objeto único
                    await ProcessarItemSku(jsonElement, conn);
                    registrosInseridos++;
                    
                    await _logger.LogAsync("INFO", $"Importação de Produtos SKU concluída. Total: {registrosInseridos} registro.");
                    return registrosInseridos;
                }
                else
                {
                    await _logger.LogAsync("WARNING", "Formato de JSON não suportado");
                    return 0;
                }

                // Processar array de itens
                foreach (var item in itemsElement.EnumerateArray())
                {
                    await ProcessarItemSku(item, conn);
                    registrosInseridos++;
                }

                await _logger.LogAsync("INFO", $"Importação de Produtos SKU concluída. Total: {registrosInseridos} registros.");
            }
            catch (Exception ex)
            {
                await _logger.LogAsync("ERRO", $"Erro durante a importação de Produtos SKU: {ex.Message}");
                await _logger.LogAsync("ERRO", $"Stack: {ex.StackTrace}");
            }

            return registrosInseridos;
        }

        /// <summary>
        /// Processa um item individual de SKU
        /// </summary>
        private async Task ProcessarItemSku(System.Text.Json.JsonElement item, NpgsqlConnection conn)
        {
            string query = @"
                INSERT INTO produtos_sku (
                    status, codigo, sku, codigotamanho, tamanho, codigocor, cor,
                    descricao, narrativa, unidademedida, composicao, observacao,
                    departamento, grupo, subgrupo, linha, marca, colecao, fornecedor,
                    altura, largura, comprimento, peso
                )
                VALUES (
                    @status, @codigo, @sku, @codigotamanho, @tamanho, @codigocor, @cor,
                    @descricao, @narrativa, @unidademedida, @composicao, @observacao,
                    @departamento, @grupo, @subgrupo, @linha, @marca, @colecao, @fornecedor,
                    @altura, @largura, @comprimento, @peso
                )
                ON CONFLICT (sku)
                DO UPDATE SET
                    status = EXCLUDED.status,
                    codigo = EXCLUDED.codigo,
                    codigotamanho = EXCLUDED.codigotamanho,
                    tamanho = EXCLUDED.tamanho,
                    codigocor = EXCLUDED.codigocor,
                    cor = EXCLUDED.cor,
                    descricao = EXCLUDED.descricao,
                    narrativa = EXCLUDED.narrativa,
                    unidademedida = EXCLUDED.unidademedida,
                    composicao = EXCLUDED.composicao,
                    observacao = EXCLUDED.observacao,
                    departamento = EXCLUDED.departamento,
                    grupo = EXCLUDED.grupo,
                    subgrupo = EXCLUDED.subgrupo,
                    linha = EXCLUDED.linha,
                    marca = EXCLUDED.marca,
                    colecao = EXCLUDED.colecao,
                    fornecedor = EXCLUDED.fornecedor,
                    altura = EXCLUDED.altura,
                    largura = EXCLUDED.largura,
                    comprimento = EXCLUDED.comprimento,
                    peso = EXCLUDED.peso";

            await using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@status", GetJsonPropertyAsString(item, "status"));
            cmd.Parameters.AddWithValue("@codigo", GetJsonPropertyAsString(item, "codigo"));
            cmd.Parameters.AddWithValue("@sku", GetJsonPropertyAsString(item, "sku"));
            cmd.Parameters.AddWithValue("@codigotamanho", GetJsonPropertyAsString(item, "codigotamanho"));
            cmd.Parameters.AddWithValue("@tamanho", GetJsonPropertyAsString(item, "tamanho"));
            cmd.Parameters.AddWithValue("@codigocor", GetJsonPropertyAsString(item, "codigocor"));
            cmd.Parameters.AddWithValue("@cor", GetJsonPropertyAsString(item, "cor"));
            cmd.Parameters.AddWithValue("@descricao", GetJsonPropertyAsString(item, "descricao"));
            cmd.Parameters.AddWithValue("@narrativa", GetJsonPropertyAsString(item, "narrativa"));
            cmd.Parameters.AddWithValue("@unidademedida", GetJsonPropertyAsString(item, "unidademedida"));
            cmd.Parameters.AddWithValue("@composicao", GetJsonPropertyAsString(item, "composicao"));
            cmd.Parameters.AddWithValue("@observacao", GetJsonPropertyAsString(item, "observacao"));
            cmd.Parameters.AddWithValue("@departamento", GetJsonPropertyAsString(item, "departamento"));
            cmd.Parameters.AddWithValue("@grupo", GetJsonPropertyAsString(item, "grupo"));
            cmd.Parameters.AddWithValue("@subgrupo", GetJsonPropertyAsString(item, "subgrupo"));
            cmd.Parameters.AddWithValue("@linha", GetJsonPropertyAsString(item, "linha"));
            cmd.Parameters.AddWithValue("@marca", GetJsonPropertyAsString(item, "marca"));
            cmd.Parameters.AddWithValue("@colecao", GetJsonPropertyAsString(item, "colecao"));
            cmd.Parameters.AddWithValue("@fornecedor", GetJsonPropertyAsInt(item, "fornecedor"));
            cmd.Parameters.AddWithValue("@altura", GetJsonPropertyAsDecimal(item, "altura"));
            cmd.Parameters.AddWithValue("@largura", GetJsonPropertyAsDecimal(item, "largura"));
            cmd.Parameters.AddWithValue("@comprimento", GetJsonPropertyAsDecimal(item, "comprimento"));
            cmd.Parameters.AddWithValue("@peso", GetJsonPropertyAsDecimal(item, "peso"));

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Helper para extrair propriedade como string
        /// </summary>
        private object GetJsonPropertyAsString(System.Text.Json.JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && 
                prop.ValueKind != System.Text.Json.JsonValueKind.Null)
            {
                return prop.GetString() ?? (object)DBNull.Value;
            }
            return DBNull.Value;
        }

        /// <summary>
        /// Helper para extrair propriedade como int
        /// </summary>
        private object GetJsonPropertyAsInt(System.Text.Json.JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && 
                prop.ValueKind == System.Text.Json.JsonValueKind.Number)
            {
                return prop.GetInt32();
            }
            return DBNull.Value;
        }

        /// <summary>
        /// Helper para extrair propriedade como decimal
        /// </summary>
        private object GetJsonPropertyAsDecimal(System.Text.Json.JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && 
                prop.ValueKind == System.Text.Json.JsonValueKind.Number)
            {
                return prop.GetDecimal();
            }
            return DBNull.Value;
        }
    }
}
