using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Base62.Benchmark;

[Orderer(SummaryOrderPolicy.Method)]
public class Base62Benchmark
{
    private readonly Base62Converter _converter = new();
    private readonly Base62Converter _invertedConverter = new(Base62Converter.CharacterSet.INVERTED);
    private string[] _inputs = [];
    private string[] _encodedValues = [];
    private byte[] _bytesWithLeadingZeros = [];

    [GlobalSetup]
    public void GlobalSetup()
    {
        var validationPath = Path.Combine(AppContext.BaseDirectory, "validation_data.txt");
        var inputs = new List<string>();
        var encoded = new List<string>();

        foreach (var line in File.ReadLines(validationPath))
        {
            var parts = line.Split('\t');
            if (parts.Length < 2)
                continue;

            inputs.Add(parts[0]);
            encoded.Add(parts[1]);
        }

        _inputs = inputs.ToArray();
        _encodedValues = encoded.ToArray();
        _bytesWithLeadingZeros = [0, 0, 1, 2, 0, 0];
    }

    [Benchmark]
    public void EncodeString_AllValidationSamples()
    {
        foreach (var input in _inputs)
        {
            _ = _converter.Encode(input);
        }
    }

    [Benchmark]
    public void DecodeString_AllValidationSamples()
    {
        foreach (var value in _encodedValues)
        {
            _ = _converter.Decode(value);
        }
    }

    [Benchmark]
    public void EncodeDecode_RoundTrip_AllValidationSamples()
    {
        foreach (var input in _inputs)
        {
            var encoded = _converter.Encode(input);
            _ = _converter.Decode(encoded);
        }
    }

    [Benchmark]
    public void EncodeBytes_WithLeadingZeros()
    {
        _ = _converter.Encode(_bytesWithLeadingZeros);
    }

    [Benchmark]
    public void DecodeBytes_WithLeadingZeros()
    {
        var encoded = _converter.Encode(_bytesWithLeadingZeros);
        _ = _converter.Decode(encoded);
    }

    [Benchmark]
    public void EncodeString_InvertedCharacterSet()
    {
        foreach (var input in _inputs)
        {
            _ = _invertedConverter.Encode(input);
        }
    }

    [Benchmark]
    public void EncodeString_ShortAscii()
    {
        _ = _converter.Encode("120");
    }

    [Benchmark]
    public void EncodeString_Unicode()
    {
        _ = _converter.Encode("love爱");
    }
}
