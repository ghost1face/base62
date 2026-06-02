using System.Text.Json.Serialization;

namespace Base62.Benchmark.Nuget;

public class NugetSearchResultRoot
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("problems")]
    public List<string> Problems { get; set; } = [];

    [JsonPropertyName("searchResult")]
    public List<NugetSourceResult> SearchResult { get; set; } = [];
}
