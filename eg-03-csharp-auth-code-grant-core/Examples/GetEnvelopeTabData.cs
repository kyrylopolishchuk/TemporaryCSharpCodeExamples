using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eg_03_csharp_auth_code_grant_core.Examples
{
    public static class GetEnvelopeTabData
    {
        /// <summary>
        /// Get all tab data (form data) from envelope
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="envelopeId">The required envelopeId</param>
        /// <returns>Object containing all the tab data (form data)</returns>
        public static EnvelopeFormData GetEnvelopeFormData(string accessToken, string basePath, string accountId, string envelopeId)
        {
            // Construct your API headers
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);

            // Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            return envelopesApi.GetFormData(accountId, envelopeId);
        }
    }
}
