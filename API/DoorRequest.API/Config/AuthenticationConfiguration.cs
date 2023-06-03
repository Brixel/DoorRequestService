using System.ComponentModel.DataAnnotations;

namespace DoorRequest.API.Config;

public class AuthenticationConfiguration
{
    public const string SectionName = "Authentication";

    [Required(AllowEmptyStrings = false)]
    public string Authority {get;set;}

    [Required(AllowEmptyStrings = false)]
    public string Audience {get;set;} = "door-request-api";
}