using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System;

[assembly: FunctionsStartup(typeof(Hollan.Function.Startup))]

namespace Hollan.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(s => s.GetService<CosmosClientFactory>().GetClient());
        }
    }

    public class CosmosClientFactory
    {
        private CosmosClient _client;

        public CosmosClientFactory(IConfiguration configuration)
        {
            
        }
        public CosmosClient GetClient()
        {
            if(_client == null)
            {
                // Could use local.settings.json value when debugging locally
                // or pull from Key Vault when running in cloud using this feature:
                // https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references
                _client = new CosmosClient(Environment.GetEnvironmentVariable("CosmosDbConnectionString"));
            }
            return _client;
        }
    }
}