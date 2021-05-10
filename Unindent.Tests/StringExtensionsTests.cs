using System;
using FluentAssertions;
using NUnit.Framework;

namespace Unindent.Tests
{
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
            (null as string)
                .Invoking(s => s!.Unindent())
                .Should()
                .ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Unindent_ZeroTabStop()
        {
            "anything"
                .Invoking(s => s.Unindent(0))
                .Should()
                .ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Unindent_Empty()
        {
            AssertUnindentReturnsSame("");
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
            AssertUnindentReturnsSame(
                Lines(
                    "",
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
}
