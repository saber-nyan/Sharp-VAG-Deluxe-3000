using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sharp_VAG_Deluxe_3000;

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
        public void TestBuildUrl() {
            Assert.AreEqual("https://saber.nya.pub",
                Utils.BuildUrl("https://saber.nya.pub", new Dictionary<string, string>()));
            Assert.AreEqual("lol.com/?meh=nope", Utils.BuildUrl("lol.com/", new Dictionary<string, string> {
                {"meh", "nope"},
                {"lol", null},
                {"SHIT", ""},
                {"", ""}
            }));
            Assert.AreEqual("a.com/?a=b&c=d&e=f", Utils.BuildUrl("a.com/", new Dictionary<string, string> {
                {"a", "b"},
                {"c", "d"},
                {"e", "f"}
            }));
        }
    }
}