using Conector4.General;
using Conector4.General.Elastic_Search;
using Conector4.Presence.Models.Produtos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Conector4.Presence.Controllers.Produtos
{
    public class crProdutos
    {
        private readonly PresenceApiHelper _api;
        private readonly clsConfig _config;
        private ElasticLogger _logger = new ElasticLogger();

        public crProdutos(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
            _config = new clsConfig();
        }

        public async Task<(bool Success, string Content, string Error)> Get(int offset = 0)
        {
            if (_api == null)
            {
                return (false, null, "PresenceApiHelper não inicializado");
            }

            try
            {
                var body = new { data_at = "2020-01-01" };
                var result = await _api.PostAsync("PRODUTOS", body, offset);
                
                if (result == null)
                    return (false, null, "Resultado nulo da API");
                
                if (result.Success)
                    return (true, result.Content, null);
                
                return (false, null, result.Error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("crProdutos.Get error: " + ex.Message);
                return (false, null, ex.Message);
            }
        }

        public async Task Insert(string json)
        {
            try
            {
                var payload = System.Text.Json.JsonSerializer.Deserialize<moProdutos>(json);

                if (payload?.Data == null || payload.Data.Count == 0)
                    return;

                await using var conn = new NpgsqlConnection(_config.getConfigBD());
                await conn.OpenAsync();

                await _logger.LogAsync("INFO", "Iniciando importação de Produtos...");

                foreach (var item in payload.Data)
                {
                    string query = @"INSERT INTO produtos (codigo, descricao, narrativa, departamento, grupo, subgrupo, 
                                    linha, marca, colecao, unidademedida, fornecedor, composicao, ativo, observacao, 
                                    datacadastro, altura, largura, comprimento, peso)
                        VALUES (@codigo, @descricao, @narrativa, @departamento, @grupo, @subgrupo, 
                                @linha, @marca, @colecao, @unidademedida, @fornecedor, @composicao, @ativo, @observacao, 
                                @datacadastro, @altura, @largura, @comprimento, @peso)
                        ON CONFLICT (codigo)
                        DO UPDATE SET
                            descricao = EXCLUDED.descricao,
                            narrativa = EXCLUDED.narrativa,
                            departamento = EXCLUDED.departamento,
                            grupo = EXCLUDED.grupo,
                            subgrupo = EXCLUDED.subgrupo,
                            linha = EXCLUDED.linha,
                            marca = EXCLUDED.marca,
                            colecao = EXCLUDED.colecao,
                            unidademedida = EXCLUDED.unidademedida,
                            fornecedor = EXCLUDED.fornecedor,
                            composicao = EXCLUDED.composicao,
                            ativo = EXCLUDED.ativo,
                            observacao = EXCLUDED.observacao,
                            datacadastro = EXCLUDED.datacadastro,
                            altura = EXCLUDED.altura,
                            largura = EXCLUDED.largura,
                            comprimento = EXCLUDED.comprimento,
                            peso = EXCLUDED.peso;
                        ";

                    var command = new NpgsqlCommand(query, conn);
                    command.Parameters.AddWithValue("@codigo", item.Codigo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@descricao", item.Descricao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@narrativa", item.Narrativa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@departamento", item.Departamento ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@grupo", item.Grupo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@subgrupo", item.Subgrupo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@linha", item.Linha ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@marca", item.Marca.ToString() ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@colecao", item.Colecao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@unidademedida", item.UnidadeMedida ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@fornecedor", item.Fornecedor ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@composicao", item.Composicao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ativo", item.Ativo ? "true" : "false");
                    command.Parameters.AddWithValue("@observacao", item.Observacao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@datacadastro", item.DataCadastro != default ? item.DataCadastro : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@altura", item.Altura);
                    command.Parameters.AddWithValue("@largura", item.Largura);
                    command.Parameters.AddWithValue("@comprimento", item.Comprimento);
                    command.Parameters.AddWithValue("@peso", item.Peso);

                    await command.ExecuteNonQueryAsync();
                }

                await _logger.LogAsync("INFO", $"Importação de Produtos concluída. Total: {payload.Data.Count} registros.");
            }
            catch (Exception ex)
            {
                await _logger.LogAsync("ERRO", $"Erro durante a importação de Produtos. Classe crProdutos: {ex.Message}");
            }
        }
    }
}
