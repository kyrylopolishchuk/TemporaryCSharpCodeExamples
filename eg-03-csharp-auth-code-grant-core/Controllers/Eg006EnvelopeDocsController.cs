using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg006")]
    public class Eg006EnvelopeDocsController : EgController
    {
        public Eg006EnvelopeDocsController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "List envelope documents";
        }

        public override string EgName => "eg006";

        // ***DS.snippet.0.start
        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Data for this method
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            var envelopeId = RequestItemsService.EnvelopeId;


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

            (EnvelopeDocumentsResult results, Examples.EnvelopeDocuments envelopeDocuments) = 
                Examples.ListEnvelopeDocuments.GetDocuments(accessToken, basePath, accountId, envelopeId);

            // Save the envelopeId and its list of documents in the session so
            // they can be used in example 7 (download a document)
            RequestItemsService.EnvelopeDocuments = envelopeDocuments;
        
            ViewBag.envelopeDocuments = envelopeDocuments;
            ViewBag.h1 = "List envelope documents result";
            ViewBag.message = "Results from the EnvelopeDocuments::list method:";
            ViewBag.Locals.Json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return View("example_done");
        }
        // ***DS.snippet.0.end
    }
}