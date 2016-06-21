using MeS.PaymentGatewaysModule.Web.Managers;
using Microsoft.Practices.Unity;
using System;
using VirtoCommerce.Domain.Payment.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace MeS.PaymentGatewaysModule.Web
{
    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        public override void Initialize()
        {
            var settings = _container.Resolve<ISettingsManager>().GetModuleSettings("MeS.PaymentGateway");

            Func<MesPaymentMethod> meSPaymentMethodFactory = () => new MesPaymentMethod
            {
                Name = "Merchant e-solutions payment gateway",
                Description = "Merchant e-solutions payment gateway integration",
                LogoUrl = "Modules/$(MeS.PaymentGateway)/Content/logo.gif",
                Settings = settings
            };

            _container.Resolve<IPaymentMethodsService>().RegisterPaymentMethod(meSPaymentMethodFactory);
        }

        #endregion
    }
}
