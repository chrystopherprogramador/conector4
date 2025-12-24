using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Conector4.Presence.Controllers.Tabela_de_Preco
{
    public class TabPrecoPayload
    {
        public List<TabPrecoItem> data { get; set; }
    }

    public class TabPrecoItem
    {
        public string codigo { get; set; }
        public int tabelaPreco { get; set; }
        public decimal precoCusto { get; set; }
        public decimal precoVarejo { get; set; }
    }

    public class crTabPrecoInput
    {
        private readonly string _connectionString;
        private General.Elastic_Search.ElasticLogger _logger = new General.Elastic_Search.ElasticLogger();
        public crTabPrecoInput(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task ImportarAsync(string json)
        {
            var payload = JsonSerializer.Deserialize<TabPrecoPayload>(json);

            if (payload?.data == null || payload.data.Count == 0)
                return;

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var tran = await conn.BeginTransactionAsync();

            // Cria um batch para mandar todos os inserts de uma vez
            var batch = new NpgsqlBatch(conn, tran);

            foreach (var item in payload.data)
            {
                var command = new NpgsqlBatchCommand(@"
            INSERT INTO tabpreco (codigo, tabpreco, precocusto, precovarejo)
            VALUES (@codigo, @tabpreco, @precocusto, @precovarejo)
            ON CONFLICT (codigo, tabpreco)
            DO UPDATE SET
                precocusto  = EXCLUDED.precocusto,
                precovarejo = EXCLUDED.precovarejo
        ");

                command.Parameters.AddWithValue("codigo", item.codigo);
                command.Parameters.AddWithValue("tabpreco", item.tabelaPreco);
                command.Parameters.AddWithValue("precocusto", item.precoCusto);
                command.Parameters.AddWithValue("precovarejo", item.precoVarejo);
                await _logger.LogAsync("Info", $"Adicionando comando para código {item.codigo}, tabela {item.tabelaPreco}");
                batch.BatchCommands.Add(command);
            }
            await _logger.LogAsync("Info", $"Executando batch com {batch.BatchCommands.Count} comandos.");
            await batch.ExecuteNonQueryAsync();
            await tran.CommitAsync();
        }

        public async Task ImportarAsyncInd(string json)
        {
            var payload = JsonSerializer.Deserialize<TabPrecoPayload>(json);

            if (payload?.data == null || payload.data.Count == 0)
                return;

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var tran = await conn.BeginTransactionAsync();

            // Cria um batch para mandar todos os inserts de uma vez
            var batch = new NpgsqlBatch(conn, tran);

            foreach (var item in payload.data)
            {
                var command = new NpgsqlBatchCommand(@"
            INSERT INTO tabpreco (codigo, tabpreco, precocusto, precovarejo)
            VALUES (@codigo, @tabpreco, @precocusto, @precovarejo)
            ON CONFLICT (codigo, tabpreco)
            DO UPDATE SET
                precocusto  = EXCLUDED.precocusto,
                precovarejo = EXCLUDED.precovarejo
        ");

                command.Parameters.AddWithValue("codigo", item.codigo);
                command.Parameters.AddWithValue("tabpreco", item.tabelaPreco);
                command.Parameters.AddWithValue("precocusto", item.precoCusto);
                command.Parameters.AddWithValue("precovarejo", item.precoVarejo);
                await _logger.LogAsync("Info", $"Adicionando comando para código {item.codigo}, tabela {item.tabelaPreco}");
                batch.BatchCommands.Add(command);
            }
            await _logger.LogAsync("Info", $"Executando batch com {batch.BatchCommands.Count} comandos.");
            await batch.ExecuteNonQueryAsync();
            await tran.CommitAsync();
        }
    }
}
