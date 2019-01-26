using System;

namespace Sharp_VAG_Deluxe_3000.Exceptions {
    public class VkBaseException : ApplicationException {
        /// <summary>
        ///     Error code, see <a href="https://vk.com/dev/errors">documentation</a>.
        /// </summary>
        public enum ApiErrorCodeEnum {
            UnknownError = 1,
            AppDisabled = 2,
            UnknownMethod = 3,
            InvalidSignature = 4,
            AuthorizationFailed = 5,
            RateLimited = 6,
            InsufficientRights = 7,
            InvalidRequest = 8,
            ActionRateLimited = 9,
            ServerError = 10,
            AppMisconfigured = 11,
            CodeCompilationFailed = 12,
            CodeExecutionFailed = 13,
            CaptchaValidationRequired = 14,
            AccessDenied = 15,
            HttpsRequired = 16,
            ValidationRequired = 17,
            AccountDisabled = 18,
            DataUnavailable = 19,
            LoadingFailed = 22,
            MethodDisabled = 23,
            UserConfirmationRequired = 24,
            InvalidAppKey = 28,
            MethodRateLimited = 29,
            AccountPrivate = 30,
            InvalidParameter = 100,
            InvalidAppId = 101,
            OutOfLimits = 103,
            InvalidUserId = 113,
            InvalidServer = 118,
            InvalidParameter1 = 120,
            InvalidHash = 121,
            InvalidGroupId = 125,
            MenuAccessDenied = 148,
            InvalidTimestamp = 150,
            UserAccessDenied = 170,
            InvalidListId = 171,
            ListsFull = 173,
            CannotInteractWithYourself = 174,
            CannotAddYouBlacklisted = 175,
            CannotAddHimBlacklisted = 176,
            CannotAddUserNotFound = 177,
            AlbumAccessDenied = 200,
            AudioAccessDenied = 201,
            GroupAccessDenied = 203,
            VideoAccessDenied = 204,
            PostAccessDenied = 210,
            WallCommentsAccessDenied = 211,
            PostCommentsAccessDenied = 212,
            CommentingAccessDenied = 213,
            PostingAccessDenied = 214,
            StatusAccessDenied = 220,
            MusicBroadcastDisabled = 221,
            LinkPostingDenied = 222,
            CommentsLimitReached = 223,
            GroupListAccessDenied = 260,
            AlbumFull = 300,
            VideoAlbumFull = 302,
            TransactionForbidden = 500,
            VideoAlreadyAdded = 800,
            VideoCommentsClosed = 801,
            CannotSendHimBlacklisted = 900,
            CannotSendYouBlacklisted = 902,
            TooMuchForwards = 913,
            TooLongMessage = 914,
            ConversationAccessDenied = 917,
            ChatUserNotFound = 935
        } // TODO: move to VkApiException

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