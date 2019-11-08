using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg016")]
    public class Eg016SetTabValuesController : EgController
    {
        // Set up the Ping Url, signer client ID, and the return (callback) URL for embedded signing
        private string dsPingUrl;
        private readonly string signerClientId = "1000";
        private string dsReturnUrl;

        public Eg016SetTabValuesController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "SetTabValues";
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";
        }
        public override string EgName => "eg016";

        // ***DS.snippet.0.start
        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName)
        {
            // Check the token with minimal buffer time
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // The envelope will be sent first to the signer; after it is signed,
            // a copy is sent to the cc person
            //
            // Read files from a local directory
            // The reads could raise an exception if the file is not available!
            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1: Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            var results = Examples.SetEnvelopeTabValue.CreateEnvelopeAndUpdateTabData(signerEmail, signerName, signerClientId, accessToken, basePath, accountId, Config.tabsDocx, dsReturnUrl, dsPingUrl);
            RequestItemsService.EnvelopeId = results.Item1;
            return Redirect(results.Item2);
        }
        // ***DS.snippet.0.end
    }
}