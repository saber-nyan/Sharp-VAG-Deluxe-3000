namespace Sharp_VAG_Deluxe_3000 {
    /// <summary>
    ///     Authorization params.<br />
    ///     See <a href="https://vk.com/dev/auth_direct">official docs</a>.
    /// </summary>
    public class AuthorizationParams {
        /// <summary>
        ///     User login (e-mail, phone number).
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        ///     User password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Captcha solution (if required).
        /// </summary>
        public string CaptchaKey { get; set; }

        /// <summary>
        ///     Captcha SID (if required).
        /// </summary>
        public string CaptchaSid { get; set; }

        /// <summary>
        ///     2FA code (if required).
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Force SMS as 2FA (1 or 0).
        /// </summary>
        public string ForceSms { get; set; }

        /// <summary>
        ///     Permissions. Required at least audio,offline.<br />
        ///     See <a href="https://vk.com/dev/permissions">official docs</a>.
        /// </summary>
        public string Scope { get; set; }
    }
}