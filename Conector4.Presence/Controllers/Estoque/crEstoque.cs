using Conector4.General;
using Conector4.General.Elastic_Search;
using Conector4.Presence.Models.Estoque;
using Conector4.Presence.Models.Fornecedores;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json.Nodes;

namespace Conector4.Presence.Controllers.Estoque
{
    public class crEstoque
    {
        private readonly PresenceApiHelper _api;
        private readonly clsConfig _config;
        private ElasticLogger _logger = new ElasticLogger();
        public crEstoque(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
            _config = new clsConfig();
        }

        public async Task Insert(string json)
        {

            try
            {
                await using var conn = new NpgsqlConnection(_config.getConfigBD());
                await conn.OpenAsync();

                await _logger.LogAsync("INFO","Iniciando importação de Estoque...");

                // Tratar diferentes formatos de JSON
                System.Text.Json.JsonElement jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);

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
                    // Formato: { "codigo": "...", ... } - objeto único
                    await _logger.LogAsync("INFO", "JSON como objeto único detectado");
                    await ProcessarItemEstoque(jsonElement, conn);
                    return;
                }
                else
                {
                    await _logger.LogAsync("WARNING", "Formato de JSON não suportado em Estoque");
                    return;
                }

                // Processar array de itens
                int count = 0;
                foreach (var item in itemsElement.EnumerateArray())
                {
                    await ProcessarItemEstoque(item, conn);
                    count++;
                }

                await _logger.LogAsync("INFO", $"Importação de Estoque concluída. Total: {count} registros.");
            }
            catch (Exception ex)
            {
                await _logger.LogAsync("ERRO", $"Erro durante a importação de Estoque: {ex.Message}");
                await _logger.LogAsync("ERRO", $"Stack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Processa um item individual de estoque
        /// </summary>
        private async Task ProcessarItemEstoque(System.Text.Json.JsonElement item, NpgsqlConnection conn)
        {
            string query = @"INSERT INTO estoque (codigo, loja, quantidade)
                VALUES (@codigo, @loja, @quantidade)
                ON CONFLICT (codigo, loja)
                DO UPDATE SET
                    quantidade = EXCLUDED.quantidade";

            var command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@codigo", GetJsonPropertyAsString(item, "codigo"));
            command.Parameters.AddWithValue("@loja", GetJsonPropertyAsString(item, "loja"));
            command.Parameters.AddWithValue("@quantidade", GetJsonPropertyAsDecimal(item, "quantidade"));

            await command.ExecuteNonQueryAsync();
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
        /// Helper para extrair propriedade como decimal
        /// </summary>
        private object GetJsonPropertyAsDecimal(System.Text.Json.JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == System.Text.Json.JsonValueKind.Number)
                {
                    return prop.GetDecimal();
                }
                else if (prop.ValueKind == System.Text.Json.JsonValueKind.String && 
                         decimal.TryParse(prop.GetString(), out decimal result))
                {
                    return result;
                }
            }
            return 0m; // Default para quantidade
        }
        public async Task<(bool Success, string Content, string Error)> Get(string store, int offset = 0)
        {
    
            if(_api == null)
            {
                return (false, null, "PresenceApiHelper não inicializado");
            }

            try
            {
                var body = new { data_at = "2000-01-01",loja =  store};
                var result = await _api.PostAsync("ESTOQUE", body, offset);
                if (result.Success)
                {
                    return (true, result.Content, result.Error);
                }
                else
                {
                    return (false, result.Content, result.Error);
                }

            }
            catch (Exception ex)
            {
                return(false, null, ex.Message);
            }

        }

    }
}
