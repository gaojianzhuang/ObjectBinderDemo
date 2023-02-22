using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectBinderDemo
{

    #region HiddenTypeModelBinder
    public class HiddenTypeModelBinder : IModelBinder
    {
        public const string HiddenTypeFieldName = HiddenTypeModelBinderProvider.HiddenTypeFieldName;

        public HiddenTypeModelBinder(IServiceProvider serviceProvider,
                                     ModelBinderProviderContext modelBinderProviderContext)
        {
            ServiceProvider = serviceProvider;
            ModelBinderProviderContext = modelBinderProviderContext;
        }
        public IServiceProvider ServiceProvider { get; }
        public ModelBinderProviderContext ModelBinderProviderContext { get; }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var typeFieldName = string.IsNullOrEmpty(bindingContext.ModelName)
                              ? HiddenTypeFieldName
                              : $"{bindingContext.ModelName}.{HiddenTypeFieldName}";
            var typeFieldValues = bindingContext.ValueProvider.GetValue(typeFieldName);
            var modelTypeValue = typeFieldValues.Values.FirstOrDefault();
            if (!string.IsNullOrEmpty(modelTypeValue))
            {
                var modelType = Type.GetType(modelTypeValue);
                if (modelType != null)
                {
                    await TryBindModelAsync(bindingContext, modelType);
                }
            }
        }

        protected virtual async Task TryBindModelAsync(ModelBindingContext bindingContext, Type realModelType)
        {
            var context = ModelBinderProviderContext;

            var modelMetadataProvider = ServiceProvider.GetRequiredService<IModelMetadataProvider>();
            var modelMetadata = modelMetadataProvider.GetMetadataForType(realModelType);

            var propertyBinders = new Dictionary<ModelMetadata, IModelBinder>();
            for (var i = 0; i < modelMetadata.Properties.Count; i++)
            {
                var property = modelMetadata.Properties[i];
                propertyBinders.Add(property, context.CreateBinder(property));
            }

            var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            var modelBinder = new InternalComplexTypeModelBinder(
                                propertyBinders,
                                loggerFactory,
                                ServiceProvider,
                                allowValidatingTopLevelNodes: true);

            bindingContext.ModelMetadata = modelMetadata;
            await modelBinder.BindModelAsync(bindingContext);
        }
    }
    #endregion

    internal class InternalComplexTypeModelBinder : ComplexTypeModelBinder
    {
        public InternalComplexTypeModelBinder(IDictionary<ModelMetadata, IModelBinder> propertyBinders,
                                              ILoggerFactory loggerFactory,
                                              IServiceProvider serviceProvider,
                                              bool allowValidatingTopLevelNodes)
            : base(propertyBinders, loggerFactory, allowValidatingTopLevelNodes)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        protected override object CreateModel(ModelBindingContext bindingContext)
        {
            var model = ServiceProvider.GetService(bindingContext.ModelType);
            return model ?? base.CreateModel(bindingContext);
        }
    }
}
