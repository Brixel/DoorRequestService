using System;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Extensions.ManagedClient;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace DoorRequest.API.Services
{
    public class BrixelOpenDoorClient : IBrixelOpenDoorClient
    {
        private readonly string _topic;
        private readonly IMqttClientOptions _options;

        public BrixelOpenDoorClient(string clientId, string server, string topic, int port, bool useSSL = true, string username = null, string password = null)
        {
            _topic = topic;
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(server, port);

            if (!string.IsNullOrWhiteSpace(username) || !string.IsNullOrWhiteSpace(password))
            {
                optionsBuilder.WithCredentials(username, password);
            }

            if (useSSL)
            {
                optionsBuilder
                    .WithTls(new MqttClientOptionsBuilderTlsParameters
                    {
                        AllowUntrustedCertificates = true,
                        SslProtocol = SslProtocols.Tls12,
                        IgnoreCertificateChainErrors = true,
                        IgnoreCertificateRevocationErrors = true,
                        UseTls = true
                    });
            }

            _options = optionsBuilder.Build();
        }

        public async Task<bool> OpenDoor()
        {
            using (var mqttClient = new MqttFactory().CreateMqttClient())
            {
                await mqttClient.ConnectAsync(_options, CancellationToken.None);
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(_topic)
                    .WithPayload("1")
                    .WithExactlyOnceQoS()
                    .Build();
                var result = await mqttClient.PublishAsync(message, CancellationToken.None);
                var isSuccess = result.ReasonCode == MqttClientPublishReasonCode.Success;

                return isSuccess;
            }

        }
    }
}