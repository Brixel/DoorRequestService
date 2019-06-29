using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Extensions.ManagedClient;

namespace DoorRequest.API.Services
{
    public class BrixelOpenDoorClient : IBrixelOpenDoorClient
    {
        private readonly string _clientId;
        private readonly string _username;
        private readonly string _password;
        private readonly string _server;
        private readonly string _topic;

        public BrixelOpenDoorClient(string clientId, string username, string password, string server, string topic)
        {
            _clientId = clientId;
            _username = username;
            _password = password;
            _server = server;
            _topic = topic;
        }
        public async Task<bool> OpenDoor()
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(_clientId)
                    .WithCredentials(_username, _password)
                    .WithTcpServer(_server, 8883).WithTls(new MqttClientOptionsBuilderTlsParameters()
                    {
                        AllowUntrustedCertificates = true,
                        SslProtocol = SslProtocols.Tls12,
                        IgnoreCertificateChainErrors = true,
                        IgnoreCertificateRevocationErrors = true,
                        UseTls = true
                    })
                    .Build())
                .Build();
            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            await mqttClient.StartAsync(options);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_topic)
                .WithPayload("1")
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
            var result = await mqttClient.PublishAsync(message);
            if (result.ReasonCode == MqttClientPublishReasonCode.Success)
            {
                return true;
            }

            return false;
        }
    }
}