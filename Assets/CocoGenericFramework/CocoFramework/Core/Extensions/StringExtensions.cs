using UnityEngine;
using System.Collections;
using System.Text;
using System.Globalization;

namespace TabTale {

	public static class StringExtensions
	{
		public static Encoding DefaultEncoding;

		static StringExtensions()
		{
			DefaultEncoding = Encoding.ASCII;
		}

		public static byte[] ToByteArray(this string s, Encoding encoding)
		{
			return encoding.GetBytes(s);
		}

		public static byte[] ToByteArray(this string s)
		{
			return DefaultEncoding.GetBytes(s);
		}

		public static bool IsNullOrEmpty(this string s)
		{
			return string.IsNullOrEmpty (s);
		}

	    public static string KiloFormat(this float num)
	    {
	        return Mathf.FloorToInt(num).KiloFormat();
	    }

        public static string KiloFormat(this int num)
        {
            if (num >= 1000000000000)
                return (num / 1000000000000f).ToString("0.000") + "T";
            if (num >= 10000000000)
                return (num / 1000000000f).ToString("0.0") + "G";
            if (num >= 1000000000)
                return (num / 1000000000f).ToString("0.000") + "G";
            if (num >= 10000000)
                return (num / 1000000f).ToString("0.0") + "M";
            if (num >= 1000000)
                return (num / 1000000f).ToString("0.000") + "M";
            if (num >= 10000)
                return (num / 1000f).ToString("0.0") + "K";
            if (num >= 1000)
                return (num / 1000f).ToString("0.000") + "K";

            return num.ToString("0");
        }

		/// <summary>
		/// Safe Substring, will give out put a substring of UP TO maxSize chars.
		/// </summary>
		/// <returns>The substirng.</returns>
		/// <param name="str">String.</param>
		/// <param name="maxSize">Max size.</param>
		public static string SSubstring(this string str, int maxSize)
		{
			if (str.Length <= maxSize) {
				return str;
			}
			string res = string.Format("{0}{1}",str.Substring(0,maxSize),"... TRUNCATED");
			return res;
		}


		public static string ToTitle(this string value)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
		}
	}
}
