using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
namespace BazWebApp.Handlers
{
    /// <summary>
    /// Model binding yerelleştirme sınıfı
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ModelBindingLocalizationHandler : IConfigureOptions<MvcOptions>
    {
        private readonly IServiceScopeFactory _serviceFactory;
        /// <summary>
        /// Model binding yerelleştirme konst.
        /// </summary>
        /// <param name="serviceFactory"></param>
        public ModelBindingLocalizationHandler(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        
        /// <summary>
        /// Model binding ayarlarını tetikleyen metod
        /// </summary>
        /// <param name="options"></param>
        public void Configure(MvcOptions options)
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;

                var T = (IStringLocalizerFactory)provider.GetService(typeof(IStringLocalizerFactory));
                var localizer = T.Create("ModelBindingMessages", "BazWebApp");

                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) =>
                    localizer["'{0}' must be a string with a maximum length of {1}", x, y]);

                options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor((x) =>
                    localizer["A value for the '{0}' parameter or property was not provided.", x]);

                options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() =>
                    localizer["A value is required."]);

                options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() =>
                    localizer["A non-empty request body is required."]);

                options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor((x) =>
                    localizer["Please enter value for {0}", x]);

                options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() =>
                    localizer["The supplied value is invalid."]);

                options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() =>
                    localizer["The field must be a number."]);

                options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) =>
                    localizer["The supplied value is invalid for {0}.", x]);

                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) =>
                    localizer["Please enter value for {0}", x]);

                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) =>
                    localizer["The field {0} must be a number.", x]);

                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) =>
                    localizer["Please enter value for {0}", x]);
            }
        }
    }
}
