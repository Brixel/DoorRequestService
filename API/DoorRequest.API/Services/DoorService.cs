
using DoorRequest.API.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace DoorRequest.API.Services;

public class DoorService : IDoorService
{
    private readonly string _topic;
    private readonly MqttClientOptions _options;
    private readonly ILogger<DoorService> _logger;

    public DoorService(IOptions<DoorConfiguration> options, ILogger<DoorService> logger)
    {
        _topic = options.Value.Topic;
        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithClientId(options.Value.ClientId)
            .WithTcpServer(options.Value.Server, options.Value.Port);

        if (!string.IsNullOrWhiteSpace(options.Value.Username) || !string.IsNullOrWhiteSpace(options.Value.Password))
        {
            optionsBuilder.WithCredentials(options.Value.Username, options.Value.Password);
        }

        if (options.Value.UseSSL)
        {
            optionsBuilder
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    SslProtocol = SslProtocols.Tls12,
                    UseTls = true,
                    CertificateValidationHandler = e =>
                    {
                        var cert = (X509Certificate2)e.Certificate;
                        return cert.Thumbprint == options.Value.CertificateThumbprint;
                    }
                });
        }

        _options = optionsBuilder.Build();
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    public async Task<bool> OpenDoor()
    {
        _logger.LogInformation("Sending request to open the door via MQTT");
        using var mqttClient = new MqttFactory().CreateMqttClient();
        await mqttClient.ConnectAsync(_options, CancellationToken.None);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(_topic)
            .WithPayload("1")
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();
        var result = await mqttClient.PublishAsync(message, CancellationToken.None);
        var isSuccess = result.ReasonCode == MqttClientPublishReasonCode.Success;

        return isSuccess;
    }
}