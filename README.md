# Base62

Base62 encoder and decoder based on [Base62 for PHP](https://github.com/tuupola/base62) for .NET.  This library is useful for converting data into shortened strings good for URL shortening and/or obfuscating auto-incrementing resource ids from being exposed through RESTful APIs.

## Compatibility

Written for `netstandard1.0` this library should be able to be used cross-platform.  See [netstandard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) details for more information.

## Usage

```
var base62Converter = new Base62Converter();

var encoded = base62Converter.Encode("120");

Console.WriteLine(encoded);

var decoded = base62Converter.Decode(encoded);

Console.WriteLine(decoded);

// output is:
//    "DWjo"
//    "120"

```

## Character sets

By default Base62 uses `[0-9A-Za-z]` character set but can be alternated to use `[0-9a-zA-Z]` through the constructor.

```
new Base62Converter(Base62Converter.CharacterSet.INVERTED);

...

```

## License

[The MIT LIcense (MIT)](./License).