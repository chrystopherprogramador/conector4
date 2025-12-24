using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Conector4.General.Elastic_Search
{
    public class ElasticLogger
    {
        private readonly HttpClient _client;
        private readonly string _indexName;

        // Valores fixos
        private const string ElasticUrl = "http://elastic.websamos.com.br:9200";
        private const string Username = "elastic";
        private const string Password = "-NJE4DMiRZClZklirs9M";
        private const string DefaultIndex = "logs-app";

        public ElasticLogger()
        {
            _indexName = DefaultIndex;

            _client = new HttpClient
            {
                BaseAddress = new Uri(ElasticUrl)
            };

            var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        /// <summary>
        /// Grava um log no Elasticsearch
        /// </summary>
        public async Task LogAsync(string level, string message)
        {
            var logEntry = new
            {
                @timestamp = DateTime.UtcNow,
                level,
                message
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(logEntry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{_indexName}/_doc", content);
            try
            {
                response.EnsureSuccessStatusCode();
            }catch (Exception ex)
            {
                return;
            }
            
        }

        /// <summary>
        /// Lista os últimos logs do índice
        /// </summary>
        public async Task<List<LogEntry>> GetLogsByDateAsync(string date,string dateFinal, string level = null, string keyword = null, int size = 50)
        {
            try
            {
                var mustClauses = new List<object>();

                // Filtro por intervalo de tempo
                mustClauses.Add(new
                {
                    range = new
                    {
                        @timestamp = new
                        {
                            gte = $"{date}T00:00:00",
                            lte = $"{dateFinal}T23:59:59"
                        }
                    }
                });

                // Filtro por nível/status do log
                if (!string.IsNullOrEmpty(level))
                {
                    mustClauses.Add(new
                    {
                        match = new
                        {
                            level = level
                        }
                    });
                }

                // Filtro por palavra-chave na mensagem
                if (!string.IsNullOrEmpty(keyword))
                {
                    mustClauses.Add(new
                    {
                        match = new
                        {
                            message = keyword
                        }
                    });
                }

                var query = new
                {
                    size,
                    query = new
                    {
                        @bool = new
                        {
                            must = mustClauses
                        }
                    },
                    sort = new[] { new { @timestamp = new { order = "desc" } } }
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(query);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"{_indexName}/_search", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(result))
                    return new List<LogEntry>();

                var parsed = JObject.Parse(result);

                var hitsToken = parsed["hits"]?["hits"];
                if (hitsToken == null || !hitsToken.HasValues)
                    return new List<LogEntry>();

                var logs = new List<LogEntry>();
                foreach (var hit in hitsToken)
                {
                    var source = hit["_source"];
                    if (source == null)
                        continue;

                    // Captura o valor cru do @timestamp
                    var rawTimestamp = source["timestamp"]?.ToString();

                    DateTime utcDate;
                    string timestampSp = null;

                    if (DateTime.TryParse(rawTimestamp, out utcDate))
                    {
                        // Pega o timezone de São Paulo
                        var tzSp = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                        // Em Linux/Unix pode ser "America/Sao_Paulo"

                        // Converte de UTC para SP
                        var spDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, tzSp);

                        // Formata como quiser
                        timestampSp = spDate.ToString("dd/MM/yyyy HH:mm:ss");
                    }

                    // Usa timestampSp no LogEntry
                    logs.Add(new LogEntry
                    {
                        Timestamp = timestampSp,
                        Level = source["level"]?.ToString(),
                        Message = source["message"]?.ToString()
                    });

                }


                return logs;
            }
            catch
            {
                return new List<LogEntry>();
            }
        }




    }

    public class LogEntry
    {
        public string Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
    }

}
