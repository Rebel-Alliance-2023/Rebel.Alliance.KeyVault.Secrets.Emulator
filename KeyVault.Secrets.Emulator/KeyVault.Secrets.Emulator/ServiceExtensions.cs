using KeyVault.Secrets.Emulator.Rebel.Alliance.KeyVault.Secrets.Emulator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rebel.Alliance.KeyVault.Secrets.Emulator
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds the Azure Key Vault Emulator services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddAzureKeyVaultEmulator(this IServiceCollection services)
        {
            // Add the SecretClient to the service collection with a singleton lifetime
            services.AddSingleton<SecretClient>(provider =>
            {
                // Resolve the logger from the service provider
                var logger = provider.GetRequiredService<ILogger<SecretClient>>();
                // Create and return a new instance of SecretClient with the resolved logger
                return new SecretClient(logger);
            });

            return services;
        }
    }
}
