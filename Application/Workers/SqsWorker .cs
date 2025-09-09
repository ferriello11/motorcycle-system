using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text.Json;
using TesteTecnico.Application.DTOs;

public class SqsWorker : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;
    private readonly IConfiguration _config;

    public SqsWorker(IAmazonSQS sqsClient, IConfiguration config)
    {
        _sqsClient = sqsClient;
        _queueUrl = config["AWS:SQSQueueUrl"];
        _config = config;
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
                        Console.WriteLine($"Mensagem recebida: {message.Body}");
                        var moto = JsonSerializer.Deserialize<CreateMotorcycleDto>(message.Body);

                        if(moto.Year == 2024)
                        {
                             Console.WriteLine($"Moto do ano 2024 recebida: {moto.Plate}");
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
