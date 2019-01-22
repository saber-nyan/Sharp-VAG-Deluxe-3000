using System;

namespace Sharp_VAG_Deluxe_3000.Exceptions {
    /// <summary>
    ///     Base VK API exception.
    /// </summary>
    public class VkBaseException : ApplicationException {
        /// <summary>
        ///     Constructs API exception with response body.
        /// </summary>
        /// <param name="responseBody">Response body.</param>
        public VkBaseException(string responseBody) {
            ResponseBody = responseBody;
        }

        /// <summary>
        ///     Response body.
        /// </summary>
        private string ResponseBody { get; }

        public override string ToString() {
            return $"{base.ToString()}, {nameof(ResponseBody)}: {ResponseBody}";
        }
    }
}