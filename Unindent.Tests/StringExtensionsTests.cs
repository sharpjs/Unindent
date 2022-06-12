/*
    Copyright 2022 Jeffrey Sharp

    Permission to use, copy, modify, and distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

namespace Unindent.Tests;

[TestFixture]
[TestFixtureSource(typeof(NewLineModes), nameof(NewLineModes.All))]
public class StringExtensionsTests
{
    private readonly string _newLine;

    public StringExtensionsTests(NewLineMode mode)
    {
        _newLine = mode.GetNewLineString();
    }

    [Test]
    public void Unindent_NullString()
    {
        Invoking(() => default(string)!.Unindent())
            .Should().ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void Unindent_ZeroTabStop()
    {
        Invoking(() => "anything".Unindent(tabStop: 0))
            .Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Test]
    public void Unindent_Empty()
    {
        AssertUnindentReturnsSame("");
    }

    [Test]
    public void Unindent_Empty_LeadingEol()
    {
        AssertUnindent(Lines("", ""), "");
    }

    [Test]
    public void Unindent_Empty_TrailingSpace()
    {
        AssertUnindent(" \t", "");
    }

    [Test]
    public void Unindent_NonEmpty_NonIndented()
    {
        AssertUnindentReturnsSame("ab");
    }

    [Test]
    public void Unindent_NonEmpty_Indented()
    {
        AssertUnindent(" \tab \t ", "ab");
    }

    [Test]
    public void Unindent_OneLine_NonIndented()
    {
        AssertUnindentReturnsSame(
            Lines(
                "ab",
                ""
            )
        );
    }

    [Test]
    public void Unindent_OneLine_Indented()
    {
        AssertUnindent(
            Lines(
                "\t ab",
                "\t  "
            ),
            Lines(
                "ab",
                ""
            )
        );
    }

    [Test]
    public void Unindent_MultiLine_NonIndented()
    {
        AssertUnindent(
            Lines(
                "",         // <-- leading EOL (will be removed)
                "",
                "ab",
                "",
                "",
                "de",
                ""
            ),
            Lines(
                "",
                "ab",
                "",
                "",
                "de",
                ""
            )
        );
    }

    [Test]
    public void Unindent_MultiLine_Indented()
    {
        AssertUnindent(
            Lines(
                "",         //  <-- leading EOL (will be removed)
                "\t ",
                "\t ab",
                "\t   de",
                "\t "
            ),
            Lines(
                "",
                "ab",
                "  de",
                ""
            )
        );
    }

    [Test]
    public void Unindent_MultiLine_Indented_EarlyEol()
    {
        AssertUnindent(
            Lines(
                "",         // <-- leading EOL (will be removed)
                "",         // <-- early EOL
                "\t   ab",
                "\t",       // <-- early EOL
                " ",        // <-- early EOL
                "\t de",
                ""
            ),
            Lines(
                "",         // <-- all space removed
                "  ab",
                "",         // <-- all space removed
                "",         // <-- all space removed
                "de",
                ""
            )
        );
    }

    [Test]
    public void Unindent_MultiLine_Indented_LateEol()
    {
        AssertUnindent(
            Lines(
                "\t   ab",
                "\t  \t ",  // <-- late EOL
                "\t de",
                "\t  \t "   // <-- late EOL
            ),
            Lines(
                "  ab",
                " \t ",     // <-- space after indent preserved
                "de",
                ""          // <-- trailing space always removed
            )
        );
    }

    [Test]
    public void Unindent_SpaceTabMix_DefaultTabStop()
    {
        AssertUnindent(
            Lines(
            //  "|~~~~~~~V-------|"
                      "\t(one)",
                     " \t (two)",
                    "  \t  (three)",
                "           (four)"
            ),
            Lines(
                "(one)",
                " (two)",
                "  (three)",
                "   (four)"
            )
        );
    }

    [Test]
    public void Unindent_SpaceTabMix_CustomTabStop()
    {
        AssertUnindent(
            tabStop: 4,
            Lines(
            //  "|~~~V---|---|"
                  "\t(one)",
                 " \t (two)",
                "  \t  (three)",
                "       (four)"
            ),
            Lines(
                "(one)",
                " (two)",
                "  (three)",
                "   (four)"
            )
        );
    }

    [Test]
    public void Unindent_SpaceTabMix_TabAcrossIndent()
    {
        AssertUnindent(
            tabStop: 4,
            Lines(
            //  "|~~~V---|---|"
                  "\t(one)",    // <-- tab jumps one space past indent
                "   (two)"
            ),
            Lines(
                " (one)",       // <-- space added to preserve alignment
                "(two)"
            )
        );
    }

    private static void AssertUnindentReturnsSame(string input)
    {
        input.Unindent().Should().BeSameAs(input);
    }

    private static void AssertUnindent(string input, string output)
    {
        input.Unindent().Should().Be(output);
    }

    private static void AssertUnindent(int tabStop, string input, string output)
    {
        input.Unindent(tabStop).Should().Be(output);
    }

    private string Lines(params string[] lines)
    {
        return string.Join(_newLine, lines);
    }
}
