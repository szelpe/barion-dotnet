using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using BarionClientLibrary;
using System.Net.Http;
using System.Web.Configuration;

namespace SampleWebsite.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            var barionSettings = new BarionSettings
            {
                BaseUrl = new Uri(WebConfigurationManager.AppSettings["BarionBaseAddress"]),
                POSKey = new Guid(WebConfigurationManager.AppSettings["BarionPOSKey"]),
                Payee = WebConfigurationManager.AppSettings["BarionPayee"]
            };

            container.RegisterInstance(barionSettings);
            container.RegisterType<BarionClient>();
			
			// This is not the best way to use HttpClient, use the HttpClient factory pattern instead
			//     See: https://www.nuget.org/packages/Microsoft.Extensions.Http
            container.RegisterType<HttpClient>(new InjectionFactory(c => new HttpClient()));
        }
    }
}
