using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;


namespace eg_03_csharp_auth_code_grant_core.Examples
{
    public static class CreatePermissionProfile
    {
        /// <summary>
        /// Creates a permission profile
        /// </summary>
        /// <param name="profileName">Profile name</param>
        /// <param name="accountRoleSettings">Sccount role settings</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>A permission profile</returns>
        public static PermissionProfile Create(string profileName, AccountRoleSettingsExtension accountRoleSettings, string accessToken, string basePath, string accountId)
        {
            // Construct your API headers
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            AccountsApi accountsApi = new AccountsApi(config);           

            var newPermissionProfile = new PermissionProfile(PermissionProfileName: profileName, Settings: accountRoleSettings);

            // Call the eSignature REST API
            return accountsApi.CreatePermissionProfile(accountId, newPermissionProfile);
        }

        /// <summary>
        /// Temporary subclass for AccountRoleSettings
        /// This class is needed for now until DCM-3905 is ready
        /// </summary>
        public class AccountRoleSettingsExtension : AccountRoleSettings
        {
            [System.Runtime.Serialization.DataMember(Name = "signingUIVersion", EmitDefaultValue = false)]
            public string SigningUiVersion { get; set; }
        }
    }
}
