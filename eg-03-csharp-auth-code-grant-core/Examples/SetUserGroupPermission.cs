using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace eg_03_csharp_auth_code_grant_core.Examples
{
    public static class SetUserGroupPermission
    {
        /// <summary>
        /// Sets permission to the user group
        /// </summary>
        /// <param name="permissionProfileId">Permission profile id</param>
        /// <param name="userGroupId">User group id</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>A group information</returns>
        public static GroupInformation GetGroupInformation(string permissionProfileId, string userGroupId, string accessToken, string basePath, string accountId)
		{

            // Construct your API headers
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            var groupsApi = new GroupsApi(config);

            // Construct your request body
            var editedGroup = new Group
            {
                GroupId = userGroupId,
                PermissionProfileId = permissionProfileId
            };
            var requestBody = new GroupInformation { Groups = new List<Group> { editedGroup } };
            // Call the eSignature REST API
            return groupsApi.UpdateGroups(accountId, requestBody);
        }
    }
}
