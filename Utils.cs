using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_VAG_Deluxe_3000 {
    /// <summary>
    ///     Various utilities.
    /// </summary>
    public class Utils {
        private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-";

        /// <summary>
        ///     Generates a string consisting of random characters (from <see cref="Alphabet" />)
        /// </summary>
        /// <param name="length">Length of generated string.</param>
        /// <returns>A string consisting of random characters</returns>
        public static string GenerateRandomString(int length) {
            var random = new Random();
            var sb = new StringBuilder(length);
            for (var i = 0; i < length; i++) sb.Append(Alphabet[random.Next(Alphabet.Length)]);

            return sb.ToString();
        }

        /// <summary>
        ///     Build GET query URL.
        /// </summary>
        /// <param name="baseUrl">Base URL to add params to.</param>
        /// <param name="params">Params dictionary.</param>
        /// <returns>Query URL.</returns>
        public static string BuildUrl(string baseUrl, Dictionary<string, string> @params) {
            if (@params.Count <= 0) return baseUrl;
            baseUrl += "?";
            var isFirst = true;
            foreach (var param in @params) {
                if (string.IsNullOrWhiteSpace(param.Key) || string.IsNullOrWhiteSpace(param.Value)) continue;

                if (!isFirst)
                    baseUrl += "&";
                else
                    isFirst = false;

                baseUrl += Uri.EscapeDataString(param.Key) + "=" + Uri.EscapeDataString(param.Value);
            }

            return baseUrl;
        }
    }
}