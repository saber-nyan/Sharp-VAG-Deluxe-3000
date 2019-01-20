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
            api.Authorize(new AuthorizationParams {
                Login = "LGN",
                Password = "PWD", // TO!DO REMOVE!!!
                Scope = "audio,offline"
            }).GetAwaiter().GetResult();
            Assert.IsNotEmpty(api.AccessToken, "no access token!");
        }
    }
}