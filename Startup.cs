using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;

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
        private readonly IConfigurationRoot _config;
        public CosmosClientFactory(IConfiguration config)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));

            _config = new ConfigurationBuilder()
                .AddConfiguration(config)
                .AddAzureKeyVault($"https://{config["KeyVaultName"]}.vault.azure.net/", 
                    keyVaultClient,
                    new DefaultKeyVaultSecretManager())
                .Build();
        }

        public CosmosClient GetClient()
        {
            if(_client == null)
            {
                var connectionString = _config["CosmosDbConnectionString"];
                _client = new CosmosClient(connectionString);
            }
            return _client;
        }
    }
}