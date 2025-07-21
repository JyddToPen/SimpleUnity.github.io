using System;

namespace LearnGC.Scripts.Runtime
{
    internal static class StringExtension
    {
        public static unsafe string UnsafeToLower(this string str)
        {
            fixed (char* c = str)
            {
                int length = str.Length;
                for (int i = 0; i < length; ++i)
                {
                    c[i] = char.ToLower(c[i]);
                }
            }

            return str;
        }

        public static void ToLowerInBuffer(this string str, Span<char> buffer)
        {
            for (int i = 0; i < str.Length; i++)
            {
                buffer[i] = char.ToLowerInvariant(str[i]);
            }
        }
    }
}