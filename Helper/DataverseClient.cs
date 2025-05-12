using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace ManagedIdentityDemo
{
    public class DataverseClient
    {
        string _token { get; set; }
        string _environment { get; set; }
        IOrganizationService _service { get; set; }
        public DataverseClient(string token, string dataverseEnvironment)
        {
            _token = token;
            _environment = dataverseEnvironment;
        }

        internal IOrganizationService getOrganizationService()
        {
            if(_service == null)
            {
                _service =  new ServiceClient(tokenProviderFunction: async (input) => { return _token; }, instanceUrl: new Uri(_environment), useUniqueInstance: true);
            }
            return _service;
        }
    }
}