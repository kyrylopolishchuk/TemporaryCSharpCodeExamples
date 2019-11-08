﻿using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg009")]
    public class Eg009UseTemplateController : EgController
    {
        public Eg009UseTemplateController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
        }

        public override string EgName => "eg009";

        // ***DS.snippet.0.start
        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            var templateId = RequestItemsService.TemplateId;

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

            string envelopeId = Examples.CreateAndSendEnvelopeFromTemplate.SendEnvelopeFromTemplate(signerEmail, signerName, ccEmail, ccName, accessToken, basePath, accountId, templateId);

            RequestItemsService.EnvelopeId = envelopeId;
            ViewBag.message = "The envelope has been created and sent!<br/>Envelope ID " + envelopeId + ".";
            return View("example_done");
        }
        // ***DS.snippet.0.end
    }
}