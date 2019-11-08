using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg002")]
    public class Eg002SigningViaEmailController : EgController
    {
        public Eg002SigningViaEmailController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Signing request by email";
        }

        public override string EgName => "eg002";

        // ***DS.snippet.0.start


        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Check the token with minimal buffer time.
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
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;

            string envelopeId = Examples.SingingViaEmail.SendEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName, accessToken, basePath, accountId, Config.docDocx, Config.docPdf, RequestItemsService.Status);
            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + envelopeId + ".";
            RequestItemsService.EnvelopeId = envelopeId;
            return View("example_done");
        }
        // ***DS.snippet.0.end
    }
}