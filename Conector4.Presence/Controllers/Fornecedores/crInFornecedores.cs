using Conector4.General.Elastic_Search;
using Conector4.Presence.Models.Fornecedores;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Conector4.Presence.Controllers.Fornecedores
{
    public class crInFornecedores
    {

        private readonly string _connectionString;
        private General.Elastic_Search.ElasticLogger _logger = new General.Elastic_Search.ElasticLogger();

        public crInFornecedores(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task ImportarAsync(string json)
        {
            // Implementar a lógica de importação aqui

            var payload = System.Text.Json.JsonSerializer.Deserialize<moFornecedores>(json);

            if (payload?.Data == null || payload.Data.Count == 0)
                return;

            await _logger.LogAsync("INFO", "Iniciando importação de fornecedores.:" + payload.Data.Count);

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var tran = await conn.BeginTransactionAsync();
            var batch = new NpgsqlBatch(conn, tran);

            foreach (var item in payload.Data)
            {
                var command = new NpgsqlBatchCommand(@"
                    INSERT INTO fornecedores 
                        (""codigo"", ""razaoSocial"", ""fantansia"", ""CNPJ"")
                    VALUES 
                        (@codigo, @razao_social, @nome_fantasia, @cnpj)
                    ON CONFLICT (""codigo"")
                    DO UPDATE SET
                        ""razaoSocial"" = EXCLUDED.""razaoSocial"",
                        ""fantansia""   = EXCLUDED.""fantansia"",
                        ""CNPJ""        = EXCLUDED.""CNPJ""
                ");

                command.Parameters.AddWithValue("codigo", item.Codigo);
                command.Parameters.AddWithValue("razao_social", item.RazaoSocial ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("nome_fantasia", item.Fantasia ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("cnpj", item.Cnpj ?? (object)DBNull.Value);
                batch.BatchCommands.Add(command);
                await _logger.LogAsync("INFO", $"Adicionando comando para código {item.Codigo} Fornecedores.");
            }
            try
            {
                await batch.ExecuteNonQueryAsync();
                await tran.CommitAsync();
                await _logger.LogAsync("INFO", "Importação de Fornecedores concluída com sucesso.");
            }
            catch (Exception ex)
            {
                await _logger.LogAsync("ERRO", "Erro durante a importação de Fornecedores: " + ex.Message);
                await tran.RollbackAsync();
            }

        }
    }
}
