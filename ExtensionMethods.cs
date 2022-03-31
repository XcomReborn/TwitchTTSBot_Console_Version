



namespace ExtensionMethods
{
    public static class MyExtensions
    {

        /// <summary>
        /// Generates an unsigned int by multiplying all the characters together and return the sum this will be a unique number.
        /// </summary>
        public static int GetIntFromString(this string theString)
        {

            uint sum = 1;

            foreach (char character in theString)
            {

                uint number = (uint)character;
                sum = sum * character;

            }

            return Math.Abs((int)sum);

        }


        /// <summary>
        /// Outputs the byte array as a string using 0 and 1 as bits with each byte separated by a space.
        /// <example>
        /// <code>
        /// byte[] bytes = new byte[] { 0x20, 0x20};
        /// System.Console.WriteLine(bytes.FormatByteArrayAsString());
        /// </code>
        /// </example>
        /// </summary>    
        /// <returns>
        /// 00100000 01000000
        /// </returns>

        public static string FormatByteArrayAsString(this byte[] bytes)
        {

            return string.Join(" ", bytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));

        }

        /// <summary>
        /// Reverses all the characters in the string.
        /// <example>
        /// "abcd" -> "dcba"
        /// </example>
        /// </summary>

        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


    }


public static class EnumerableExtension
{
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
}


}
