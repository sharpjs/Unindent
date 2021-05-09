using System;
using FluentAssertions;
using NUnit.Framework;

namespace Unindent.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
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
        public void Unindent_EmptyString()
        {
            var s = string.Empty;

            s.Unindent().Should().BeSameAs(s);
        }
    }
}
