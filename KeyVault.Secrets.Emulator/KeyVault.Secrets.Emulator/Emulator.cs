namespace KeyVault.Secrets.Emulator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    namespace Rebel.Alliance.KeyVault.Secrets.Emulator
    {
        public class KeyVaultSecret
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public SecretProperties Properties { get; set; }

            public KeyVaultSecret(string name, string value)
            {
                Name = name;
                Value = value;
                Properties = new SecretProperties { Name = name };
            }
        }

        public class SecretProperties
        {
            public string Name { get; set; }
            public bool Enabled { get; set; } = true;
            public DateTimeOffset? NotBefore { get; set; }
            public DateTimeOffset? ExpiresOn { get; set; }
            public string ContentType { get; set; }
            public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
            public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
            public DateTimeOffset? UpdatedOn { get; set; }
            public bool Deleted { get; set; } = false;
        }

        public class SecretClient
        {
            private readonly Dictionary<string, KeyVaultSecret> _secrets = new();
            private readonly List<KeyVaultSecret> _deletedSecrets = new();
            private readonly ILogger<SecretClient> _logger;

            public SecretClient(ILogger<SecretClient> logger)
            {
                _logger = logger;
            }

            public async Task<KeyVaultSecret> GetSecretAsync(string name)
            {
                _logger.LogInformation("Getting secret: {SecretName}", name);
                if (_secrets.TryGetValue(name, out var secret))
                {
                    return await Task.FromResult(secret);
                }
                throw new KeyNotFoundException($"Secret with name '{name}' not found.");
            }

            public async Task<KeyVaultSecret> SetSecretAsync(string name, string value)
            {
                _logger.LogInformation("Setting secret: {SecretName}", name);
                var secret = new KeyVaultSecret(name, value);
                _secrets[name] = secret;
                return await Task.FromResult(secret);
            }

            public async Task DeleteSecretAsync(string name)
            {
                _logger.LogInformation("Deleting secret: {SecretName}", name);
                if (_secrets.TryGetValue(name, out var secret))
                {
                    _secrets.Remove(name);
                    secret.Properties.Deleted = true;
                    _deletedSecrets.Add(secret);
                    await Task.CompletedTask;
                }
                else
                {
                    throw new KeyNotFoundException($"Secret with name '{name}' not found.");
                }
            }

            public async Task PurgeDeletedSecretAsync(string name)
            {
                _logger.LogInformation("Purging deleted secret: {SecretName}", name);
                var secret = _deletedSecrets.FirstOrDefault(s => s.Name == name);
                if (secret != null)
                {
                    _deletedSecrets.Remove(secret);
                    await Task.CompletedTask;
                }
                else
                {
                    throw new KeyNotFoundException($"Deleted secret with name '{name}' not found.");
                }
            }

            public async Task RecoverDeletedSecretAsync(string name)
            {
                _logger.LogInformation("Recovering deleted secret: {SecretName}", name);
                var secret = _deletedSecrets.FirstOrDefault(s => s.Name == name);
                if (secret != null)
                {
                    secret.Properties.Deleted = false;
                    _secrets[name] = secret;
                    _deletedSecrets.Remove(secret);
                    await Task.CompletedTask;
                }
                else
                {
                    throw new KeyNotFoundException($"Deleted secret with name '{name}' not found.");
                }
            }

            public async Task UpdateSecretPropertiesAsync(SecretProperties properties)
            {
                _logger.LogInformation("Updating secret properties: {SecretName}", properties.Name);
                if (_secrets.TryGetValue(properties.Name, out var secret))
                {
                    secret.Properties = properties;
                    secret.Properties.UpdatedOn = DateTimeOffset.UtcNow;
                    await Task.CompletedTask;
                }
                else
                {
                    throw new KeyNotFoundException($"Secret with name '{properties.Name}' not found.");
                }
            }

            public async IAsyncEnumerable<SecretProperties> ListPropertiesOfSecretsAsync()
            {
                _logger.LogInformation("Listing all secret properties.");
                foreach (var secret in _secrets.Values)
                {
                    yield return await Task.FromResult(secret.Properties);
                }
            }

            public async IAsyncEnumerable<KeyVaultSecret> ListDeletedSecretsAsync()
            {
                _logger.LogInformation("Listing all deleted secrets.");
                foreach (var secret in _deletedSecrets)
                {
                    yield return await Task.FromResult(secret);
                }
            }
        }
    }

}
