using System.Collections.Generic;

namespace Unindent.Tests
{
    using static NewLineMode;

    public static class NewLineModes
    {
        public static IReadOnlyList<NewLineMode> All { get; }
            = new[] { CrLf, Cr, Lf };

        public static string GetNewLineString(this NewLineMode mode)
            => mode switch
            {
                CrLf => "\r\n",
                Cr   => "\r",
                _    => "\n",
            };
    }
}
