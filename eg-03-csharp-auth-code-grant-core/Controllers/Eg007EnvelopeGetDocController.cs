using System.Collections.Generic;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg007")]
    public class Eg007EnvelopeGetDocController : EgController
    {
        public Eg007EnvelopeGetDocController(DSConfiguration config, IRequestItemsService requestItemsService) 
            : base(config, requestItemsService)
        {
            ViewBag.title = "Download a document";
        }

        public override string EgName => "eg007";

        // ***DS.snippet.0.start
        [HttpPost]
        public ActionResult Create(string docSelect)
        {
            // Data for this method
            // docSelect -- argument
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = RequestItemsService.Session.BasePath + "/restapi";
            var accountId = RequestItemsService.Session.AccountId;
            var envelopeId = RequestItemsService.EnvelopeId;
            // documents data for the envelope. See example EG006
            List<Examples.EnvelopeDocItem> documents = RequestItemsService.EnvelopeDocuments.Documents;

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

            var results = Examples.GetDocumentFromEnvelope.DownloadDocument(accessToken, basePath, accountId, envelopeId, documents, docSelect);
            return File(results.Item1, results.Item2, results.Item3);
        }
        // ***DS.snippet.0.end
    }
}