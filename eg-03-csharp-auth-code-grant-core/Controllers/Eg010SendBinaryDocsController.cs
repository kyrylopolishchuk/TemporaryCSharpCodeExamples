using System.Net;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg010")]
    public class Eg010SendBinaryDocsController : EgController
    {
        public Eg010SendBinaryDocsController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "Send envelope with multipart mime";
        }

        public override string EgName => "eg010";

        // Returns a tuple. See https://stackoverflow.com/a/36436255/64904
        // ***DS.snippet.0.start
        // ***DS.snippet.0.end


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

            (bool statusOk, string envelopeId, string errorCode, string errorMessage, WebException webEx) =
                Examples.CreateEnvelopeWithMultipleDocumentTypes.CreateAndSendEnvelope(signerEmail, signerName, ccEmail, ccName, Config.docDocx, Config.docPdf, accessToken, basePath, accountId);

            if (statusOk)
            {
                RequestItemsService.EnvelopeId = envelopeId;
                ViewBag.h1 = "Envelope sent";
                ViewBag.message = "The envelope has been created and sent!<br/>Envelope ID " + envelopeId + ".";
                return View("example_done");
            }
            else
            {
                ViewBag.errorCode = errorCode;
                ViewBag.errorMessage = errorMessage;
                ViewBag.err = webEx;
                return View("error");
            }
        }
    }
}