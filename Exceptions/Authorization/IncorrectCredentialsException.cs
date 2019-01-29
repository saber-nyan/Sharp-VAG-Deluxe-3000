namespace Sharp_VAG_Deluxe_3000.Exceptions.Authorization {
    /// <summary>
    ///     Login or password is incorrect.
    /// </summary>
    public class IncorrectCredentialsException : VkBaseAuthorizationException {
        /// <summary>
        ///     Constructs "Incorrect Credentials" exception.
        /// </summary>
        /// <param name="responseBody">
        ///     <inheritdoc cref="VkBaseAuthorizationException.ResponseBody" />
        /// </param>
        public IncorrectCredentialsException(string responseBody) : base(responseBody) { }
    }
}
