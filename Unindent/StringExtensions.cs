using System;
using System.Text;

namespace Unindent
{
    using static Math;

    /// <summary>
    ///   Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///   Removes indentation from the specified string.
        /// </summary>
        /// <param name="s">
        ///   The string from which to remove indentation.
        /// </param>
        /// <param name="tabStop">
        ///   The count of columns between tab stops.  Must be a positive
        ///   number.
        /// </param>
        /// <returns>
        ///   <para>
        ///     A string with content of <paramref name="s"/>, but unindented:
        ///     the leading space, if any, common to all non-blank lines is
        ///     removed from all lines (blank or not).  If <paramref name="s"/>
        ///     ends with trailing space, this method removes that space.
        ///   </para>
        ///   <para>
        ///     <em>Space</em> is any mixture of the space (<c>U+0020</c>) and
        ///     tab (<c>U+0009</c>) characters.  Each tab advances to the next
        ///     tab stop.  Tab stops are <paramref name="tabStop"/> spaces
        ///     apart.  If a tab in <paramref name="s"/> jumps past the
        ///     indentation, this method replaces the tab with spaces to
        ///     preserve alignment.
        ///   </para>
        ///   <para>
        ///     Lines end via any mixture of the carriage return
        ///     (<c>U+000D</c>) and line feed (<c>U+000A</c>) characters.  A
        ///     line is <em>blank</em> if it contains no characters other than
        ///     spaces or tabs.
        ///   </para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="s"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="tabStop"/> is zero or negative.
        /// </exception>
        public static string Unindent(this string s, int tabStop = 8)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (tabStop < 1)
                throw new ArgumentOutOfRangeException(nameof(tabStop));

            // Ignore trailing space
            var limit = s.LengthExcludingTrailingSpace();

            // Compute indent width and count
            var (indent, count) = s.GetIndentWidthAndCount(limit, tabStop);

            // Shortcut for simple cases
            if (indent == 0 || count == 0)
                return s.Substring(0, limit);

            // Build unindented string
            return s.UnindentCore(limit, indent, tabStop);
        }

        private static (int, int) GetIndentWidthAndCount(
            this string s,
            int         limit,
            int         tabStop)
        {
            var index  = 0;             // string index
            var column = 0;             // column offset from BOL
            var indent = int.MaxValue;  // width of one indent
            var count  = 0;             // count of indents

            while (index < limit)
            {
                switch (s[index++])
                {
                    case ' ':
                        // Advance to next column
                        column++;
                        continue;

                    case '\t':
                        // Advance to next tab stop
                        column += tabStop - column % tabStop;
                        continue;

                    case '\r':
                    case '\n':
                        // Empty line => do not consider indent
                        column = 0;
                        continue;

                    default:
                        // Non-empty line => ident found
                        indent = Min(indent, column);
                        count++;

                        // Skip to next potential indent
                        index  = s.IndexOfNextBol(index, limit);
                        column = 0;
                        continue;
                }
            }

            return (indent, count);
        }

        private static string UnindentCore(
            this string s,
            int         limit,
            int         indent,
            int         tabStop)
        {
            var index  = 0;
            var result = new StringBuilder(limit);

            do
            {
                index = s.SkipIndent          (index, indent, tabStop, result);
                index = s.CopyUntilNextIndent (index, limit,           result);
            }
            while (index < limit);

            return result.ToString();
        }

        private static int SkipIndent(
            this string   s,
            int           index,
            int           indent,
            int           tabStop,
            StringBuilder result)
        {
            var column = 0; // column offset from BOL

            for (;;)
            {
                switch (s[index])
                {
                    case ' ':
                        // Consume character
                        index++;

                        // Advance to next column
                        column++;

                        // Check if more indent remains
                        if (column < indent)
                            continue;

                        // Indent has been skipped
                        return index;

                    case '\t':
                        // Consume character
                        index++;

                        // Advance to next tab stop
                        column += tabStop - column % tabStop;

                        // Check if more indent remains
                        if (column < indent)
                            continue;

                        // Add make-up spaces if tab jumped past indent
                        for (var i = column - indent; i > 0; i--)
                            result.Append(' ');

                        // Indent has been skipped
                        return index;

                    default:
                        // This must be an early EOL, due to checks elsewhere.
                        // Consider any indent as having been skipped.
                        return index;
                }
            }
        }

        private static int CopyUntilNextIndent(
            this string   s,
            int           index,
            int           limit,
            StringBuilder result)
        {
            var bol = s.IndexOfNextBol(index, limit);

            result.Append(s, index, bol - index);

            return bol;
        }

        private static int LengthExcludingTrailingSpace(this string s)
        {
            var index = s.Length;

            while (--index >= 0)
                if (!s[index].IsSpace())
                    break;

            return index + 1;
        }

        private static int IndexOfNextEol(this string s, int index, int limit)
        {
            for (; index < limit; index++)
                if (s[index].IsEol())
                    return index;

            return limit;
        }

        private static int IndexOfNextBol(this string s, int index, int limit)
        {
            index = s.IndexOfNextEol(index, limit);

            for (; index < limit; index++)
                if (!s[index].IsEol())
                    return index;

            return limit;
        }

        private static bool IsSpace(this char c)
        {
            return c == ' ' || c == '\t';
        }

        private static bool IsEol(this char c)
        {
            return c == '\r' || c == '\n';
        }
    }
}
