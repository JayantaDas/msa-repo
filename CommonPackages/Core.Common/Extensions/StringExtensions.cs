namespace Core.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string SubstringBetween(this string value, string first, string last)
        {
            int posA = value.IndexOf(first, System.StringComparison.Ordinal);
            int posB = value.LastIndexOf(last, System.StringComparison.Ordinal);
            if (posA == -1)
            {
                return "";
            }

            if (posB == -1)
            {
                return "";
            }

            int adjustedPosA = posA + first.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string SubstringBefore(this string value, string strValue)
        {
            int posA = value.IndexOf(strValue, System.StringComparison.Ordinal);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string SubstringAfter(this string value, string strValue)
        {
            int posA = value.LastIndexOf(strValue, System.StringComparison.Ordinal);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + strValue.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }
    }
}
