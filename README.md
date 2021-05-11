# Unindent

Removes indentation from .NET strings.

## Status

[![Build Status](https://github.com/sharpjs/Unindent/workflows/Build/badge.svg)](https://github.com/sharpjs/Unindent/actions)
[![Package Version](https://img.shields.io/nuget/v/Unindent.svg)](https://www.nuget.org/packages/Unindent)
[![Package Downloads](https://img.shields.io/nuget/dt/Unindent.svg)](https://www.nuget.org/packages/Unindent)

Coming soon.

## Installation

<!-- Install the NuGet package. -->
You can't just yet.

## Usage

This package provides an `Unindent` extension method for .NET strings.

```csharp
var result = input.Unindent();
```

The `Unindent` method returns a string with the content of the input string,
but unindented: the leading space, if any, common to all non-blank lines is
removed from each line (blank or not).

Space is any mixture of the space (`U+0020`) and tab (`U+0009`) characters.
Each tab advances to the next tab stop.  By default, tab stops are 8 spaces
apart.  The `tabStop` parameter overrides this default.

Lines end via any mixture of the carriage return (`U+000D`) and line feed
(`U+000A`) characters.  Thus Unindent supports `CR+LF` (Windows), `LF`
(Linux/Unix), and `CR` (classic MacOS) line endings.  A line is blank if it
contains no characters other than spaces or tabs.

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
