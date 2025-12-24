using Conector4.Presence.Controllers;
using System;
using System.Threading.Tasks;
using Conector4.General.Elastic_Search;
using Npgsql;
using Conector4.General;

namespace Conector4.Presence.Controllers.NovaPasta
{
    public class crTabPreco
    {
        private readonly PresenceApiHelper _api;
        private readonly ElasticLogger _logger = new ElasticLogger();
        private clsConfig _config = new clsConfig();
        public crTabPreco(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
        }

        public async Task<List<string>> RetornaFaltantes()
        {
            var list = new List<string>();

            try
            {

                using (NpgsqlConnection _connect = new NpgsqlConnection(_config.getConfigBD()))
                {
                    await _connect.OpenAsync();

                    string sqlQuery =
                    "SELECT p.codigo AS codigo, p.descricao AS descricao, c.precovarejo AS varejo, c.precocusto AS custo " +
                    "FROM produtos_sku p " +
                    "LEFT JOIN tabpreco c ON c.codigo = p.codigo " +
                    "WHERE c.precocusto IS NULL " +   
                    "GROUP BY p.codigo, p.descricao, c.precovarejo, c.precocusto";


                    using (NpgsqlCommand _command = new NpgsqlCommand(sqlQuery,_connect))
                    {
                        using var reader = await _command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            var codigo = reader.GetString(reader.GetOrdinal("codigo"));
                            list.Add(codigo);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
               await _logger.LogAsync("Error","Erro ao obter produtos sem preço..:" +  ex.Message);
            }

            return list;
        }

        public async Task<(bool Sucess, string Content, string Erro)> getTabPrecoInd(string codigo,int tab)
        {
            var body = new { tabelaPreco = tab , codigo = codigo};

            await _logger.LogAsync("Info", $"crTabPreco.getTabPrecoInd start: Tab={tab}, offset={0},codigo={codigo}");
            var result = await _api.PostAsync("PRECOS", body, 0);
            if (result.Success)
            {
                await _logger.LogAsync("Info", $"crTabPreco.getTabPrecoInd success: Tab={tab}, offset={0}, length={result.Content?.Length}");
                return (true, result.Content, null);
            }
            await _logger.LogAsync("Error", $"crTabPreco.getTabPrecoInd error: {result.Error}");
            return (false, null, result.Error);
        }
        public async Task<(bool Success, string Content, string Error)> getTabPreco(int Tab, int offset)
        {
            var body = new {tabelaPreco = Tab };
            await _logger.LogAsync("Info", $"crTabPreco.getTabPreco start: Tab={Tab}, offset={offset}");
            var result = await _api.PostAsync("PRECOS", body, offset);
            if (result.Success)
            {
                await _logger.LogAsync("Info", $"crTabPreco.getTabPreco success: Tab={Tab}, offset={offset}, length={result.Content?.Length}");
                return (true, result.Content, null);
            }

            await _logger.LogAsync("Error", $"crTabPreco.getTabPreco error: {result.Error}");
            return (false, null, result.Error);
        }
    }
}
