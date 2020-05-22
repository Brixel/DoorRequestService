namespace DoorRequest.API.Config
{
    public class DoorConfiguration
    {
        public string ClientId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Topic { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
    }
}