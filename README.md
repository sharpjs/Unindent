# Unindent

Removes indentation from .NET strings.

## Status

[![Build Status](https://github.com/sharpjs/Unindent/workflows/Build/badge.svg)](https://github.com/sharpjs/Unindent/actions)
[![Package Version](https://img.shields.io/nuget/v/Unindent.svg)](https://www.nuget.org/packages/Unindent)
[![Package Downloads](https://img.shields.io/nuget/dt/Unindent.svg)](https://www.nuget.org/packages/Unindent)

Test coverage is 100%.

Unindent is available as a [NuGet package](https://www.nuget.org/packages/Unindent).

## Usage

This package provides an `Unindent` extension method for .NET strings.

```csharp
var result = input.Unindent();
```

The `Unindent` method returns a string with the content of the input string,
but unindented: if all<sup>[1](#blanks)</sup> input lines begin with the same
amount of space (the *indentation*), that space is removed from the returned
string.

*Space* here means any mixture of the space (`U+0020`) and tab (`U+0009`)
characters.  Tabs work like in most text editors<sup>[2](#split-tabs)</sup>: a
tab advances to the next tab stop.  By default, tab stops are 8 spaces apart.
The optional `tabStop` parameter overrides the default.

Lines end via any mixture of the carriage return (`U+000D`) and line feed
(`U+000A`) characters.  Thus Unindent supports `CR+LF` (Windows), `LF`
(Linux/Unix), and `CR` (classic MacOS) line endings.

<details>
<summary>Click here for details about some edge cases.</summary>

- <sup><a id="blanks">1</a></sup> Unindent ignores *blank* lines (those
  containing only space) when discovering indentation in the input string, but
  the method still removes indentation from blank lines that have it.  See
  [this test](https://github.com/sharpjs/Unindent/blob/4bad5c2249c4e4a4a4976ede12799e0d825bca61/Unindent.Tests/StringExtensionsTests.cs#L155-L158)
  for an example.

- <sup><a id="blanks">2</a></sup> If a tab character jumps past the computed
  indentation width, that tab is replaced by space characters in order to
  preserve column alignments present in the input string.  See [this test](https://github.com/sharpjs/Unindent/blob/4bad5c2249c4e4a4a4976ede12799e0d825bca61/Unindent.Tests/StringExtensionsTests.cs#L215)
  for an example.

- If the input string ends with trailing space, Unindent removes that space.
  See [this test](https://github.com/sharpjs/Unindent/blob/4bad5c2249c4e4a4a4976ede12799e0d825bca61/Unindent.Tests/StringExtensionsTests.cs#L155-L158)
  for an example.

</details>

### Example

Given this example code:

```csharp
namespace Foo
{
    public class Bar
    {
        const string Query = @"
            -- Pretend there is a table called NaturalNumbers

            SELECT TOP 100
                Output = CASE
                    WHEN N % 15 = 0 THEN N'Fizz Buzz'
                    WHEN N %  5 = 0 THEN N'Buzz'
                    WHEN N %  3 = 0 THEN N'Fizz'
                    ELSE CONVERT(nvarchar(10), N)
                END
            FROM NaturalNumbers; -- 1, 2, 3, ...
        ";
    }
}
```

The string constant `Query` is indented by 12 spaces.  To unindent it:

```csharp
var query = Query.Unindent();
```

...which yields:

```
-- Pretend there is a table called NaturalNumbers

SELECT TOP 100
    Output = CASE
        WHEN N % 15 = 0 THEN N'Fizz Buzz'
        WHEN N %  5 = 0 THEN N'Buzz'
        WHEN N %  3 = 0 THEN N'Fizz'
        ELSE CONVERT(nvarchar(10), N)
    END
FROM NaturalNumbers; -- 1, 2, 3, ...
```
