using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg003")]
    public class Eg003ListEnvelopesController : EgController
    {
        public Eg003ListEnvelopesController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "List envelopes";
        }
   
        public override string EgName => "eg003";

        // ***DS.snippet.0.start

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Data for this method
            string accessToken = RequestItemsService.User.AccessToken;
            string basePath = RequestItemsService.Session.BasePath + "/restapi";
            string accountId = RequestItemsService.Session.AccountId;
            
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

            // Call the worker
            EnvelopesInformation results = Examples.ListAccountEnvelopes.ListAllEnvelope(accessToken, basePath, accountId);
            // Process results
            ViewBag.h1 = "List envelopes results";
            ViewBag.message = "Results from the Envelopes::listStatusChanges method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results,Formatting.Indented);
            return View("example_done");
        }
        // ***DS.snippet.0.end
    }
}