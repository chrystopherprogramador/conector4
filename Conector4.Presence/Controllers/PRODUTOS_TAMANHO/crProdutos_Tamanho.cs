using Conector4.General.Elastic_Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence.Controllers.PRODUTOS_TAMANHO
{
    public class crProdutos_Tamanho
    {

        private readonly PresenceApiHelper _api;
        private readonly ElasticLogger _logger = new ElasticLogger();

        public crProdutos_Tamanho(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
        }

        public async Task<(bool Success, string Content, string Error)> get(int offset)
        {
            var body = new { data_at = "2000-01-01" };
            await _logger.LogAsync("Info", $"crProdutos_Tamanho.get start: offset={offset}");
            var result = await _api.PostAsync("PRODUTOS_TAMANHO", body, offset);
            if (result.Success)
            {
                await _logger.LogAsync("Info", $"crProdutos_Tamanho.get success: offset={offset}, length={result.Content?.Length}");
                return (true, result.Content, null);
            }

            await _logger.LogAsync("Error", $"crProdutos_Tamanho.get error: {result.Error}");
            return (false, null, result.Error);
        }
    }
}
