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
            AssertUnindent(
                "   ",
                ""
            );
        }

        [Test]
        public void Unindent_OneLine()
        {
            AssertUnindentReturnsSame("abc");
        }

        [Test]
        public void Unindent_OneLine_TrailingSpace()
        {
            AssertUnindent(
                "abc  ",
                "abc"
            );
        }

        [Test]
        public void Unindent_MultiLines()
        {
            AssertUnindent(
                Lines(
                    "",
                    "    abc",
                    "",
                    "   def",
                    ""
                ),
                Lines(
                    "",
                    " abc",
                    "",
                    "def",
                    ""
                )
            );
        }

        [Test]
        public void Unindent_MultiLines_EmptyLinesHaveSpace()
        {
            AssertUnindent(
                Lines(
                    "     ",
                    "    abc",
                    "    ",
                    "   def",
                    "    "
                ),
                Lines(
                    "  ",
                    " abc",
                    " ",
                    "def",
                    "" // <-- trailing space always removed
                )
            );
        }

        [Test]
        public void Unindent_Tabs_DefaultTabStop()
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
        public void Unindent_Tabs_CustomTabStop()
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
        public void Unindent_Tabs_AutoTransmuteToSpace()
        {
            AssertUnindent(
                tabStop: 4,
                Lines(
                //  "|~~~V---|---|"
                      "\t(one)",
                    "   (two)"
                ),
                Lines(
                    " (one)",
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
