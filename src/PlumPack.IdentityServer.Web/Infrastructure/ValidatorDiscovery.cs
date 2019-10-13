using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;

namespace PlumPack.IdentityServer.Web.Infrastructure
{
    public static class ValidatorDiscovery
    {
        public static IEnumerable<Validator> DiscoverValidators(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IValidator).IsAssignableFrom(type))
                {
                    continue;
                }

                var validatorInterface = FindValidatorInterface(type);

                yield return new Validator
                {
                    Interface = validatorInterface,
                    Implementation = type
                };
            }
        }

        private static Type FindValidatorInterface(Type type)
        {
            foreach (var inter in type.GetInterfaces())
            {
                if (inter.IsGenericType)
                {
                    if (inter.GetGenericTypeDefinition() == typeof(IValidator<>))
                    {
                        return inter;
                    }
                }
            }
            
            var baseType = type.BaseType;
            return baseType != null ? FindValidatorInterface(baseType) : null;
        }
        
        public class Validator
        {
            public Type Interface { get; set; }
            
            public Type Implementation { get; set; }
        }
    }
}