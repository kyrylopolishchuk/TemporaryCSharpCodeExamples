using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eg_03_csharp_auth_code_grant_core.Examples
{
    public static class ListEnvelopeRecipients
    {
        /// <summary>
        /// Gets a list of all the recipients for a specific envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">The required envelopeId</param>
        /// <returns>An object containing information about all the recipients in the envelope</returns>
        public static Recipients GetRecipients(string accessToken, string basePath, string accountId, string envelopeId)
        {
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            Recipients results = envelopesApi.ListRecipients(accountId, envelopeId);
            return results;
        }
    }
}
