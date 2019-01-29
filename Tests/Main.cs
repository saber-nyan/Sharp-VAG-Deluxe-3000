using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sharp_VAG_Deluxe_3000;
using Sharp_VAG_Deluxe_3000.Exceptions;
using Sharp_VAG_Deluxe_3000.Exceptions.Authorization;

namespace Tests {
    public class Tests {
        [Test]
        public void TestGetGcmReceipt() {
            var result = VkApi.GetGcmReceipt().Result;
            Assert.IsNotNull(result, "receipt is null!");
        }

        [Test]
        public void TestAuth() {
            var api = new VkApi();
            JObject credentials;
            using (var reader =
                File.OpenText($"{Environment.GetEnvironmentVariable("USERPROFILE")}\\credentials.json")) {
                credentials = (JObject) JToken.ReadFrom(new JsonTextReader(reader));
            }

            api.Authorize(new AuthorizationParams {
                Login = credentials["login"].ToString(),
                Password = credentials["password"].ToString(),
                Scope = "audio,offline"
            }).GetAwaiter().GetResult();
            Assert.IsNotEmpty(api.AccessToken, "no access token!");
        }

        [Test]
        public void TestAuthException() {
            var api = new VkApi();

            Assert.Throws<IncorrectCredentialsException>(() =>
                api.Authorize(new AuthorizationParams {
                    Login = "unknown_login",
                    Password = "wrong_password",
                    Scope = "offline"
                }).GetAwaiter().GetResult());
        }

        [Test]
        public void TestBuildUrl() {
            Assert.AreEqual("https://saber.nya.pub",
                Utils.BuildUrl("https://saber.nya.pub", new Dictionary<string, string>()));
            Assert.AreEqual("lol.com/?meh=nope&SHIT=%20", Utils.BuildUrl("lol.com/", new Dictionary<string, string> {
                {"meh", "nope"},
                {"lol", null},
                {"SHIT", " "},
                {"", ""}
            }));
            Assert.AreEqual("a.com/?a=b&c=d&e=f", Utils.BuildUrl("a.com/", new Dictionary<string, string> {
                {"a", "b"},
                {"c", "d"},
                {"e", "f"}
            }));
        }

        [Test]
        public void TestEnumCast() {
            var errorType = Utils.GetEnumObjectByValue<VkApiException.ApiErrorCodeEnum>(1);
            Assert.AreEqual(VkApiException.ApiErrorCodeEnum.UnknownError, errorType);
            var errorType2 = Utils.GetEnumObjectByValue<VkApiException.ApiErrorCodeEnum>(201);
            Assert.AreEqual(VkApiException.ApiErrorCodeEnum.AudioAccessDenied, errorType2);
            Assert.Null(Utils.GetEnumObjectByValue<VkApiException.ApiErrorCodeEnum>(2281337));
        }
    }
}
