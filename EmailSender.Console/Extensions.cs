using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EmailSender.Console {
    public static class Extensions {


        public static string Repeat(this char c, int charCount) {
            if (charCount <= 0)
                throw new ArgumentOutOfRangeException("charCount");

            var chars = new List<char>(charCount);
            for (var i = 0; i < charCount; i++)
                chars.Add(c);

            return new String(chars.ToArray());

        }

        public static Dictionary<string, string> ToArgumentDictionary(this string[] args) {
            var dict = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            for (int a = 1; a < args.Length; a++) {
                var argSplit = args[a].Split(new[] { '=' }, 2);
                dict.Add(argSplit[0], argSplit[1]);
            }

            return dict;
        }


        public static Func<TArg, TResult> Memoize<TArg, TResult>(this Func<TArg, TResult> f) {
            var map = new Dictionary<TArg, TResult>();
            return a => {
                TResult value;
                if (map.TryGetValue(a, out value))
                    return value;
                value = f(a);
                map.Add(a, value);
                return value;
            };
        }

        /// <summary>
        /// Throws not supported exception if type is not convertable from string
        /// </summary>

        public static object ToType(this string value, Type targetType) {
            var converter = TypeDescriptor.GetConverter(targetType);
            if (!converter.CanConvertFrom(typeof(string))) {
                throw new NotSupportedException();
            }
            return converter.ConvertFromString(value);
        }
        

    }
}
