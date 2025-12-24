using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Conector4.General;
using Conector4.General.Elastic_Search;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Conector4.General.Rabbit
{
    public class RabbitConsumer
    {
        private readonly ElasticLogger _logger = new ElasticLogger();

        public async Task StartAsync(
            string queueName,
            Func<string, Task> processMessage,
            ushort prefetchCount = 10,
            int maxAttempts = 5)
        {
            var factory = new ConnectionFactory
            {
                HostName = "vms.websamos.com.br",
                Port = 5672,
                VirtualHost = "/",
                UserName = "ti",
                Password = "Ti@2015!*"
            };

            await _logger.LogAsync("Info", $"Starting RabbitConsumer for queue={queueName} with prefetch={prefetchCount} and maxAttempts={maxAttempts}");

            // MUDANÇA V7: Conexão e Canal são criados via Async e são IAsyncDisposable
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // MUDANÇA V7: Métodos de declaração agora são Async
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: prefetchCount,
                global: false
            );

            var consumer = new AsyncEventingBasicConsumer(channel);

            // MUDANÇA V7: O evento agora é 'ReceivedAsync'
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                Conector4.General.Metrics.IncrementInFlight(queueName);
                try
                {
                    // ea.Body agora é ReadOnlyMemory<byte>, ToArray() converte para byte[] compatível com Encoding
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    await _logger.LogAsync("Info", $"Received message on {queueName}; size={json?.Length}");

                    await processMessage(json);

                    // ✅ Processou → ACK Assíncrono
                    await channel.BasicAckAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false
                    );

                    Conector4.General.Metrics.IncrementProcessed(queueName);
                    await _logger.LogAsync("Info", $"Processed message on {queueName}");
                }
                catch (Exception ex)
                {
                    // Falha no processamento
                    Conector4.General.Metrics.IncrementFailed(queueName);
                    await _logger.LogAsync("Error", $"Processing failed on {queueName}: {ex.Message}");

                    try
                    {
                        // Lógica de Retries atualizada para v7
                        var headers = ea.BasicProperties.Headers;
                        int retries = 0;

                        // Verifica header existente com segurança
                        if (headers != null && headers.TryGetValue("x-retries", out var val))
                        {
                            try
                            {
                                if (val is byte[] b)
                                {
                                    var s = Encoding.UTF8.GetString(b);
                                    if (int.TryParse(s, out var parsed)) retries = parsed;
                                }
                                else if (val is int i) retries = i;
                                else if (val is long l) retries = (int)l;
                            }
                            catch { retries = 0; }
                        }

                        retries++;

                        // MUDANÇA V7: Preparação de propriedades e headers
                        // Devemos criar um novo dicionário clonado para não tentar modificar um ReadOnlyDictionary se houver
                        var newHeaders = new Dictionary<string, object?>(headers ?? new Dictionary<string, object?>());
                        newHeaders["x-retries"] = retries;

                        // MUDANÇA V7: Instanciação direta de BasicProperties
                        var props = new BasicProperties
                        {
                            ContentType = ea.BasicProperties.ContentType ?? "application/json",
                            DeliveryMode = DeliveryModes.Persistent, // Usa Enum
                            Headers = newHeaders
                        };

                        if (retries >= maxAttempts)
                        {
                            // --- ENVIAR PARA DLQ ---
                            var dlq = queueName + ".DLQ";

                            // MUDANÇA V7: BasicPublishAsync requer 'mandatory' (bool) antes das props
                            await channel.BasicPublishAsync(
                                exchange: string.Empty,
                                routingKey: dlq,
                                mandatory: false,
                                basicProperties: props,
                                body: ea.Body // Passa a memória diretamente
                            );

                            await _logger.LogAsync("Error", $"Message moved to DLQ {dlq} after {retries} attempts");

                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                        else
                        {
                            // --- RE-ENFILEIRAR (RETRY) ---
                            await channel.BasicPublishAsync(
                                exchange: string.Empty,
                                routingKey: queueName,
                                mandatory: false,
                                basicProperties: props,
                                body: ea.Body
                            );

                            await _logger.LogAsync("Info", $"Requeued message to {queueName} with retries={retries}");

                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                    }
                    catch (Exception ex2)
                    {
                        // Fallback: Nack se falhar ao republicar
                        await _logger.LogAsync("Error", $"Failed to republish message: {ex2.Message}");
                        try
                        {
                            await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                        }
                        catch { }
                    }
                }
                finally
                {
                    Conector4.General.Metrics.DecrementInFlight(queueName);
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );

            // Mantém a thread viva
            await Task.Delay(Timeout.Infinite);
        }
    }
}