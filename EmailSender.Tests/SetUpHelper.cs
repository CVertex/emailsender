using System;
using System.Xml.Linq;

namespace EmailSender.Tests
{
    public static class SetUpHelper
    {
        /// <summary>
        /// Tries to parse integer from element. If unsuccessful, will return defaultValue
        /// </summary>
        public static int ParseInt(this XElement element, int defaultValue)
        {
            int result;
            if (element==null || !Int32.TryParse(element.Value,out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Tries to parse integer from element. If unsuccessful, will throw an exception
        /// </summary>
        public static int ParseInt(this XElement element) {

            return Int32.Parse(element.Value);
        }

        /// <summary>
        /// Tries to parse integer from element. If unsuccessful, will throw an exception
        /// </summary>
        public static string ToValueOrEmpty(this XElement element) {

            if (element == null)
                return String.Empty;

            return element.Value;
        }
    }
}