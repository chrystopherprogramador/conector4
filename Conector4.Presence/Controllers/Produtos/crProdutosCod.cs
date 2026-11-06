using Conector4.General;
using Conector4.General.Elastic_Search;
using Conector4.General.Rabbit;
using Conector4.Presence.Controllers.Estoque;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Conector4.Presence.Controllers.Produtos
{
    public class crProdutosCod
    {

        private readonly PresenceApiHelper _api;
        private readonly clsConfig _config;
        private ElasticLogger _logger = new ElasticLogger();
        private RabbitProducer _producer = new RabbitProducer();

        // Controle de taxa de requisições
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static int _requestCount = 0;
        private static DateTime _lastRequestReset = DateTime.Now;
        private const int MAX_REQUESTS_PER_MINUTE = 50; // Ajuste conforme limite da API
        private const int BASE_DELAY_MS = 1500; // Delay base entre requisições (1.5 segundos)

        public crProdutosCod(PresenceApiHelper api = null)
        {
            _api = api ?? new PresenceApiHelper();
            _config = new clsConfig();
        }

        /// <summary>
        /// Controla taxa de requisições e aguarda se necessário
        /// </summary>
        private async Task<bool> AguardarRateLimitAsync(Action<string> logCallback = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                // Reset contador a cada minuto
                if ((DateTime.Now - _lastRequestReset).TotalMinutes >= 1)
                {
                    _requestCount = 0;
                    _lastRequestReset = DateTime.Now;
                    logCallback?.Invoke($"[RATE LIMIT] Contador resetado. Nova janela de 1 minuto iniciada.");
                }

                // Verificar se atingiu o limite
                if (_requestCount >= MAX_REQUESTS_PER_MINUTE)
                {
                    var tempoEspera = 60 - (DateTime.Now - _lastRequestReset).TotalSeconds;
                    if (tempoEspera > 0)
                    {
                        logCallback?.Invoke($"[RATE LIMIT] ⚠ LIMITE ATINGIDO! Aguardando {tempoEspera:F0} segundos...");
                        await Task.Delay(TimeSpan.FromSeconds(tempoEspera + 1));
                        _requestCount = 0;
                        _lastRequestReset = DateTime.Now;
                    }
                }

                _requestCount++;

                // Avisar quando estiver próximo do limite
                if (_requestCount >= MAX_REQUESTS_PER_MINUTE * 0.8)
                {
                    logCallback?.Invoke($"[RATE LIMIT] ⚠ Aviso: {_requestCount}/{MAX_REQUESTS_PER_MINUTE} requisições usadas");
                }

                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Executa requisição com retry e backoff exponencial
        /// </summary>
        private async Task<(bool Success, string Content, string Error)> ExecutarComRetryAsync(
            Func<Task<(bool Success, string Content, string Error)>> operacao,
            string nomeOperacao,
            int maxRetries = 3,
            Action<string> logCallback = null)
        {
            int tentativa = 0;
            int delayMs = BASE_DELAY_MS;

            while (tentativa < maxRetries)
            {
                tentativa++;

                try
                {
                    // Aguardar rate limit
                    await AguardarRateLimitAsync(logCallback);

                    // Executar operação
                    var resultado = await operacao();

                    // Se sucesso, retornar
                    if (resultado.Success)
                    {
                        return resultado;
                    }

                    // Se erro 429 (TooManyRequests), fazer retry com backoff
                    if (resultado.Error != null && resultado.Error.Contains("TooManyRequests"))
                    {
                        if (tentativa < maxRetries)
                        {
                            logCallback?.Invoke($"[RETRY] ⚠ Erro 429 em {nomeOperacao}. Tentativa {tentativa}/{maxRetries}. Aguardando {delayMs / 1000}s...");
                            await Task.Delay(delayMs);
                            delayMs *= 2; // Backoff exponencial
                            continue;
                        }
                    }

                    // Outros erros, retornar imediatamente
                    return resultado;
                }
                catch (Exception ex)
                {
                    if (tentativa < maxRetries)
                    {
                        logCallback?.Invoke($"[RETRY] ⚠ Exceção em {nomeOperacao}. Tentativa {tentativa}/{maxRetries}. Erro: {ex.Message}");
                        await Task.Delay(delayMs);
                        delayMs *= 2;
                        continue;
                    }

                    return (false, null, ex.Message);
                }
            }

            return (false, null, $"Falha após {maxRetries} tentativas");
        }

        public async Task<(bool Success, string Content, string Error)> Get(int offset = 0, string codigo = "")
        {
            if (_api == null)
            {
                return (false, null, "PresenceApiHelper não inicializado");
            }

            try
            {
                var body = new { codigo = codigo };
                var result = await _api.PostAsync("PRODUTOS", body, offset);

                if (result == null)
                    return (false, null, "Resultado nulo da API");

                if (result.Success)
                    return (true, result.Content, null);

                return (false, null, result.Error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("crProdutos.Get error: " + ex.Message);
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Busca produto, SKU e estoque de forma integrada
        /// </summary>
        public async Task<(bool Success, string ProdutoContent, string SkuContent, string EstoqueContent, string Error)> GetCompleto(
            string codigo, 
            string loja = "01")
        {
            try
            {
                // 1. Buscar Produto
                var produtoResult = await Get(0, codigo);
                if (!produtoResult.Success)
                {
                    return (false, null, null, null, $"Erro ao buscar produto: {produtoResult.Error}");
                }

                // 2. Buscar SKU
                var skuController = new crProdutosSku();
                var skuResult = await skuController.Get(codigo);

                // 3. Buscar Estoque
                var estoqueController = new crEstoque();
                var estoqueResult = await estoqueController.Get(loja);

                return (
                    true,
                    produtoResult.Content,
                    skuResult.Success ? skuResult.Content : null,
                    estoqueResult.Success ? estoqueResult.Content : null,
                    null
                );
            }
            catch (Exception ex)
            {
                await _logger.LogAsync("Error", $"Erro ao buscar dados completos do produto {codigo}: {ex.Message}");
                return (false, null, null, null, ex.Message);
            }
        }

        /// <summary>
        /// Busca múltiplos produtos e publica no RabbitMQ
        /// </summary>
        public async Task<(int Sucesso, int Falhas, List<string> CodigosComErro)> BuscarEPublicarCodigos(
            List<string> codigos, 
            string nomeFila = "Presence.Produtos",
            IProgress<(int Atual, int Total, string Codigo)> progresso = null)
        {
            int sucesso = 0;
            int falhas = 0;
            List<string> codigosComErro = new List<string>();

            await _logger.LogAsync("Info", $"Iniciando busca de {codigos.Count} produtos para publicação no RabbitMQ");

            for (int i = 0; i < codigos.Count; i++)
            {
                string codigo = codigos[i];

                try
                {
                    // Reportar progresso
                    progresso?.Report((i + 1, codigos.Count, codigo));

                    // Buscar produto na API
                    var result = await Get(0, codigo);

                    if (result.Success && !string.IsNullOrEmpty(result.Content))
                    {
                        // Publicar no RabbitMQ
                        await _producer.PublishAsync(nomeFila, result.Content);
                        sucesso++;
                        
                        await _logger.LogAsync("Info", $"Produto {codigo} publicado com sucesso na fila {nomeFila}");
                    }
                    else
                    {
                        falhas++;
                        codigosComErro.Add(codigo);
                        await _logger.LogAsync("Warning", $"Produto {codigo} não encontrado ou erro: {result.Error}");
                    }

                    // Pequeno delay para não sobrecarregar a API
                    await Task.Delay(50);
                }
                catch (Exception ex)
                {
                    falhas++;
                    codigosComErro.Add(codigo);
                    await _logger.LogAsync("Error", $"Erro ao processar produto {codigo}: {ex.Message}");
                }
            }

            await _logger.LogAsync("Info", $"Busca concluída. Sucesso: {sucesso}, Falhas: {falhas}");

            return (sucesso, falhas, codigosComErro);
        }

        /// <summary>
        /// Busca produtos, SKUs e estoque e processa direto no banco de dados
        /// </summary>
        public async Task<(int Sucesso, int Falhas, Dictionary<string, int> ResultadosPorTipo)> BuscarEProcessarCompleto(
            List<string> codigos,
            string loja = "01",
            IProgress<(int Atual, int Total, string Codigo, string Etapa)> progresso = null,
            Action<string> logCallback = null)
        {
            int sucessoTotal = 0;
            int falhasTotal = 0;
            var resultadosPorTipo = new Dictionary<string, int>
            {
                ["Produtos"] = 0,
                ["SKUs"] = 0,
                ["Estoques"] = 0
            };

            logCallback?.Invoke($"[INFO] Iniciando processamento COMPLETO de {codigos.Count} produtos (Produto + SKU + Estoque)");
            logCallback?.Invoke($"[INFO] Rate Limit: Máximo {MAX_REQUESTS_PER_MINUTE} requisições por minuto");
            logCallback?.Invoke($"[INFO] Delay entre operações: {BASE_DELAY_MS}ms");
            await _logger.LogAsync("Info", $"Iniciando processamento COMPLETO de {codigos.Count} produtos");

            var produtosController = new crProdutos();
            var skuController = new crProdutosSku();
            var estoqueController = new crEstoque();

            for (int i = 0; i < codigos.Count; i++)
            {
                string codigo = codigos[i];
                bool erroNesteCodigo = false;

                logCallback?.Invoke($"\n═══════════════════════════════════════════");
                logCallback?.Invoke($"[{i + 1}/{codigos.Count}] Processando código: {codigo}");
                logCallback?.Invoke($"═══════════════════════════════════════════");

                try
                {
                    // 1. BUSCAR E PROCESSAR PRODUTO COM RETRY
                    progresso?.Report((i + 1, codigos.Count, codigo, "Buscando Produto"));
                    logCallback?.Invoke($"[PRODUTO] Buscando produto {codigo} na API...");
                    
                    var produtoResult = await ExecutarComRetryAsync(
                        async () => await Get(0, codigo),
                        $"PRODUTO {codigo}",
                        3,
                        logCallback
                    );

                    if (produtoResult.Success && !string.IsNullOrEmpty(produtoResult.Content))
                    {
                        logCallback?.Invoke($"[PRODUTO] ✓ Produto {codigo} encontrado na API");
                        logCallback?.Invoke($"[PRODUTO] Processando produto no banco de dados...");
                        
                        progresso?.Report((i + 1, codigos.Count, codigo, "Salvando Produto"));
                        await produtosController.Insert(produtoResult.Content);
                        
                        resultadosPorTipo["Produtos"]++;
                        logCallback?.Invoke($"[PRODUTO] ✓ Produto {codigo} salvo no banco com sucesso");
                        await _logger.LogAsync("Info", $"Produto {codigo} processado");
                    }
                    else
                    {
                        erroNesteCodigo = true;
                        logCallback?.Invoke($"[PRODUTO] ✗ ERRO: Produto {codigo} não encontrado - {produtoResult.Error}");
                        await _logger.LogAsync("Warning", $"Produto {codigo} não encontrado: {produtoResult.Error}");
                    }

                    await Task.Delay(BASE_DELAY_MS); // Delay fixo entre tipos

                    // 2. BUSCAR E PROCESSAR SKU COM RETRY
                    progresso?.Report((i + 1, codigos.Count, codigo, "Buscando SKU"));
                    logCallback?.Invoke($"[SKU] Buscando SKUs do produto {codigo} na API...");
                    
                    var skuResult = await ExecutarComRetryAsync(
                        async () => await skuController.Get(codigo),
                        $"SKU {codigo}",
                        3,
                        logCallback
                    );

                    if (skuResult.Success && !string.IsNullOrEmpty(skuResult.Content))
                    {
                        logCallback?.Invoke($"[SKU] ✓ SKUs encontrados na API");
                        logCallback?.Invoke($"[SKU] Processando SKUs no banco de dados...");
                        
                        progresso?.Report((i + 1, codigos.Count, codigo, "Salvando SKU"));
                        int skusInseridos = await skuController.Insert(skuResult.Content);
                        
                        resultadosPorTipo["SKUs"]++;
                        logCallback?.Invoke($"[SKU] ✓ {skusInseridos} SKU(s) salvos no banco com sucesso");
                        await _logger.LogAsync("Info", $"SKU {codigo} processado - {skusInseridos} registros");
                    }
                    else
                    {
                        logCallback?.Invoke($"[SKU] ⚠ Aviso: SKUs não encontrados para {codigo} - {skuResult.Error}");
                        await _logger.LogAsync("Warning", $"SKU {codigo} não encontrado");
                    }

                    await Task.Delay(BASE_DELAY_MS); // Delay fixo entre tipos

                    // 3. BUSCAR E PROCESSAR ESTOQUE COM RETRY
                    progresso?.Report((i + 1, codigos.Count, codigo, "Buscando Estoque"));
                    logCallback?.Invoke($"[ESTOQUE] Buscando estoque da loja {loja} na API...");
                    
                    var estoqueResult = await ExecutarComRetryAsync(
                        async () => await estoqueController.Get(loja),
                        $"ESTOQUE loja {loja}",
                        3,
                        logCallback
                    );

                    if (estoqueResult.Success && !string.IsNullOrEmpty(estoqueResult.Content))
                    {
                        logCallback?.Invoke($"[ESTOQUE] ✓ Estoque encontrado na API");
                        logCallback?.Invoke($"[ESTOQUE] Processando estoque no banco de dados...");
                        
                        progresso?.Report((i + 1, codigos.Count, codigo, "Salvando Estoque"));
                        await estoqueController.Insert(estoqueResult.Content);
                        
                        resultadosPorTipo["Estoques"]++;
                        logCallback?.Invoke($"[ESTOQUE] ✓ Estoque da loja {loja} salvo no banco com sucesso");
                        await _logger.LogAsync("Info", $"Estoque loja {loja} processado");
                    }
                    else
                    {
                        logCallback?.Invoke($"[ESTOQUE] ⚠ Aviso: Estoque não encontrado para loja {loja} - {estoqueResult.Error}");
                        await _logger.LogAsync("Warning", $"Estoque loja {loja} não encontrado");
                    }

                    if (!erroNesteCodigo)
                    {
                        sucessoTotal++;
                        logCallback?.Invoke($"[RESULTADO] ✓✓✓ Código {codigo} processado com SUCESSO TOTAL");
                    }
                    else
                    {
                        falhasTotal++;
                        logCallback?.Invoke($"[RESULTADO] ✗✗✗ Código {codigo} processado com FALHAS");
                    }

                    await Task.Delay(BASE_DELAY_MS); // Delay entre códigos
                }
                catch (Exception ex)
                {
                    falhasTotal++;
                    logCallback?.Invoke($"[ERRO] ✗✗✗ EXCEÇÃO ao processar código {codigo}: {ex.Message}");
                    logCallback?.Invoke($"[ERRO] Stack: {ex.StackTrace}");
                    await _logger.LogAsync("Error", $"Erro ao processar código {codigo}: {ex.Message}");
                }
            }

            logCallback?.Invoke($"\n╔═══════════════════════════════════════════╗");
            logCallback?.Invoke($"║  PROCESSAMENTO COMPLETO FINALIZADO        ║");
            logCallback?.Invoke($"╚═══════════════════════════════════════════╝");
            logCallback?.Invoke($"✓ Sucesso Total: {sucessoTotal}");
            logCallback?.Invoke($"✗ Falhas Total: {falhasTotal}");
            logCallback?.Invoke($"📦 Produtos processados: {resultadosPorTipo["Produtos"]}");
            logCallback?.Invoke($"🏷️ SKUs processados: {resultadosPorTipo["SKUs"]}");
            logCallback?.Invoke($"📊 Estoques processados: {resultadosPorTipo["Estoques"]}");

            await _logger.LogAsync("Info", 
                $"Processamento COMPLETO concluído. Sucesso: {sucessoTotal}, Falhas: {falhasTotal}. " +
                $"Produtos: {resultadosPorTipo["Produtos"]}, SKUs: {resultadosPorTipo["SKUs"]}, Estoques: {resultadosPorTipo["Estoques"]}");

            return (sucessoTotal, falhasTotal, resultadosPorTipo);
        }

        /// <summary>
        /// Busca produtos em lotes e publica no RabbitMQ
        /// </summary>
        public async Task<(int Sucesso, int Falhas)> BuscarEPublicarEmLotes(
            List<string> codigos,
            int tamanhoDosLotes = 50,
            string nomeFila = "Presence.Produtos",
            IProgress<(int Lote, int TotalLotes, int TotalCodigos)> progresso = null)
        {
            int sucessoTotal = 0;
            int falhasTotal = 0;

            await _logger.LogAsync("Info", $"Iniciando busca em lotes de {tamanhoDosLotes}. Total: {codigos.Count} produtos");

            int totalLotes = (int)Math.Ceiling((double)codigos.Count / tamanhoDosLotes);

            for (int i = 0; i < codigos.Count; i += tamanhoDosLotes)
            {
                int loteAtual = (i / tamanhoDosLotes) + 1;
                int take = Math.Min(tamanhoDosLotes, codigos.Count - i);
                var lote = codigos.Skip(i).Take(take).ToList();

                try
                {
                    // Reportar progresso
                    progresso?.Report((loteAtual, totalLotes, i + take));

                    // Buscar lote na API (se API suportar array)
                    var body = new { codigos = lote.ToArray() };
                    var result = await _api.PostAsync("PRODUTOS", body, 0);

                    if (result != null && result.Success && !string.IsNullOrEmpty(result.Content))
                    {
                        // Publicar no RabbitMQ
                        await _producer.PublishAsync(nomeFila, result.Content);
                        sucessoTotal += take;
                        
                        await _logger.LogAsync("Info", $"Lote {loteAtual}/{totalLotes} publicado com sucesso ({take} produtos)");
                    }
                    else
                    {
                        falhasTotal += take;
                        await _logger.LogAsync("Warning", $"Lote {loteAtual}/{totalLotes} com erro: {result?.Error}");
                    }

                    // Delay entre lotes
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    falhasTotal += take;
                    await _logger.LogAsync("Error", $"Erro no lote {loteAtual}/{totalLotes}: {ex.Message}");
                }
            }

            await _logger.LogAsync("Info", $"Busca em lotes concluída. Sucesso: {sucessoTotal}, Falhas: {falhasTotal}");

            return (sucessoTotal, falhasTotal);
        }
    }
}


