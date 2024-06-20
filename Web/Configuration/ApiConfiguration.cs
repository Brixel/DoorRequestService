using System.ComponentModel.DataAnnotations;

namespace Web.Configuration;

public class ApiConfiguration
{
    public const string SectionName = "ApiConfiguration";

    [Url, Required(AllowEmptyStrings = false)]
    public string BaseUrl { get; set; } = default!;
}
