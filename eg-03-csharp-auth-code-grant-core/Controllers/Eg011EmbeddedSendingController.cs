using System;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg011")]
    public class Eg011EmbeddedSendingController : EgController
    {
        public Eg011EmbeddedSendingController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Embedded Sending";
        }

        public override string EgName => "eg011";

        // ***DS.snippet.0.start
        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName, string startingView)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            // startingView
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            string dsReturnUrl = Config.AppUrl + "/dsReturn";

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

            string redirectUrl = Examples.EmbeddedSending.SendEnvelopeUsingEmbeddedSending(signerEmail,  signerName,  ccEmail, ccName, Config.docDocx, Config.docPdf,
                accessToken,  basePath, accountId,  startingView,  dsReturnUrl);

            Console.WriteLine("Sender view URL: " + redirectUrl);
            return Redirect(redirectUrl);
        }
        // ***DS.snippet.0.end
    }
}