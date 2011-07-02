using System;
using System.Text;

namespace Assman
{
    public static class StringExtensions
    {
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            var sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public static bool TryConvertTo<TEnum>(this string str, out TEnum value)
        {
            try
            {
                value = (TEnum) Enum.Parse(typeof (TEnum), str, ignoreCase : true);
                return true;
            }
            catch (Exception)
            {
                value = default(TEnum);
                return false;
            }
        }
    }
}