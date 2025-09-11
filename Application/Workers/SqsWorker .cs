using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using TesteTecnico.Application.DTOs;
using TesteTecnico.Domain.Entities;
using TesteTecnico.Application.Interfaces;
using System;

public class SqsWorker : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;

    public SqsWorker(IAmazonSQS sqsClient, IConfiguration config, IServiceProvider serviceProvider)
    {
        _sqsClient = sqsClient;
        _queueUrl = config["AWS:SQSQueueUrl"];
        _config = config;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 5
            }, stoppingToken);

            if (response.Messages != null && response.Messages.Count > 0)
            {
                foreach (var message in response.Messages)
                {
                    try
                    {
                        var moto = JsonSerializer.Deserialize<CreateMotorcycleDto>(message.Body);

                        if (moto.Year == 2024)
                        {

                            using var scope = _serviceProvider.CreateScope();
                            var notificationService = scope.ServiceProvider
                                .GetRequiredService<IMotorcycleNotificationService>();

                            var notification = new MotorcycleNotification
                            {
                                Plate = moto.Plate,
                                Message = $"Moto {moto.Model} ({moto.Plate}) cadastrada com sucesso em {moto.Year}."
                            };

                            await notificationService.SaveAsync(notification);
                        }

                        await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                    }
                }
            }
        }
    }
}
