using System.Text.Json.Serialization;

namespace Base62.Benchmark.Nuget;

public class NugetPackage
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("version")]
    public required string Version { get; set; }
}
