using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duckify {
    public static class Helper {
        public static string EncodeTo64(string toEncode) {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string ConvertMsToReadable(double ms) {
            TimeSpan ts = TimeSpan.FromMilliseconds(ms);
            return ts.ToString(@"mm\:ss");
        }

        public static string ConvertToString(this IEnumerable<string> items) {
            var result = "";
            foreach (var item in items) {
                result += item + ", ";
            }
            return result.Remove(result.Length - 2, 2);
        }

    }
}
