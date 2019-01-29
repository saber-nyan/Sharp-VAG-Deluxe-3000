namespace Sharp_VAG_Deluxe_3000.Exceptions.Authorization {
    /// <summary>
    ///     Base VK API exception.
    /// </summary>
    public class VkBaseAuthorizationException : VkBaseException {
        public VkBaseAuthorizationException(string responseBody) : base(responseBody) { }
    }
}
