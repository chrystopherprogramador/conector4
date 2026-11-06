using Conector4.General.Rabbit;
using Conector4.Presence.Controllers.NovaPasta;
using Conector4.Presence.Controllers.Tabela_de_Preco;
using System.Net.Http.Json;
using System.Text;
using Conector4.General.Elastic_Search;
using Conector4.Presence.Controllers.PRODUTOS_TAMANHO;
using Conector4.Presence.Controllers.Fornecedores;
using Conector4.Presence.Controllers.Estoque;
using Conector4.Presence.Controllers.Produtos;

namespace Conector4.Mensageria
{
    public class RabbitPeekMessage
    {
        public string payload { get; set; } = "";
        public long payload_bytes { get; set; }
        public int message_count { get; set; }
        public bool redelivered { get; set; }
        public Dictionary<string, object>? properties { get; set; }
    }

    public class clsFila
    {
        private bool _started = false;
        private readonly object _lock = new object();
        private readonly ElasticLogger _logger = new ElasticLogger();
        private readonly HttpClient _httpClient;

        public clsFila()
        {
            _httpClient = new HttpClient();
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("ti:Ti@2015!*"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);
        }

        public Task fila()
        {
            lock (_lock)
            {
                if (_started)
                    return Task.CompletedTask;

                _started = true;

                var config = new Conector4.General.clsConfig();
                var consumer = new RabbitConsumer();

                // Helper para rodar N consumers concorrentes por fila
                async Task RunConsumer<TRepo>(string queue, Func<TRepo> repoFactory, Func<TRepo, string, Task> importFunc, int workers = 5)
                {
                    for (int i = 0; i < workers; i++)
                    {
                        _ = Task.Run(async () =>
                        {
                            await _logger.LogAsync("Info", $"Starting consumer {queue} worker {i}");
                            await consumer.StartAsync(queue, async json =>
                            {
                                try
                                {
                                    var repo = repoFactory();

                                    // Se vier lote de mensagens, dispara todas em paralelo
                                    var mensagens = json.Split('\n'); // ajuste conforme formato real
                                    var tasks = mensagens
                                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                                        .Select(msg => importFunc(repo, msg));

                                    await Task.WhenAll(tasks); // roda todas em paralelo

                                    await _logger.LogAsync("Info", $"{queue} worker {i} processed {mensagens.Length} mensagens");
                                }
                                catch (Exception ex)
                                {
                                    await _logger.LogAsync("Error", $"{queue} worker {i} failed: {ex.Message}");
                                    throw; // deixa o Rabbit requeue
                                }
                            });
                        });
                    }
                }

                // Consumers otimizados (cada fila com múltiplos workers)
                _ = RunConsumer("Presence.ProdutoSKU",
                    () => new ProdutosSkuRepository(config.getConfigBD()),
                    (repo, msg) => repo.ImportarProdutosSkuAsync(msg), workers: 5);

                _ = RunConsumer("Presence.TabPreco",
                    () => new crTabPrecoInput(config.getConfigBD()),
                    (repo, msg) => repo.ImportarAsync(msg), workers: 5);

                _ = RunConsumer("Presence.TabPrecoInd",
                    () => new crTabPrecoInput(config.getConfigBD()),
                    (repo, msg) => repo.ImportarAsyncInd(msg), workers: 5);

                _ = RunConsumer("Presence.Produtos_Tamanho",
                    () => new crProdutos_Recebe(config.getConfigBD()),
                    (repo, msg) => repo.Importar(msg), workers: 5);

                _ = RunConsumer("Presence.TabFornecedores", () => new crInFornecedores(config.getConfigBD()),
                    (repo, msg) => repo.ImportarAsync(msg), workers: 5);

                _ = RunConsumer("Presence.Estoque", () => new crEstoque(),
                    (repo, msg) => repo.Insert(msg), workers: 5);

                _ = RunConsumer("Presence.Produtos", () => new crProdutos(),
                    (repo, msg) => repo.Insert(msg), workers: 5);

                _ = RunConsumer("Presence.VendasHeader", () => new crVendaHeader(),
                    (repo, msg) => repo.Insert(msg), workers: 5);

                return Task.CompletedTask;
            }
        }


        public async Task<List<RabbitPeekMessage>> GetQueueMessagesAsync(string queue)
        {
            try
            {
                var url = $"http://rabbit.websamos.com.br/api/queues/%2F/{queue}/get";

                var body = new
                {
                    count = 200, // pega mais mensagens por vez
                    ackmode = "ack_requeue_true",
                    encoding = "auto"
                };

                var response = await _httpClient.PostAsJsonAsync(url, body);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<RabbitPeekMessage>>();
                return result ?? new List<RabbitPeekMessage>();
            }
            catch (Exception ex)
            {
                await _logger.LogAsync("Error", $"GetQueueMessagesAsync failed for queue={queue}: {ex.Message}");
                return new List<RabbitPeekMessage>();
            }
        }
    }
}
