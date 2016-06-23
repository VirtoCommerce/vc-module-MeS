using Mes.Gateway;
using System;
using System.Collections.Specialized;
using System.Globalization;
using VirtoCommerce.Domain.Payment.Model;

namespace MeS.PaymentGatewaysModule.Web.Managers
{
    public class MesPaymentMethod : PaymentMethod
    {
        public MesPaymentMethod()
            : base("Mes")
        {
        }

        private static string MeSProfileIdStoreSetting = "Mes.PaymentGateway.Credentials.ProfileId";
        private static string MeSProfileKeyStoreSetting = "Mes.PaymentGateway.Credentials.ProfileKey";

        public override PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Standard; }
        }

        public override PaymentMethodGroupType PaymentMethodGroupType
        {
            get { return PaymentMethodGroupType.BankCard; }
        }

        private string ProfileId
        {
            get
            {
                return GetSetting(MeSProfileIdStoreSetting);
            }
        }

        private string ProfileKey
        {
            get
            {
                return GetSetting(MeSProfileKeyStoreSetting);
            }
        }

        public override ProcessPaymentResult ProcessPayment(ProcessPaymentEvaluationContext context)
        {
            var retVal = new ProcessPaymentResult();

            GatewaySettings settings = new GatewaySettings();
            settings.setCredentials(ProfileId, ProfileKey)
                //.setVerbose(true)
                .setHostUrl(GatewaySettings.URL_CERT);
            Gateway gateway = new Gateway(settings);

            GatewayRequest request = new GatewayRequest(GatewayRequest.TransactionType.SALE);
            if (string.IsNullOrEmpty(context.Payment.OuterId))
            {
                // request.setCardData("4012888812348882", "1199");
                request.setCardData(context.BankCardInfo.BankCardNumber,
                    context.BankCardInfo.BankCardMonth.ToString() + context.BankCardInfo.BankCardYear);

                request.setCurrencyCode(context.Payment.Currency);

                if (context.Payment.BillingAddress != null)
                    request.setBillingAddress(context.Payment.BillingAddress.ToString(), context.Payment.BillingAddress.Zip);
            }
            else
            {
                request.setTokenData(context.Payment.OuterId, string.Empty);
            }
            request.setAmount(context.Order.Sum.ToString(CultureInfo.InvariantCulture));
            GatewayResponse response = gateway.run(request);

            var tranId = response.getTransactionId();

            var errorCode = response.getErrorCode();

            if (errorCode.Equals("000"))
            {
                retVal.OuterId = tranId;
                retVal.IsSuccess = true;
                retVal.NewPaymentStatus = PaymentStatus.Pending; //maybe
            }
            else
            {
                retVal.NewPaymentStatus = PaymentStatus.Voided;
                retVal.Error = string.Format("Mes error {0}", errorCode);
            }

            return retVal;
        }

        public override PostProcessPaymentResult PostProcessPayment(PostProcessPaymentEvaluationContext context)
        {
            return new PostProcessPaymentResult();
        }

        public override ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection context)
        {
            return new ValidatePostProcessRequestResult { IsSuccess = false };
        }

        public override VoidProcessPaymentResult VoidProcessPayment(VoidProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override CaptureProcessPaymentResult CaptureProcessPayment(CaptureProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override RefundProcessPaymentResult RefundProcessPayment(RefundProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}