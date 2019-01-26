namespace Sharp_VAG_Deluxe_3000.Exceptions.Authorization {
    /// <summary>
    ///     Captcha solution is required.
    /// </summary>
    public class CaptchaValidationRequiredException : VkBaseAuthorizationException {
        /// <summary>
        ///     Constructs "Need Captcha Validation" exception.
        /// </summary>
        /// <param name="responseBody">
        ///     <inheritdoc cref="VkBaseAuthorizationException.ResponseBody" />
        /// </param>
        /// <param name="captchaSid">Captcha ID to send with captcha solution.</param>
        /// <param name="captchaImg">Captcha image URL.</param>
        public CaptchaValidationRequiredException(string responseBody, string captchaSid, string captchaImg)
            : base(responseBody) {
            CaptchaSid = captchaSid;
            CaptchaImg = captchaImg;
        }

        /// <summary>
        ///     Captcha ID to send with captcha solution.
        /// </summary>
        private string CaptchaSid { get; }

        /// <summary>
        ///     Captcha image URL.
        /// </summary>
        private string CaptchaImg { get; }

        public override string ToString() {
            return $"{base.ToString()}, {nameof(CaptchaSid)}: {CaptchaSid}, {nameof(CaptchaImg)}: {CaptchaImg}";
        }
    }
}