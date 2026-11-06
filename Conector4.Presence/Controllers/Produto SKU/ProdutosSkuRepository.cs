using Conector4.General.Elastic_Search;
using Conector4.Presence.Models.Produto_SKU;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Threading.Tasks;

public class ProdutosSkuRepository
{
    private readonly string _connectionString;
    private readonly ElasticLogger _logger = new ElasticLogger();
    public ProdutosSkuRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ImportarProdutosSkuAsync(string json)
    {
        var response = JsonConvert.DeserializeObject<ProdutosSkuResponse>(json);

        if (response == null || response.Data.Count == 0)
            return;

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        _logger.LogAsync("PRODUCT", $"Iniciando importação de {response.Data.Count} produtos SKU.");
        _logger.LogAsync("PRODUCT", $"Ultimo Código + 1 {response.NextOffSet}");

        const string sql = @"
            INSERT INTO produtos_sku (
                status, codigo, sku, codigotamanho, tamanho, codigocor, cor,
                descricao, narrativa, unidademedida, composicao, observacao,
                departamento, grupo, subgrupo, linha, marca, colecao,
                fornecedor, altura, largura, comprimento, peso
            )
            VALUES (
                @status, @codigo, @sku, @codigotamanho, @tamanho, @codigocor, @cor,
                @descricao, @narrativa, @unidademedida, @composicao, @observacao,
                @departamento, @grupo, @subgrupo, @linha, @marca, @colecao,
                @fornecedor, @altura, @largura, @comprimento, @peso
            )
            ON CONFLICT (sku) DO UPDATE SET
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
                peso = EXCLUDED.peso;
        ";

        foreach (var item in response.Data)
        {
            await using var cmd = new NpgsqlCommand(sql, conn);

            if(item.Marca == "Texas Cent")
            {
                item.Marca = "TEXAS CENTER";
            }

            cmd.Parameters.AddWithValue("status", (object?)item.Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("codigo", (object?)item.Codigo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("sku", item.Sku);
            cmd.Parameters.AddWithValue("codigotamanho", (object?)item.CodigoTamanho ?? DBNull.Value);
            cmd.Parameters.AddWithValue("tamanho", (object?)item.Tamanho ?? DBNull.Value);
            cmd.Parameters.AddWithValue("codigocor", (object?)item.CodigoCor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("cor", (object?)item.Cor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("descricao", (object?)item.Descricao ?? DBNull.Value);
            cmd.Parameters.AddWithValue("narrativa", (object?)item.Narrativa ?? DBNull.Value);
            cmd.Parameters.AddWithValue("unidademedida", (object?)item.UnidadeMedida ?? DBNull.Value);
            cmd.Parameters.AddWithValue("composicao", (object?)item.Composicao ?? DBNull.Value);
            cmd.Parameters.AddWithValue("observacao", (object?)item.Observacao ?? DBNull.Value);
            cmd.Parameters.AddWithValue("departamento", (object?)item.Departamento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("grupo", (object?)item.Grupo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("subgrupo", (object?)item.Subgrupo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("linha", (object?)item.Linha ?? DBNull.Value);
            cmd.Parameters.AddWithValue("marca", (object?)item.Marca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("colecao", (object?)item.Colecao ?? DBNull.Value);
            cmd.Parameters.AddWithValue("fornecedor", (object?)item.Fornecedor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("altura", (object?)item.Altura ?? DBNull.Value);
            cmd.Parameters.AddWithValue("largura", (object?)item.Largura ?? DBNull.Value);
            cmd.Parameters.AddWithValue("comprimento", (object?)item.Comprimento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("peso", (object?)item.Peso ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
            await _logger.LogAsync("PRODUCT", $"Produto: {(object?)item.Codigo ?? DBNull.Value} importado com sucesso. SKU..: {item.Sku}");
        }
    }
}
