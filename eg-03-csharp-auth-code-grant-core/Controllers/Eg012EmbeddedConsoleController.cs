﻿using System;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg012")]
    public class Eg012EmbeddedConsoleController : EgController
    {
        public Eg012EmbeddedConsoleController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "eg012";

        // ***DS.snippet.0.start
        [HttpPost]
        public IActionResult Create(string startingView)
        {
            // Data for this method
            // startingView
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            string dsReturnUrl = Config.AppUrl + "/dsReturn";
            string envelopeId = RequestItemsService.EnvelopeId;

            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation 
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after 
                // authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            string redirectUrl = Examples.ShowEmbeddedConsole.CreateEmbeddedConsoleView(accessToken, basePath,
                accountId, startingView, dsReturnUrl, envelopeId);

            Console.WriteLine("NDSE view URL: " + redirectUrl);

            return Redirect(redirectUrl);
        }
        // ***DS.snippet.0.end
    }
}