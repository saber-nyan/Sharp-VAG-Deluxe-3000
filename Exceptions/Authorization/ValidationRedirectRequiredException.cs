namespace Sharp_VAG_Deluxe_3000.Exceptions.Authorization {
    /// <summary>
    ///     External validation (via WEB-browser) required.
    /// </summary>
    public class ValidationRedirectRequiredException : VkBaseAuthorizationException {
        /// <summary>
        ///     Constructs new "Need WEB-based Validation" exception.
        /// </summary>
        /// <param name="responseBody">
        ///     <inheritdoc cref="VkBaseAuthorizationException.ResponseBody" />
        /// </param>
        /// <param name="redirectUrl">URL to redirect user to.</param>
        public ValidationRedirectRequiredException(string responseBody, string redirectUrl) : base(responseBody) {
            RedirectUrl = redirectUrl;
        }

        /// <summary>
        ///     URL to redirect user to.
        /// </summary>
        private string RedirectUrl { get; }

        public override string ToString() {
            return $"{base.ToString()}, {nameof(RedirectUrl)}: {RedirectUrl}";
        }
    }
}
