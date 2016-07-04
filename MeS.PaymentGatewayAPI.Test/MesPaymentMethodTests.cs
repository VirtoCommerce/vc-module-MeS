using Mes.Gateway;
using Xunit;

namespace MeS.PaymentGatewayAPI.Test
{
    [Trait("Category", "CI")]
    public class MeSPaymentGatewayAPITests
    {
        private static string ProfileId = "94100011811900000286",
                              ProfileKey = "blGMSGehNOZCHESLZCFtZKvqJPUzVhbE";

        [Fact]
        public void ProcessPayment_successfull()
        {
            //arrange
            GatewaySettings settings = new GatewaySettings();
            settings.setCredentials(ProfileId, ProfileKey)
                //.setVerbose(true)
                .setHostUrl(GatewaySettings.URL_CERT);
            Gateway gateway = new Gateway(settings);

            var request = CreatePayRequest();

            //act
            var response = gateway.run(request);

            //assert
            Assert.NotNull(response.getTransactionId());
            Assert.Equal("000", response.getErrorCode());
        }

        private GatewayRequest CreatePayRequest()
        {
            GatewayRequest request = new GatewayRequest(GatewayRequest.TransactionType.SALE);
            request.setCardData("4012888812348882", "1199");
            request.setCurrencyCode("USD");
            request.setAmount("154.46");

            return request;
        }
    }
}
