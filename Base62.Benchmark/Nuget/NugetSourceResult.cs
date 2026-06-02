using System.Text.Json.Serialization;

namespace Base62.Benchmark.Nuget;

public class NugetSourceResult
{
    [JsonPropertyName("sourceName")]
    public required string SourceName { get; set; }

    [JsonPropertyName("packages")]
    public List<NugetPackage> Packages { get; set; } = [];
}
