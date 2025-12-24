using Conector4.Presence.Models.Token;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Conector4.General.Elastic_Search;

namespace Conector4.Presence.Controllers.Token {
   
    public class crToken
    {

        Config config = new Config();
        Conector4.General.clsKey key = new Conector4.General.clsKey();
        private readonly ElasticLogger _logger = new ElasticLogger();

        // cache por metodo
        private static readonly ConcurrentDictionary<string, (string Token, DateTime Expires)> _cache = new();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        private static SemaphoreSlim GetLock(string metodo) => _locks.GetOrAdd(metodo, _ => new SemaphoreSlim(1,1));

        public async Task<string> GetToken(string metodo)
        {
            if (string.IsNullOrWhiteSpace(metodo))
                throw new ArgumentException("metodo inválido", nameof(metodo));

            // verifica cache
            if (_cache.TryGetValue(metodo, out var entry))
            {
                if (entry.Expires > DateTime.UtcNow && !string.IsNullOrEmpty(entry.Token))
                {
                    await _logger.LogAsync("Info", $"GetToken: retornando token em cache para metodo={metodo}");
                    return entry.Token;
                }
            }

            var sem = GetLock(metodo);
            await sem.WaitAsync();
            try
            {
                // checar novamente dentro do lock
                if (_cache.TryGetValue(metodo, out entry))
                {
                    if (entry.Expires > DateTime.UtcNow && !string.IsNullOrEmpty(entry.Token))
                    {
                        await _logger.LogAsync("Info", $"GetToken: retornando token em cache (pos-lock) para metodo={metodo}");
                        return entry.Token;
                    }
                }

                var keys = await key.GetKeysAsync(General.clsKey.PlataformType.Presence);

                if (keys == null || keys.Count == 0)
                {
                    await _logger.LogAsync("Error", "GetToken: chaves não encontradas para a plataforma Presence.");
                    throw new InvalidOperationException("Chaves não encontradas para a plataforma Presence.");
                }

                // Tenta cada credencial até obter um token válido
                foreach (var credential in keys)
                {
                    await _logger.LogAsync("Info", $"Tentando credencial user={credential.User}");

                    var client = new RestClient(config.UrlPrecenceToken);
                    var request = new RestRequest();
                    request.Method = Method.Post;
                    request.AddHeader("Accept", "application/json");
                    request.AddHeader("Content-Type", "application/json");
                    request.Authenticator = new HttpBasicAuthenticator(credential.User ?? string.Empty, credential.Pass ?? string.Empty);

                    var bodyObj = new
                    {
                        sistema = "ER5",
                        serie = 1916,
                        metodo = metodo
                    };
                    string body = Newtonsoft.Json.JsonConvert.SerializeObject(bodyObj);
                    request.AddBody(body, "application/json");

                    RestResponse response = null;
                    try
                    {
                        response = await client.ExecuteAsync(request);
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogAsync("Error", $"GetToken: falha na chamada para user={credential.User}: {ex.Message}");
                        continue;
                    }

                    if (response == null || !response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                    {
                        await _logger.LogAsync("Error", $"GetToken: resposta inválida para user={credential.User}; status={response?.StatusCode}");
                        continue;
                    }

                    moToken tokenPresece = null;
                    try
                    {
                        tokenPresece = Newtonsoft.Json.JsonConvert.DeserializeObject<moToken>(response.Content);
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogAsync("Error", $"GetToken: falha ao desserializar resposta para user={credential.User}: {ex.Message}");
                        continue;
                    }

                    if (tokenPresece != null && !string.IsNullOrEmpty(tokenPresece.token))
                    {
                        string tk = tokenPresece.token;

                        // definir expiracao conservadora (9 minutos)
                        var expires = DateTime.UtcNow.AddMinutes(9);
                        _cache[metodo] = (tk, expires);

                        await _logger.LogAsync("Info", $"GetToken: token obtido user={credential.User}; tamanho={tk.Length}");
                        return tk;
                    }

                    await _logger.LogAsync("Error", $"GetToken: token vazio para user={credential.User}");
                }

                await _logger.LogAsync("Error", "GetToken: não foi possível obter token usando as credenciais configuradas.");
                throw new Exception("Não foi possível obter token usando as credenciais configuradas.");
            }
            finally
            {
                sem.Release();
            }
        }



    }
}
