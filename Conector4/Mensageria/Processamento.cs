using Conector4.General.Elastic_Search;
using Conector4.General.Rabbit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Mensageria
{
    public class Processamento
    {
        RabbitProducer producer = new RabbitProducer();
        private readonly ElasticLogger _logger = new ElasticLogger();

        // Guards to prevent overlapping runs
        private int _isRunningTabPreco = 0;
        private int _isRunningProdutoSku = 0;
        private int _isRunningGrade = 0;

        // Max pages to process per timer tick to keep cycles short
        private const int MaxPagesPerRun = 100000;

        public async Task ProcessarPaginadoAsync(
            Func<int, Task<(bool Success, string Content, string Error)>> chamadaApi,
            string nomeFila,
            string nomeProcesso,
            int maxPages = MaxPagesPerRun
        )
        {
            int offset = 0;
            int pages = 0;

            await _logger.LogAsync("Warning", $"Sincronizando {nomeProcesso}");

            while (pages < maxPages)
            {
                (bool Success, string Content, string Error) result;

                try
                {
                    result = await chamadaApi(offset);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", $"{nomeProcesso}: erro na chamada da API - {ex.Message}");
                    return;
                }

                if (!result.Success)
                {
                    await _logger.LogAsync("Error", $"{nomeProcesso}: erro retornado - {result.Error}");
                    return;
                }

                if (string.IsNullOrEmpty(result.Content))
                {
                    await _logger.LogAsync("Error", $"{nomeProcesso}: conteúdo vazio.");
                    return;
                }

                JObject jsonObject;
                try
                {
                    jsonObject = JObject.Parse(result.Content);
                }
                catch
                {
                    await _logger.LogAsync("Error", $"{nomeProcesso}: JSON inválido.");
                    return;
                }

                if (jsonObject["data"] is JArray data && data.Count == 0)
                {
                    await _logger.LogAsync("Info", $"{nomeProcesso}: nenhum dado retornado.");
                    break;
                }

                await producer.PublishAsync(nomeFila, result.Content);

                ApiPagedResponse meta;
                try
                {
                    meta = JsonConvert.DeserializeObject<ApiPagedResponse>(result.Content);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", $"{nomeProcesso}: erro ao desserializar meta - {ex.Message}");
                    return;
                }

                offset = meta.nextOffSet;
                if (offset <= 0)
                    break;

                pages++;
            }
        }


    }
}
