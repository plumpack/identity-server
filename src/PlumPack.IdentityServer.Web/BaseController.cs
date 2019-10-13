using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace PlumPack.IdentityServer.Web
{
    public class BaseController : Controller
    {
        public void AddSuccessMessage(string message)
        {
            if (!(ViewData["currentSuccessMessages"] is List<string> currentMessages))
            {
                currentMessages = new List<string>();
            }
            currentMessages.Add(message);
            ViewData["currentSuccessMessages"] = currentMessages;
        }

        public void AddFailureMessage(string message)
        {
            if (!(ViewData["currentFailureMessages"] is List<string> currentMessages))
            {
                currentMessages = new List<string>();
            }
            currentMessages.Add(message);
            ViewData["currentFailureMessages"] = currentMessages;
        }
        
        public void AddFailureMessageFromModelState()
        {
            foreach (var error in ModelState)
            {
                if (string.IsNullOrEmpty(error.Key))
                {
                    foreach (var message in error.Value.Errors)
                    {
                        AddFailureMessage(message.ErrorMessage);
                    }
                }
            }
        }
    }
}