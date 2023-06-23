using System.ComponentModel.DataAnnotations;

namespace DoorRequest.API.Config;

public class DoorConfiguration
{
    public const string SectionName = "MQTTDoorConfiguration";

    [Required(AllowEmptyStrings = false)]
    public string ClientId { get; set; } = "DoorRequestApi";
    public string Username { get; set; }
    public string Password { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Server { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Topic { get; set; }

    [Range(1, 65535)]
    public int Port { get; set; }
    public bool UseSSL { get; set; }
    public string CertificateThumbprint { get; set; }
}