using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg022")]
    public class Eg022KbaAuthController : EgController
    {
        public Eg022KbaAuthController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "Knowledge Based Authentication";
        }
        public override string EgName => "eg022";

        // ***DS.snippet.0.start
        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication.
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // Data for this method:
            // signerEmail 
            // signerName            
            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; //represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; //represents your {ACCOUNT_ID}

            string envelopeId = Examples.RecipientAuthKBA.CreateEnvelopeWithRecipientUsingKBAAuth(signerEmail, signerName, accessToken, basePath, accountId);

            ViewBag.h1 = "Envelope sent";
            ViewBag.message = "The envelope has been created and sent!<br />Envelope ID " + envelopeId + ".";
            return View("example_done");
        }
        // ***DS.snippet.0.end
    }
}