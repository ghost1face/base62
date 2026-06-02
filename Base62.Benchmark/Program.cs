using System.Diagnostics;
using System.Text.Json;
using Base62.Benchmark.Nuget;
using BenchmarkDotNet.Running;

namespace Base62.Benchmark;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var latestVersions = await GetLatestPackageVersionNumbers("Base62", topN: 1);

        _ = BenchmarkRunner.Run<Base62Benchmark>(new BenchmarkConfig(latestVersions));
    }

    private static async Task<List<string>> GetLatestPackageVersionNumbers(
        string packageName,
        int topN = 1,
        CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"package search {packageName} --format json --exact-match",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet process");

        await process.WaitForExitAsync(cancellationToken);

        var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);

        var packages = JsonSerializer.Deserialize<NugetSearchResultRoot>(output);

        var latestPackages = packages?.SearchResult
            .SelectMany(sr => sr.Packages)
            .OrderByDescending(p => Version.Parse(p.Version))
            .Take(topN);

        return latestPackages?.Select(p => p.Version).ToList() ?? [];
    }
}
