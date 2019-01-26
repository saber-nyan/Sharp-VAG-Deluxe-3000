using System;

namespace Sharp_VAG_Deluxe_3000.Exceptions.Authorization {
    /// <summary>
    ///     2FA code is required.
    /// </summary>
    public class TwoFaValidationRequiredException : VkBaseAuthorizationException {
        /// <summary>
        ///     The way user gets 2FA code.
        /// </summary>
        public enum ValidationTypeEnum {
            /// <summary>
            ///     User gets code from SMS.
            /// </summary>
            /// <code>2fa_sms</code>
            Sms,

            /// <summary>
            ///     User gets code from app.
            /// </summary>
            /// <code>2fa_app</code>
            App
        }

        /// <summary>
        ///     Constructs new "Need 2FA Validation" exception.
        /// </summary>
        /// <param name="responseBody">
        ///     <inheritdoc cref="VkBaseAuthorizationException.ResponseBody" />
        /// </param>
        /// <param name="validationType">The way user gets 2FA code.</param>
        /// <param name="phoneMask">Semi-hidden phone number.</param>
        /// <exception cref="NotImplementedException">If <paramref name="validationType" /> is incorrect.</exception>
        public TwoFaValidationRequiredException(string responseBody, string validationType, string phoneMask = null) :
            base(responseBody) {
            switch (validationType) {
                case "2fa_sms":
                    ValidationType = ValidationTypeEnum.Sms;
                    break;
                case "2fa_app":
                    ValidationType = ValidationTypeEnum.App;
                    break;
                default:
                    throw new NotImplementedException($"ValidationTypeEnum '{validationType}' is not implemented'");
            }

            PhoneMask = phoneMask;
        }

        /// <summary>
        ///     The way user gets 2FA code.
        /// </summary>
        private ValidationTypeEnum ValidationType { get; }

        /// <summary>
        ///     Semi-hidden phone number, if using <see cref="ValidationTypeEnum.Sms" />.
        /// </summary>
        private string PhoneMask { get; }

        public override string ToString() {
            return $"{base.ToString()}, {nameof(ValidationType)}: {ValidationType}, {nameof(PhoneMask)}: {PhoneMask}";
        }
    }
}