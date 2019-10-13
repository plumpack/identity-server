using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace PlumPack.IdentityServer.Web.Infrastructure
{
    public class FeatureConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            controller.Properties.Add("feature", GetFeatureName(controller.ControllerType));
        }

        private string GetFeatureName(TypeInfo controllerType)
        {
            var tokens = controllerType.FullName.Split('.');
            if (tokens.All(t => t.Equals("features", StringComparison.CurrentCultureIgnoreCase))) return "";
            var featureName = tokens
                .SkipWhile(t => !t.Equals("features", StringComparison.CurrentCultureIgnoreCase))
                .Skip(1)
                .Take(1)
                .FirstOrDefault();
            return featureName;
        }
    }

    public class FeatureViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var controllerActionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null) return viewLocations;
            if(!controllerActionDescriptor.Properties.ContainsKey("feature")) return viewLocations;
            var featureName = controllerActionDescriptor.Properties["feature"] as string;
            if (string.IsNullOrEmpty(featureName)) return viewLocations;
            string controllerName = controllerActionDescriptor.ControllerName;

            return new List<string>
            {
                $"/Areas/{{2}}/Features/{featureName}/Views/{{0}}.cshtml",
                $"/Areas/{{2}}/Features/{featureName}/{controllerName}/Views/{{0}}.cshtml",
                "/Areas/{2}/Features/Shared/{0}.cshtml",
                $"/Features/{featureName}/Views/{{0}}.cshtml",
                $"/Features/{featureName}/{controllerName}/Views/{{0}}.cshtml",
                "/Features/Shared/{0}.cshtml"
            }.Union(viewLocations);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }

}