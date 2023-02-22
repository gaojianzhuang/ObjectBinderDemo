using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectBinderDemo
{
    public class HiddenTypeModelBinderProvider : IModelBinderProvider
    {
        public const string HiddenTypeFieldName = "___Type";
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var options = context.Services.GetService<IOptions<HiddenTypeModelBinderOptions>>();

            if (context.Metadata.BinderType == typeof(HiddenTypeModelBinder)
               || IsSupportedType(options.Value, context.Metadata.ModelType))
            {
                var propertyBinders = new Dictionary<ModelMetadata, IModelBinder>();
                for (var i = 0; i < context.Metadata.Properties.Count; i++)
                {
                    var property = context.Metadata.Properties[i];
                    propertyBinders.Add(property, context.CreateBinder(property));
                }

                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new HiddenTypeModelBinder(context.Services, context);
            }

            return null;
        }

        protected virtual bool IsSupportedType(HiddenTypeModelBinderOptions options, Type modelType)
        {
            var modelTypeGenericArgumentsCount = modelType.IsGenericType ? modelType.GetGenericArguments().Count() : -1;
            foreach (var type in options.SupportedTypes)
            {
                if (type == modelType)
                    return true;
                if (type.IsGenericType && modelType.IsGenericType 
                                       && type.GetGenericArguments().Count() == modelTypeGenericArgumentsCount)
                {
                    try
                    {
                        if (type.MakeGenericType(modelType.GetGenericArguments()) == modelType)
                            return true;
                    }
                    catch
                    {
                    }
                }
            }
            return false;
        }
    }
}
