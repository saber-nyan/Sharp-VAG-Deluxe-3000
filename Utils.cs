using System;
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
    }
}