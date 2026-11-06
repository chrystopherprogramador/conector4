using Conector4.General;
using Conector4.General.Elastic_Search;
using Conector4.Presence.Models.VendasHeader;
using Conector4.Presence.Models.VendasItens;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence.Controllers.VendasItens
{
    public class crVendasItens
    {

        private readonly PresenceApiHelper _api;
        private readonly ElasticLogger _logger = new ElasticLogger();
        private clsConfig _config = new clsConfig();
        public crVendasItens(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
        }

        public async Task<(bool Success, string Content, string Error)> get(int offset = 0)
        {
            if (_api == null)
            {
                return (false, null, "PresenceApiHelper não inicializado");
            }

            try
            {
                var body = new { data_at = "1900-01-01" };
                var result = await _api.PostAsync("VENDAS_ITENS", body, offset);
                if (result == null)
                    return (false, null, "Resultado nulo da API");

                if (result.Success)
                    await Insert(result.Content);
                return (true, result.Content, null);

                return (false, null, result.Error);
            }
            catch (Exception ex)
            {
                // Não lançar para não travar o sistema
                System.Diagnostics.Trace.WriteLine("crVendasHeader.get error: " + ex.Message);
                return (false, null, ex.Message);
            }
        }

        public async Task Insert(string json)
        {
            var response = JsonConvert.DeserializeObject<VendasItensResponse>(json);

            if (response == null || response.data.Count == 0)
                return;

            await using var conn = new NpgsqlConnection(_config.getConfigBD());
            await conn.OpenAsync();

            _logger.LogAsync("VENDAS_ITENS", $"Iniciando importação de {response.data.Count} offSet = {response.nextOffSet}");

            const string sql = @"INSERT INTO venda_itens (
                    situacaoVenda, idCompra, seriePedido, numeroPedido, controleItem,
                    idProduto, nomeProduto, quantidadeProduto, precoUnitario, desconto,
                    acrescimo, frete, seguro, outrasDespesas, icmsVlrSt, fcpValorSt,
                    ipiValor, precoCusto, precoLiquido, transacao, codigoCategoria,
                    categoriaProduto, codigoSubcategoria, subcategoriaProduto,
                    corProduto, tamanhoProduto, idLoja, lojaNome, lojaFantasia,
                    dataCompra, controle, terminal, vendedorControle, vendedorNome,
                    codigo, codigoLinha, linhaProduto, codigoColecao, colecaoProduto,
                    codigoMarca, marca, referenciaFornecedor, fidelidadePontos,
                    fidelidadeValor, dataAtualizacao, participaFidelidade, numero
                )
                VALUES (
                    @situacaoVenda, @idCompra, @seriePedido, @numeroPedido, @controleItem,
                    @idProduto, @nomeProduto, @quantidadeProduto, @precoUnitario, @desconto,
                    @acrescimo, @frete, @seguro, @outrasDespesas, @icmsVlrSt, @fcpValorSt,
                    @ipiValor, @precoCusto, @precoLiquido, @transacao, @codigoCategoria,
                    @categoriaProduto, @codigoSubcategoria, @subcategoriaProduto,
                    @corProduto, @tamanhoProduto, @idLoja, @lojaNome, @lojaFantasia,
                    @dataCompra, @controle, @terminal, @vendedorControle, @vendedorNome,
                    @codigo, @codigoLinha, @linhaProduto, @codigoColecao, @colecaoProduto,
                    @codigoMarca, @marca, @referenciaFornecedor, @fidelidadePontos,
                    @fidelidadeValor, @dataAtualizacao, @participaFidelidade, @numero
                )
                ON CONFLICT (idCompra, idproduto) DO UPDATE SET
                    situacaoVenda = EXCLUDED.situacaoVenda,
                    seriePedido = EXCLUDED.seriePedido,
                    numeroPedido = EXCLUDED.numeroPedido,
                    idProduto = EXCLUDED.idProduto,
                    nomeProduto = EXCLUDED.nomeProduto,
                    quantidadeProduto = EXCLUDED.quantidadeProduto,
                    precoUnitario = EXCLUDED.precoUnitario,
                    desconto = EXCLUDED.desconto,
                    acrescimo = EXCLUDED.acrescimo,
                    frete = EXCLUDED.frete,
                    seguro = EXCLUDED.seguro,
                    outrasDespesas = EXCLUDED.outrasDespesas,
                    icmsVlrSt = EXCLUDED.icmsVlrSt,
                    fcpValorSt = EXCLUDED.fcpValorSt,
                    ipiValor = EXCLUDED.ipiValor,
                    precoCusto = EXCLUDED.precoCusto,
                    precoLiquido = EXCLUDED.precoLiquido,
                    transacao = EXCLUDED.transacao,
                    codigoCategoria = EXCLUDED.codigoCategoria,
                    categoriaProduto = EXCLUDED.categoriaProduto,
                    codigoSubcategoria = EXCLUDED.codigoSubcategoria,
                    subcategoriaProduto = EXCLUDED.subcategoriaProduto,
                    corProduto = EXCLUDED.corProduto,
                    tamanhoProduto = EXCLUDED.tamanhoProduto,
                    idLoja = EXCLUDED.idLoja,
                    lojaNome = EXCLUDED.lojaNome,
                    lojaFantasia = EXCLUDED.lojaFantasia,
                    dataCompra = EXCLUDED.dataCompra,
                    controle = EXCLUDED.controle,
                    terminal = EXCLUDED.terminal,
                    vendedorControle = EXCLUDED.vendedorControle,
                    vendedorNome = EXCLUDED.vendedorNome,
                    codigo = EXCLUDED.codigo,
                    codigoLinha = EXCLUDED.codigoLinha,
                    linhaProduto = EXCLUDED.linhaProduto,
                    codigoColecao = EXCLUDED.codigoColecao,
                    colecaoProduto = EXCLUDED.colecaoProduto,
                    codigoMarca = EXCLUDED.codigoMarca,
                    marca = EXCLUDED.marca,
                    referenciaFornecedor = EXCLUDED.referenciaFornecedor,
                    fidelidadePontos = EXCLUDED.fidelidadePontos,
                    fidelidadeValor = EXCLUDED.fidelidadeValor,
                    dataAtualizacao = EXCLUDED.dataAtualizacao,
                    participaFidelidade = EXCLUDED.participaFidelidade,
                    numero = EXCLUDED.numero;
                ";

            foreach (var item in response.data)
            {
                await using var cmd = new NpgsqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@situacaoVenda", item.situacaoVenda ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@idCompra", item.idCompra ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@seriePedido", item.seriePedido);
                cmd.Parameters.AddWithValue("@numeroPedido", item.numeroPedido);
                cmd.Parameters.AddWithValue("@controleItem", item.controleItem);

                cmd.Parameters.AddWithValue("@idProduto", item.idProduto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@nomeProduto", item.nomeProduto ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@quantidadeProduto", item.quantidadeProduto);
                cmd.Parameters.AddWithValue("@precoUnitario", item.precoUnitario);
                cmd.Parameters.AddWithValue("@desconto", item.desconto);
                cmd.Parameters.AddWithValue("@acrescimo", item.acrescimo);
                cmd.Parameters.AddWithValue("@frete", item.frete);
                cmd.Parameters.AddWithValue("@seguro", item.seguro);
                cmd.Parameters.AddWithValue("@outrasDespesas", item.outrasDespesas);
                cmd.Parameters.AddWithValue("@icmsVlrSt", item.icmsVlrSt);
                cmd.Parameters.AddWithValue("@fcpValorSt", item.fcpValorSt);
                cmd.Parameters.AddWithValue("@ipiValor", item.ipiValor);
                cmd.Parameters.AddWithValue("@precoCusto", item.precoCusto);
                cmd.Parameters.AddWithValue("@precoLiquido", item.precoLiquido);

                cmd.Parameters.AddWithValue("@transacao", item.transacao ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@codigoCategoria", item.codigoCategoria ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@categoriaProduto", item.categoriaProduto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@codigoSubcategoria", item.codigoSubcategoria ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@subcategoriaProduto", item.subcategoriaProduto ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@corProduto", item.corProduto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tamanhoProduto", item.tamanhoProduto ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@idLoja", item.idLoja ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@lojaNome", item.lojaNome ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@lojaFantasia", item.lojaFantasia ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@dataCompra", item.dataCompra ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@controle", item.controle);
                cmd.Parameters.AddWithValue("@terminal", item.terminal);

                cmd.Parameters.AddWithValue("@vendedorControle", item.vendedorControle);
                cmd.Parameters.AddWithValue("@vendedorNome", item.vendedorNome ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@codigo", item.codigo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@codigoLinha", item.codigoLinha ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@linhaProduto", item.linhaProduto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@codigoColecao", item.codigoColecao ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@colecaoProduto", item.colecaoProduto ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@codigoMarca", item.codigoMarca ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@marca", item.marca ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@referenciaFornecedor", item.referenciaFornecedor ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@fidelidadePontos", item.fidelidadePontos);
                cmd.Parameters.AddWithValue("@fidelidadeValor", item.fidelidadeValor);

                cmd.Parameters.AddWithValue("@dataAtualizacao", item.dataAtualizacao);
                cmd.Parameters.AddWithValue("@participaFidelidade", item.participaFidelidade ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@numero", item.numero);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
