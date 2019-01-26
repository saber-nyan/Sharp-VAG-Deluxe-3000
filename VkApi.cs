﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using Sharp_VAG_Deluxe_3000.Exceptions.Authorization;

namespace Sharp_VAG_Deluxe_3000 {
    /// <summary>
    ///     Sharp VAG Deluxe 3000 core class.
    /// </summary>
    public class VkApi {
        public const string ApiVersion = "5.92";
        private const string UserAgentGcm = "Android-GCM/1.5 (generic_x86 KK)";

        private const string UserAgentGeneric =
            "KateMobileAndroid/51.2 lite-443 (Android 4.4.2; SDK 19; x86; unknown Android SDK built for x86; en)";

        private static HttpClient _http;

        /// <summary>
        ///     Creates core class instance.
        /// </summary>
        /// <param name="proxy">
        ///     (Optionally) proxy server to send all requests through.<br />
        ///     See <a href="https://stackoverflow.com/a/30605900/10018051">SO.com</a>.
        /// </param>
        public VkApi(IWebProxy proxy = null) {
            _http = proxy != null ? new HttpClient(new HttpClientHandler {Proxy = proxy}, true) : new HttpClient();
            _http.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgentGeneric);
        }

        public string AccessToken { get; private set; }

        /// <summary>
        ///     Authorizes user.
        /// </summary>
        /// <param name="authParams">Login, password and scope (at least audio+offline) are mandatory, other options are optional.</param>
        /// <returns>Nothing.</returns>
        /// <exception cref="ValidationRedirectRequiredException">If VK requires user validation via browser.</exception>
        /// <exception cref="TwoFaValidationRequiredException">
        ///     If VK requires 2FA. Invoke this method again w/
        ///     <see cref="AuthorizationParams.Code" /> parameter.
        /// </exception>
        /// <exception cref="CaptchaValidationRequiredException">
        ///     If VK requires captcha validation. Invoke this method again w/
        ///     <see cref="AuthorizationParams.CaptchaKey" /> and <see cref="AuthorizationParams.CaptchaSid" /> params.
        /// </exception>
        /// <exception cref="IncorrectCredentialsException">If login or password is incorrect.</exception>
        /// <exception cref="VkBaseAuthorizationException">If unknown exception occured.</exception>
        /// <exception cref="InvalidDataException">If server response is incorrect.</exception>
        /// <exception cref="InvalidOperationException">If GCM receipt obtainment or token refreshment failed.</exception>
        public async Task Authorize(AuthorizationParams authParams) {
            var receipt = await GetGcmReceipt();

            var authResult = await _http.GetAsync(Utils.BuildUrl("https://oauth.vk.com/token",
                new Dictionary<string, string> {
                    {"grant_type", "password"},
                    {"client_id", "2685278"},
                    {"client_secret", "lxhD8OD7dMsqtXIm5IUY"},
                    {"username", authParams.Login},
                    {"password", authParams.Password},
                    {"scope", authParams.Scope},
                    {"v", ApiVersion},
                    {"2fa_supported", "1"},
                    {"force_sms", authParams.ForceSms},
                    {"code", authParams.Code},
                    {"captcha_sid", authParams.CaptchaSid},
                    {"captcha_key", authParams.CaptchaKey}
                }));

            var responseBody = await authResult.Content.ReadAsStringAsync();
            var authResponse = JObject.Parse(responseBody);

            if (authResponse.ContainsKey("error"))
                switch (authResponse["error"]?.ToString()) {
                    case "need_validation":
                        if (authResponse.ContainsKey("redirect_uri"))
                            throw new ValidationRedirectRequiredException(responseBody,
                                authResponse["redirect_uri"].ToString());
                        else
                            throw new TwoFaValidationRequiredException(responseBody,
                                authResponse["validation_type"].ToString(),
                                authResponse["phone_mask"]?.ToString());
                    case "need_captcha":
                        throw new CaptchaValidationRequiredException(responseBody,
                            authResponse["captcha_sid"].ToString(),
                            authResponse["captcha_key"].ToString());
                    case "invalid_client":
                        switch (authResponse["error_type"].ToString()) {
                            case "username_or_password_is_incorrect":
                                throw new IncorrectCredentialsException(responseBody);
                            default:
                                throw new VkBaseAuthorizationException(responseBody);
                            // Report issue if you found any other values...
                        }
                    default:
                        throw new VkBaseAuthorizationException(responseBody);
                    // Report issue if you found any other values...
                }

            if (authResponse["user_id"] == null) throw new InvalidDataException($"user_id is null! {authResponse}");

            var nonRefreshedToken = authResponse["access_token"].ToString();
            var refreshResult = await _http.GetAsync(Utils.BuildUrl("https://api.vk.com/method/auth.refreshToken",
                new Dictionary<string, string> {
                    {"access_token", nonRefreshedToken},
                    {"receipt", receipt},
                    {"v", ApiVersion}
                }));
            refreshResult.EnsureSuccessStatusCode();

            var refreshResponse = JObject.Parse(await refreshResult.Content.ReadAsStringAsync())["response"];
            if (refreshResponse["token"] == null) throw new InvalidDataException($"token is null! {refreshResponse}");

            if (refreshResponse["token"].ToString() == nonRefreshedToken)
                throw new InvalidOperationException($"token {nonRefreshedToken} not refreshed!");

            AccessToken = refreshResponse["token"].ToString();
        }

