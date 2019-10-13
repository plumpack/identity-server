using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace PlumPack.IdentityServer.Web.Infrastructure
{
    public class AutoAreaConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var areaName = GetAreaName(controller.ControllerType);
            if (string.IsNullOrEmpty(areaName))
            {
                return;
            }
            controller.RouteValues["area"] = areaName;
        }
        
        private string GetAreaName(TypeInfo controllerType)
        {
            var tokens = controllerType.FullName.Split('.');
            if (tokens.All(t => t.Equals("areas", StringComparison.CurrentCultureIgnoreCase))) return "";
            var areaName = tokens
                .SkipWhile(t => !t.Equals("areas", StringComparison.CurrentCultureIgnoreCase))
                .Skip(1)
                .Take(1)
                .FirstOrDefault();
            return areaName;
        }
    }
}