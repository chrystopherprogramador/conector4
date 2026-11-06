using Conector4.General;
using Conector4.General.Elastic_Search;
using Conector4.Presence.Controllers;
using Conector4.Presence.Models.VendasHeader;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Conector4.Presence.Controllers.NovaPasta
{
    public class crVendaHeader
    {
        private readonly PresenceApiHelper _api;
        private readonly ElasticLogger _logger = new ElasticLogger();
        private clsConfig _config = new clsConfig();
        public crVendaHeader(PresenceApiHelper api = null)
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
                var body = new { data_at = "2021-01-01" };
                var result = await _api.PostAsync("VENDAS_HEADER", body, offset);
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
            var response = JsonConvert.DeserializeObject<VendasHeader>(json);

            if (response == null || response.data.Count == 0)
                return;

            await using var conn = new NpgsqlConnection(_config.getConfigBD());
            await conn.OpenAsync();

            _logger.LogAsync("VENDAS_HEADER", $"Iniciando importação de {response.data.Count} offSet = {response.nextOffSet}");

            const string sql = @"INSERT INTO venda_header (
                    situacaoVenda, clienteCodigo, clienteNome, clienteFantasia, clienteEmail,
                    cpfCnpj, clientePessoa, clienteSexo, clienteNascimento, clienteTelefone,
                    clienteEndereco, clienteNumero, clienteComplemento, clienteBairro,
                    clienteCidade, clienteEstado, clienteCep, idLoja, dataCompra,
                    codigoTransportadora, fantasiaTransportadora, controle, terminal,
                    idCompra, nomeLoja, nomeFantasiaLoja, codigoVendedor, nomeVendedor,
                    subtotal, totalDesconto, totalFrete, totalSeguro, totalOutrasDespesas,
                    totalIcmsValorSt, totalFcpValorSt, totalIpi, total, fidelidadePontos,
                    fidelidadeValor, dataAt, integracaoNoPedido, tipoLoja, operacaoProduto,
                    tipoNota, modelo, modeloDescricao, quantidadeTotalProduto, numero
                )
                VALUES (
                    @situacaoVenda, @clienteCodigo, @clienteNome, @clienteFantasia, @clienteEmail,
                    @cpfCnpj, @clientePessoa, @clienteSexo, @clienteNascimento, @clienteTelefone,
                    @clienteEndereco, @clienteNumero, @clienteComplemento, @clienteBairro,
                    @clienteCidade, @clienteEstado, @clienteCep, @idLoja, @dataCompra,
                    @codigoTransportadora, @fantasiaTransportadora, @controle, @terminal,
                    @idCompra, @nomeLoja, @nomeFantasiaLoja, @codigoVendedor, @nomeVendedor,
                    @subtotal, @totalDesconto, @totalFrete, @totalSeguro, @totalOutrasDespesas,
                    @totalIcmsValorSt, @totalFcpValorSt, @totalIpi, @total, @fidelidadePontos,
                    @fidelidadeValor, @dataAt, @integracaoNoPedido, @tipoLoja, @operacaoProduto,
                    @tipoNota, @modelo, @modeloDescricao, @quantidadeTotalProduto, @numero
                )
                ON CONFLICT (idCompra) DO UPDATE SET
                    situacaoVenda = EXCLUDED.situacaoVenda,
                    clienteCodigo = EXCLUDED.clienteCodigo,
                    clienteNome = EXCLUDED.clienteNome,
                    clienteFantasia = EXCLUDED.clienteFantasia,
                    clienteEmail = EXCLUDED.clienteEmail,
                    cpfCnpj = EXCLUDED.cpfCnpj,
                    clientePessoa = EXCLUDED.clientePessoa,
                    clienteSexo = EXCLUDED.clienteSexo,
                    clienteNascimento = EXCLUDED.clienteNascimento,
                    clienteTelefone = EXCLUDED.clienteTelefone,
                    clienteEndereco = EXCLUDED.clienteEndereco,
                    clienteNumero = EXCLUDED.clienteNumero,
                    clienteComplemento = EXCLUDED.clienteComplemento,
                    clienteBairro = EXCLUDED.clienteBairro,
                    clienteCidade = EXCLUDED.clienteCidade,
                    clienteEstado = EXCLUDED.clienteEstado,
                    clienteCep = EXCLUDED.clienteCep,
                    idLoja = EXCLUDED.idLoja,
                    dataCompra = EXCLUDED.dataCompra,
                    codigoTransportadora = EXCLUDED.codigoTransportadora,
                    fantasiaTransportadora = EXCLUDED.fantasiaTransportadora,
                    controle = EXCLUDED.controle,
                    terminal = EXCLUDED.terminal,
                    nomeLoja = EXCLUDED.nomeLoja,
                    nomeFantasiaLoja = EXCLUDED.nomeFantasiaLoja,
                    codigoVendedor = EXCLUDED.codigoVendedor,
                    nomeVendedor = EXCLUDED.nomeVendedor,
                    subtotal = EXCLUDED.subtotal,
                    totalDesconto = EXCLUDED.totalDesconto,
                    totalFrete = EXCLUDED.totalFrete,
                    totalSeguro = EXCLUDED.totalSeguro,
                    totalOutrasDespesas = EXCLUDED.totalOutrasDespesas,
                    totalIcmsValorSt = EXCLUDED.totalIcmsValorSt,
                    totalFcpValorSt = EXCLUDED.totalFcpValorSt,
                    totalIpi = EXCLUDED.totalIpi,
                    total = EXCLUDED.total,
                    fidelidadePontos = EXCLUDED.fidelidadePontos,
                    fidelidadeValor = EXCLUDED.fidelidadeValor,
                    dataAt = EXCLUDED.dataAt,
                    integracaoNoPedido = EXCLUDED.integracaoNoPedido,
                    tipoLoja = EXCLUDED.tipoLoja,
                    operacaoProduto = EXCLUDED.operacaoProduto,
                    tipoNota = EXCLUDED.tipoNota,
                    modelo = EXCLUDED.modelo,
                    modeloDescricao = EXCLUDED.modeloDescricao,
                    quantidadeTotalProduto = EXCLUDED.quantidadeTotalProduto,
                    numero = EXCLUDED.numero;
        ";

            foreach (var item in response.data)
            {
                await using var cmd = new NpgsqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@situacaoVenda", item.situacaoVenda ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteCodigo", item.clienteCodigo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteNome", item.clienteNome ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteFantasia", item.clienteFantasia ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteEmail", item.clienteEmail ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@cpfCnpj", item.cpfCnpj ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clientePessoa", item.clientePessoa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteSexo", item.clienteSexo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteNascimento", item.clienteNascimento ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteTelefone", item.clienteTelefone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteEndereco", item.clienteEndereco ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteNumero", item.clienteNumero ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteComplemento", item.clienteComplemento ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteBairro", item.clienteBairro ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteCidade", item.clienteCidade ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteEstado", item.clienteEstado ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@clienteCep", item.clienteCep?.Trim() ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@idLoja", item.idLoja ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@dataCompra", item.dataCompra);

                cmd.Parameters.AddWithValue("@codigoTransportadora", (object?)item.codigoTransportadora ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@fantasiaTransportadora", (object?)item.fantasiaTransportadora ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@controle", item.controle);
                cmd.Parameters.AddWithValue("@terminal", item.terminal);
                cmd.Parameters.AddWithValue("@idCompra", item.idCompra ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@nomeLoja", item.nomeLoja ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@nomeFantasiaLoja", item.nomeFantasiaLoja ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@codigoVendedor", item.codigoVendedor);
                cmd.Parameters.AddWithValue("@nomeVendedor", item.nomeVendedor ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@subtotal", item.subtotal);
                cmd.Parameters.AddWithValue("@totalDesconto", item.totalDesconto);
                cmd.Parameters.AddWithValue("@totalFrete", item.totalFrete);
                cmd.Parameters.AddWithValue("@totalSeguro", item.totalSeguro);
                cmd.Parameters.AddWithValue("@totalOutrasDespesas", item.totalOutrasDespesas);
                cmd.Parameters.AddWithValue("@totalIcmsValorSt", item.totalIcmsValorSt);
                cmd.Parameters.AddWithValue("@totalFcpValorSt", item.totalFcpValorSt);
                cmd.Parameters.AddWithValue("@totalIpi", item.totalIpi);
                cmd.Parameters.AddWithValue("@total", item.total);

                cmd.Parameters.AddWithValue("@fidelidadePontos", item.fidelidadePontos);
                cmd.Parameters.AddWithValue("@fidelidadeValor", item.fidelidadeValor);

                cmd.Parameters.AddWithValue("@dataAt", item.dataAt);
                cmd.Parameters.AddWithValue("@integracaoNoPedido", item.integracaoNoPedido ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tipoLoja", item.tipoLoja ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@operacaoProduto", item.operacaoProduto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tipoNota", item.tipoNota ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@modelo", item.modelo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@modeloDescricao", item.modeloDescricao ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@quantidadeTotalProduto", item.quantidadeTotalProduto);
                cmd.Parameters.AddWithValue("@numero", item.numero);

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }catch (Exception ex)
                {
                    _logger.LogAsync("ERROR", ex.Message);
                }
            }
        }
    }
}