        /// <summary>
        ///     Get receipt for authorization w/ access to audio.* methods.
        /// </summary>
        /// <returns>Receipt to refresh token with.</returns>
        /// <exception cref="InvalidOperationException">If token retrieval failed.</exception>
        public static async Task<string> GetGcmReceipt() {
            AndroidCheckinResponse protoCheckIn;
            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Post, "https://android.clients.google.com/checkin")) {
                requestMessage.Headers.UserAgent.ParseAdd(UserAgentGcm);
                requestMessage.Headers.Add("Expect", "");
                requestMessage.Headers.TryAddWithoutValidation("Content-Type", "application/x-protobuffer");

                var payload = new byte[] {
                    0x10, 0x00, 0x1a, 0x2a, 0x31, 0x2d, 0x39, 0x32, 0x39, 0x61, 0x30, 0x64, 0x63, 0x61, 0x30, 0x65,
                    0x65, 0x65, 0x35, 0x35, 0x35, 0x31, 0x33, 0x32, 0x38, 0x30, 0x31, 0x37, 0x31, 0x61, 0x38, 0x35,
                    0x38, 0x35, 0x64, 0x61, 0x37, 0x64, 0x63, 0x64, 0x33, 0x37, 0x30, 0x30, 0x66, 0x38, 0x22, 0xe3,
                    0x01, 0x0a, 0xbf, 0x01, 0x0a, 0x45, 0x67, 0x65, 0x6e, 0x65, 0x72, 0x69, 0x63, 0x5f, 0x78, 0x38,
                    0x36, 0x2f, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x5f, 0x73, 0x64, 0x6b, 0x5f, 0x78, 0x38, 0x36,
                    0x2f, 0x67, 0x65, 0x6e, 0x65, 0x72, 0x69, 0x63, 0x5f, 0x78, 0x38, 0x36, 0x3a, 0x34, 0x2e, 0x34,
                    0x2e, 0x32, 0x2f, 0x4b, 0x4b, 0x2f, 0x33, 0x30, 0x37, 0x39, 0x31, 0x38, 0x33, 0x3a, 0x65, 0x6e,
                    0x67, 0x2f, 0x74, 0x65, 0x73, 0x74, 0x2d, 0x6b, 0x65, 0x79, 0x73, 0x12, 0x06, 0x72, 0x61, 0x6e,
                    0x63, 0x68, 0x75, 0x1a, 0x0b, 0x67, 0x65, 0x6e, 0x65, 0x72, 0x69, 0x63, 0x5f, 0x78, 0x38, 0x36,
                    0x2a, 0x07, 0x75, 0x6e, 0x6b, 0x6e, 0x6f, 0x77, 0x6e, 0x32, 0x0e, 0x61, 0x6e, 0x64, 0x72, 0x6f,
                    0x69, 0x64, 0x2d, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x40, 0x85, 0xb5, 0x86, 0x06, 0x4a, 0x0b,
                    0x67, 0x65, 0x6e, 0x65, 0x72, 0x69, 0x63, 0x5f, 0x78, 0x38, 0x36, 0x50, 0x13, 0x5a, 0x19, 0x41,
                    0x6e, 0x64, 0x72, 0x6f, 0x69, 0x64, 0x20, 0x53, 0x44, 0x4b, 0x20, 0x62, 0x75, 0x69, 0x6c, 0x74,
                    0x20, 0x66, 0x6f, 0x72, 0x20, 0x78, 0x38, 0x36, 0x62, 0x07, 0x75, 0x6e, 0x6b, 0x6e, 0x6f, 0x77,
                    0x6e, 0x6a, 0x0e, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x5f, 0x73, 0x64, 0x6b, 0x5f, 0x78, 0x38,
                    0x36, 0x70, 0x00, 0x10, 0x00, 0x32, 0x06, 0x33, 0x31, 0x30, 0x32, 0x36, 0x30, 0x3a, 0x06, 0x33,
                    0x31, 0x30, 0x32, 0x36, 0x30, 0x42, 0x0b, 0x6d, 0x6f, 0x62, 0x69, 0x6c, 0x65, 0x3a, 0x4c, 0x54,
                    0x45, 0x3a, 0x48, 0x00, 0x32, 0x05, 0x65, 0x6e, 0x5f, 0x55, 0x53, 0x38, 0xf0, 0xb4, 0xdf, 0xa6,
                    0xb9, 0x9a, 0xb8, 0x83, 0x8e, 0x01, 0x52, 0x0f, 0x33, 0x35, 0x38, 0x32, 0x34, 0x30, 0x30, 0x35,
                    0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x30, 0x5a, 0x00, 0x62, 0x10, 0x41, 0x6d, 0x65, 0x72, 0x69,
                    0x63, 0x61, 0x2f, 0x4e, 0x65, 0x77, 0x5f, 0x59, 0x6f, 0x72, 0x6b, 0x70, 0x03, 0x7a, 0x1c, 0x37,
                    0x31, 0x51, 0x36, 0x52, 0x6e, 0x32, 0x44, 0x44, 0x5a, 0x6c, 0x31, 0x7a, 0x50, 0x44, 0x56, 0x61,
                    0x61, 0x65, 0x45, 0x48, 0x49, 0x74, 0x64, 0x2b, 0x59, 0x67, 0x3d, 0xa0, 0x01, 0x00, 0xb0, 0x01, 0x00
                };
                requestMessage.Content = new ByteArrayContent(payload);
                var result = await _http.SendAsync(requestMessage);
                result.EnsureSuccessStatusCode();
                protoCheckIn =
                    Serializer.Deserialize<AndroidCheckinResponse>(result.Content.ReadAsStreamAsync().Result);
            }

            using (var tcp = new TcpClient("mtalk.google.com", 5228)) {
                // Body data (protobuf, but i didn't found any .proto files, so...)
                byte[] message1 = {
                    0x0a, 0x0a, 0x61, 0x6e, 0x64, 0x72, 0x6f, 0x69, 0x64, 0x2d, 0x31, 0x39, 0x12, 0x0f, 0x6d, 0x63,
                    0x73, 0x2e, 0x61, 0x6e, 0x64, 0x72, 0x6f, 0x69, 0x64, 0x2e, 0x63, 0x6f, 0x6d, 0x1a
                };
                byte[] message2 = {0x22};
                byte[] message3 = {0x2a};
                byte[] message4 = {0x32};
                byte[] message5 = {
                    0x42, 0x0b, 0x0a, 0x06, 0x6e, 0x65, 0x77, 0x5f, 0x76, 0x63, 0x12, 0x01, 0x31, 0x60, 0x00, 0x70,
                    0x01, 0x80, 0x01, 0x02, 0x88, 0x01, 0x01
                };
                byte[] message6 = {0x29, 0x02};

                var idStringBytes = Encoding.ASCII.GetBytes(protoCheckIn.AndroidId.ToString());
                var idLen = VarInt.Write(idStringBytes.Length).ToList();

                var tokenStringBytes = Encoding.ASCII.GetBytes(protoCheckIn.SecurityToken.ToString());
                var tokenLen = VarInt.Write(tokenStringBytes.Length).ToList();

                var hexId = "android-" + protoCheckIn.AndroidId;
                var hexIdBytes = Encoding.ASCII.GetBytes(hexId);
                var hexIdLen = VarInt.Write(hexIdBytes.Length);

                var body = message1
                    .Concat(idLen)
                    .Concat(idStringBytes)
                    .Concat(message2)
                    .Concat(idLen)
                    .Concat(idStringBytes)
                    .Concat(message3)
                    .Concat(tokenLen)
                    .Concat(tokenStringBytes)
                    .Concat(message4)
                    .Concat(hexIdLen)
                    .Concat(hexIdBytes)
                    .Concat(message5)
                    .ToList();
                var bodyLen = VarInt.Write(body.Count);

                var payload = message6
                    .Concat(bodyLen)
                    .Concat(body)
                    .ToArray();
                //

                using (var ssl = new SslStream(tcp.GetStream(), false,
                    (sender, certificate, chain, errors) => errors == SslPolicyErrors.None)) {
                    ssl.AuthenticateAsClient("mtalk.google.com");
                    ssl.Write(payload);
                    ssl.Flush();
                    ssl.ReadByte(); // skip byte
                    var responseCode = ssl.ReadByte();
                    if (responseCode != 3) // success if second byte == 3
                        throw new InvalidOperationException($"MTalk failed, expected 3, got {responseCode}");
                }
            }

            var appid = Utils.GenerateRandomString(11);

            string receipt;

            using (var requestMessage1 =
                new HttpRequestMessage(HttpMethod.Post, "https://android.clients.google.com/c2dm/register3")) {
                requestMessage1.Headers.UserAgent.ParseAdd(UserAgentGcm);
                requestMessage1.Headers.TryAddWithoutValidation("Authorization",
                    $"AidLogin {protoCheckIn.AndroidId}:{protoCheckIn.SecurityToken}");

                var param = new Dictionary<string, string> {
                    {"X-scope", "GCM"},
                    {"X-osv", "23"},
                    {"X-subtype", "54740537194"},
                    {"X-app_ver", "443"},
                    {"X-kid", "|ID|1|"},
                    {"X-appid", appid},
                    {"X-gmsv", "13283005"},
                    {"X-cliv", "iid-10084000"},
                    {"X-app_ver_name", "51.2 lite"},
                    {"X-X-kid", "|ID|1|"},
                    {"X-subscription", "54740537194"},
                    {"X-X-subscription", "54740537194"},
                    {"X-X-subtype", "54740537194"},
                    {"app", "com.perm.kate_new_6"},
                    {"sender", "54740537194"},
                    {"device", Convert.ToString(protoCheckIn.AndroidId)},
                    {"cert", "966882ba564c2619d55d0a9afd4327a38c327456"},
                    {"app_ver", "443"},
                    {"info", "g57d5w1C4CcRUO6eTSP7b7VoT8yTYhY"},
                    {"gcm_ver", "13283005"},
                    {"plat", "0"},
                    {"X-messenger2", "1"}
                };

                requestMessage1.Content = new FormUrlEncodedContent(param);
                var result1 = await _http.SendAsync(requestMessage1);
                result1.EnsureSuccessStatusCode();
                var body1 = await result1.Content.ReadAsStringAsync();
                if (body1.Contains("Error"))
                    throw new InvalidOperationException($"C2DM registration #1 error ({body1})");

                param["X-scope"] = "id";
                param["X-kid"] = "|ID|2|";
                param["X-X-kid"] = "|ID|2|";

                using (var requestMessage2 = new HttpRequestMessage(HttpMethod.Post,
                    "https://android.clients.google.com/c2dm/register3")) {
                    requestMessage2.Headers.UserAgent.ParseAdd(UserAgentGcm);
                    requestMessage2.Headers.TryAddWithoutValidation("Authorization",
                        $"AidLogin {protoCheckIn.AndroidId}:{protoCheckIn.SecurityToken}");
                    requestMessage2.Content = new FormUrlEncodedContent(param);
                    var result2 = await _http.SendAsync(requestMessage2);

                    result2.EnsureSuccessStatusCode();
                    var body2 = await result2.Content.ReadAsStringAsync();
                    if (body2.Contains("Error"))
                        throw new InvalidOperationException($"C2DM registration #2 error ({body2})");

                    receipt = body2.Substring(13);
                }
            }

            return receipt;
        }
    }
}