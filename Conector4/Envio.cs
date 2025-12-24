using Conector4.General.Elastic_Search;
using Conector4.General.Rabbit;
using Conector4.Presence.Controllers.NovaPasta;
using Conector4.Presence.Controllers.PRODUTOS_TAMANHO;
using Conector4.Presence.Global;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4
{
    public class Envio
    {
        RabbitProducer producer = new RabbitProducer();
        private readonly ElasticLogger _logger = new ElasticLogger();

        // Guards to prevent overlapping runs
        private int _isRunningTabPreco = 0;
        private int _isRunningProdutoSku = 0;
        private int _isRunningGrade = 0;

        // Max pages to process per timer tick to keep cycles short
        private const int MaxPagesPerRun = 100000;
        public async Task EnviarPrecoParaFilaAsync()
        {
            // Dispara as duas tabelas em paralelo
            var task1 = ProcessarTabelaAsync(1, MaxPagesPerRun);
            var task2 = ProcessarTabelaAsync(2, MaxPagesPerRun);

            await Task.WhenAll(task1, task2);
        }

        public async Task ProcessarProdutosFaltantesAsync()
        {
            try
            {
                await _logger.LogAsync("Warning", "Sincronizando preços de produtos faltantes");

                var tabPreco = new crTabPreco();

                var list = new List<string>();

                list = await tabPreco.RetornaFaltantes();
                (bool Success, string Content, string Error) result;
                foreach (var item in list)
                {

                    for(var i = 1; i<= 2; i++)
                    {
                        result = await tabPreco.getTabPrecoInd(item, i);
                        await _logger.LogAsync("Warning", $"Processando Produto {item} Tabela {i}");
                        await _logger.LogAsync("Payload", $"Item: {item} payload: {result.Content}");
                        if (result.Success)
                        {
                            var jsonObject = JObject.Parse(result.Content);
                            if (jsonObject["data"] is JArray dataArray && dataArray.Count == 0)
                            {
                                await _logger.LogAsync("Info", $"Produto {item} Tabela {i}: Nenhum dado retornado, finalizando.");
                                break;
                            }
                        }
                        else
                        {
                            await _logger.LogAsync("Info", $"Produto {item} Tabela {i}: Nenhum dado retornado, finalizando.");
                            break;
                        }

                        string json = result.Content;
                        await producer.PublishAsync(
                            queueName: "Presence.TabPrecoInd",
                            json: json
                        );

                        ApiPagedResponse meta = null;
                        try
                        {
                            meta = JsonConvert.DeserializeObject<ApiPagedResponse>(json);
                        }
                        catch (Exception ex)
                        {
                            await _logger.LogAsync("Error", $"Tabela {i}|Código: {item} falha ao desserializar resposta - {ex.Message}");
                            return;
                        }

                    }

                }



            }
            catch (Exception ex)
            {
                await _logger.LogAsync("error","Erro ao processar Produtos Faltantes" + ex.Message);
            }
        }
        private async Task ProcessarTabelaAsync(int tabelaId, int maxPages)
        {
            int offset = 0;
            int pages = 0;

            await _logger.LogAsync("Warning", $"Sincronizando tabela de preço {tabelaId}");

            while (pages < maxPages)
            {

                // Loop para passar cada item como parâmetro


                    var tabPreco = new crTabPreco();

                    (bool Success, string Content, string Error) result;
                    try
                    {
                        result = await tabPreco.getTabPreco(tabelaId, offset);
                        if (result.Success)
                        {
                            var jsonObject = JObject.Parse(result.Content);

                            if (jsonObject["data"] is JArray dataArray && dataArray.Count == 0)
                            {
                                await _logger.LogAsync("Info", $"Tabela {tabelaId}: Nenhum dado retornado, finalizando.");
                                break;
                            }
                        }
                        else
                        {
                            await _logger.LogAsync("Info", $"Tabela {tabelaId}: Nenhum dado retornado, finalizando.");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogAsync("Error", $"Tabela {tabelaId}: erro ao chamar getTabPreco - {ex.Message}");
                        return;
                    }

                    if (!result.Success)
                    {
                        await _logger.LogAsync("Error", $"Tabela {tabelaId}: crTabPreco error: {result.Error}");
                        return;
                    }

                    string json = result.Content;

                    if (string.IsNullOrEmpty(json))
                    {
                        await _logger.LogAsync("Error", $"Tabela {tabelaId}: crTabPreco retornou conteúdo nulo ou vazio.");
                        return;
                    }

                    await producer.PublishAsync(
                        queueName: "Presence.TabPreco",
                        json: json
                    );

                    ApiPagedResponse meta = null;
                    try
                    {
                        meta = JsonConvert.DeserializeObject<ApiPagedResponse>(json);
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogAsync("Error", $"Tabela {tabelaId}: falha ao desserializar resposta - {ex.Message}");
                        return;
                    }

                    if (meta?.data == null || meta.data.Count == 0)
                        break;

                    offset = meta.nextOffSet;
                    if (offset <= 0)
                        break;

                    pages++;

                }
            }

        public async Task EnviarProdutoSkuParaFilaAsync()
        {
            int offset = 0;
            int pages = 0;
            int maxPages = MaxPagesPerRun;

            while (pages < maxPages)
            {
                var produtoSku = new crProdutoSKU();

                (bool Success, string Content, string Error) result;
                try
                {
                    result = await produtoSku.get(0, offset);
                    var tratado = JsonConvert.DeserializeObject<dynamic>(result.Content);
                    if (result.Success)
                    {
                        var jsonObject = JObject.Parse(result.Content);

                        if (jsonObject["data"] is JArray dataArray)
                        {
                            if (dataArray.Count == 0)
                            {
                                await _logger.LogAsync("Info", "EnviarTabPrecoFilaAsync: Nenhum dado retornado, finalizando.");
                                pages = maxPages;
                            }
                        }
                    }
                    else
                    {
                        await _logger.LogAsync("Info", "EnviarTabPrecoFilaAsync: Nenhum dado retornado, finalizando.");
                        pages = maxPages;
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", "EnviarProdutoSkuParaFilaAsync: erro ao chamar produtoSku.get - " + ex.Message);
                    return;
                }

                if (!result.Success)
                {
                    await _logger.LogAsync("Error", "crProdutoSKU error: " + result.Error);
                    return;
                }

                string json = result.Content;

                if (string.IsNullOrEmpty(json))
                {
                    await _logger.LogAsync("Error", "crProdutoSKU retornou conteúdo nulo ou vazio.");
                    return;
                }

                try
                {
                    await producer.PublishAsync(
                        queueName: "Presence.ProdutoSKU",
                        json: json
                    );
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", "EnviarProdutoSkuParaFilaAsync: falha ao publicar no Rabbit - " + ex.Message);
                    return;
                }

                ApiPagedResponse meta = null;
                try
                {
                    meta = JsonConvert.DeserializeObject<ApiPagedResponse>(json);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", "EnviarProdutoSkuParaFilaAsync: falha ao desserializar resposta - " + ex.Message);
                    return;
                }

                if (meta?.data == null || meta.data.Count == 0)
                    break;

                offset = meta.nextOffSet;
                if (offset <= 0)
                    break;

                pages++;
            }
        }

        public async Task ProcessarGradeAsync()
        {
            int offset = 0;
            int pages = 0;
            int maxPages = MaxPagesPerRun;

            while (pages < maxPages)
            {
                var produtoSku = new crProdutos_Tamanho();

                (bool Success, string Content, string Error) result;
                try
                {
                    result = await produtoSku.get(offset);
                    var tratado = JsonConvert.DeserializeObject<dynamic>(result.Content);
                    if (result.Success)
                    {
                        var jsonObject = JObject.Parse(result.Content);

                        if (jsonObject["data"] is JArray dataArray)
                        {
                            if (dataArray.Count == 0)
                            {
                                await _logger.LogAsync("Info", "EnviarcrProdutos_Tamanho: Nenhum dado retornado, finalizando.");
                                pages = maxPages;
                            }
                        }
                    }
                    else
                    {
                        await _logger.LogAsync("Info", "EnviarcrProdutos_Tamanho: Nenhum dado retornado, finalizando.");
                        pages = maxPages;
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", "EnviarcrProdutos_Tamanho: erro ao chamar crProdutos_Tamanho.get - " + ex.Message);
                    return;
                }

                if (!result.Success)
                {
                    await _logger.LogAsync("Error", "crProdutos_Tamanho error: " + result.Error);
                    return;
                }

                string json = result.Content;

                if (string.IsNullOrEmpty(json))
                {
                    await _logger.LogAsync("Error", "crProdutos_Tamanho retornou conteúdo nulo ou vazio.");
                    return;
                }

                try
                {
                    await producer.PublishAsync(
                        queueName: "Presence.Produtos_Tamanho",
                        json: json
                    );
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", "EnviarProcessarGradeAsync: falha ao publicar no Rabbit - " + ex.Message);
                    return;
                }

                ApiPagedResponse meta = null;
                try
                {
                    meta = JsonConvert.DeserializeObject<ApiPagedResponse>(json);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", "EnviarProcessarGradeAsync: falha ao desserializar resposta - " + ex.Message);
                    return;
                }

                if (meta?.data == null || meta.data.Count == 0)
                    break;

                offset = meta.nextOffSet;
                if (offset <= 0)
                    break;

                pages++;
            }
        }
    }
}
