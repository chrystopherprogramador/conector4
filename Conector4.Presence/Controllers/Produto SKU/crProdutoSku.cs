using Conector4.Presence.Controllers;
using System;
using System.Threading.Tasks;

namespace Conector4.Presence.Controllers.NovaPasta
{
    public class crProdutoSKU
    {
        private readonly PresenceApiHelper _api;

        public crProdutoSKU(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
        }

        public async Task<(bool Success, string Content, string Error)> get(int Tab, int offset = 0)
        {
            if (_api == null)
            {
                return (false, null, "PresenceApiHelper não inicializado");
            }

            try
            {
                var body = new { data_at = "2026-01-09" };
                var result = await _api.PostAsync("PRODUTOS_SKU", body, offset);
                if (result == null)
                    return (false, null, "Resultado nulo da API");

                if (result.Success)
                    return (true, result.Content, null);

                return (false, null, result.Error);
            }
            catch (Exception ex)
            {
                // Não lançar para não travar o sistema
                System.Diagnostics.Trace.WriteLine("crProdutoSKU.get error: " + ex.Message);
                return (false, null, ex.Message);
            }
        }
    }
}
