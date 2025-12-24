using Conector4.General.Elastic_Search;
using Conector4.Presence.Models.Produto_SKU;
using Conector4.Presence.Models.PRODUTOS_TAMANHO;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Threading.Tasks;

public class crProdutos_Recebe
{
    private readonly string _connectionString;
    private readonly ElasticLogger _logger = new ElasticLogger();
    public crProdutos_Recebe(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task Importar(string json)
    {
        var response = JsonConvert.DeserializeObject<moProdutosTamanhos>(json);

        if (response == null || response.data.Count == 0)
            return;

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();



        const string sql = @"
            INSERT INTO produtos_tamanho (
                codigo,posicao,tamanho
            )
            VALUES (
                @codigo,@posicao,@tamanho
            )
            ON CONFLICT (codigo) DO UPDATE SET
                posicao = EXCLUDED.posicao,
                tamanho = EXCLUDED.tamanho
                
        ";

        foreach (var item in response.data)
        {
            await using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("codigo", (object?)item.codigo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("posicao", item.posicao);
            cmd.Parameters.AddWithValue("tamanho", (object?)item.tamanho ?? DBNull.Value);
            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                await _logger.LogAsync("Error", "Erro ao importar Produtos_Tamanho: " + ex.Message);
            }
            
        }
    }
}
