using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PlumPack.IdentityServer.Web.Infrastructure
{
    public class FormValueExistsAttribute : ModelBinderAttribute
    {
        public FormValueExistsAttribute()
        {
            BinderType = typeof(Binder);
        }

        public class Binder : IModelBinder
        {
            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
                bindingContext.Result = ModelBindingResult.Success(value.Length > 0);
                return Task.CompletedTask;
            }
        }
    }
}