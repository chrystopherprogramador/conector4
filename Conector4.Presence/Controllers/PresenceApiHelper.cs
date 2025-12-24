using Conector4.Presence.Controllers.Token;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System;
using System.Threading.Tasks;
using Conector4.General.Elastic_Search;

namespace Conector4.Presence.Controllers
{
    public class PresenceApiHelper
    {
        private readonly Config _config;
        private readonly crToken _tokenProvider;
        private readonly ElasticLogger _logger = new ElasticLogger();

        public PresenceApiHelper(Config config = null, crToken tokenProvider = null)
        {
            _config = config ?? new Config();
            _tokenProvider = tokenProvider ?? new crToken();
        }

        public async Task<ApiResult> PostAsync(string metodo, object bodyObj, int offset = 0)
        {
            var result = new ApiResult();

            if (string.IsNullOrWhiteSpace(metodo))
            {
                result.Success = false;
                result.Error = "metodo inválido";
                await _logger.LogAsync("Error", "PresenceApiHelper.PostAsync: metodo inválido");
                return result;
            }

            string url = _config.UrlPresence;
            if (offset > 0)
                url += $"?offset={offset}";

            await _logger.LogAsync("Info", $"PresenceApiHelper.PostAsync start: metodo={metodo}, offset={offset}, url={url}");

            try
            {
                string token = null;
                try
                {
                    token = await _tokenProvider.GetToken(metodo);
                    await _logger.LogAsync("Info", $"Token obtido para metodo={metodo}; tamanho={token?.Length}");
                }
                catch (Exception exToken)
                {
                    result.Success = false;
                    result.Error = "Falha ao obter token: " + exToken.Message;
                    await _logger.LogAsync("Error", "Falha ao obter token: " + exToken.Message);
                    return result;
                }

                if (string.IsNullOrEmpty(token))
                {
                    result.Success = false;
                    result.Error = "Token inválido recebido do provedor.";
                    await _logger.LogAsync("Error", "Token inválido recebido do provedor.");
                    return result;
                }

                var client = new RestClient(url);

                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

                request.AddJsonBody(bodyObj);

                var response = await client.ExecuteAsync(request);

                if (response != null && response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    result.Success = true;
                    result.Content = response.Content;
                    await _logger.LogAsync("Info", $"PresenceApiHelper.PostAsync success: metodo={metodo}, status={(int)response.StatusCode}, contentLength={response.Content?.Length}");
                    return result;
                }

                var message = response?.Content ?? response?.ErrorMessage ?? "Resposta inválida";
                result.Success = false;
                result.Error = $"Presence API error (metodo={metodo}): {response?.StatusCode} - {message}";
                await _logger.LogAsync("Error", result.Error);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = "Erro na chamada PresenceApiHelper.PostAsync: " + ex.Message;
                await _logger.LogAsync("Error", result.Error);
                return result;
            }
        }
    }
}
