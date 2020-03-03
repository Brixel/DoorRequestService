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
        private readonly string _clientId;
        private readonly string _username;
        private readonly string _password;
        private readonly string _server;
        private readonly string _topic;
        private readonly IMqttClientOptions _options;

        public BrixelOpenDoorClient(string clientId, string username, string password, string server, string topic)
        {
            _clientId = clientId;
            _username = username;
            _password = password;
            _server = server;
            _topic = topic;

            _options = new MqttClientOptionsBuilder()
                    .WithClientId(_clientId)
                    .WithCredentials(_username, _password)
                    .WithTcpServer(_server, 8883)
                    .WithTls(new MqttClientOptionsBuilderTlsParameters
                    {
                        AllowUntrustedCertificates = true,
                        SslProtocol = SslProtocols.Tls12,
                        IgnoreCertificateChainErrors = true,
                        IgnoreCertificateRevocationErrors = true,
                        UseTls = true
                    }).Build();
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