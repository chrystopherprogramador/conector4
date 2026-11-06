using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence.Controllers.Fornecedores
{

    public class crFornecedores
    {
        private readonly PresenceApiHelper _api;

        public crFornecedores(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
        }

        public async Task<(bool Success, string Content, string Error)> Get(int tab, int offset = 0)
        {
            if (_api == null)
            {
                return (false, null, "PresenceApiHelper não inicializado");
            }
            try
            {
                var body = new { data_at = "2020-01-01" };
                var result = await _api.PostAsync("FORNECEDOR", body, offset);
                if (result == null)
                    return (false, null, "Resultado nulo da API");
                if (result.Success)
                    return (true, result.Content, null);
                return (false, null, result.Error);
            }
            catch (Exception ex)
            {
                // Não lançar para não travar o sistema
                System.Diagnostics.Trace.WriteLine("crFornecedores.get error: " + ex.Message);
                return (false, null, ex.Message);
            }
        }


    }
}
