
namespace KeyVault.Secrets.Emulator.Rebel.Alliance.KeyVault.Secrets.Emulator
{
    public interface ISecretClient
    {
        Task DeleteSecretAsync(string name);
        Task<KeyVaultSecret> GetSecretAsync(string name);
        IAsyncEnumerable<KeyVaultSecret> ListDeletedSecretsAsync();
        IAsyncEnumerable<SecretProperties> ListPropertiesOfSecretsAsync();
        Task PurgeDeletedSecretAsync(string name);
        Task RecoverDeletedSecretAsync(string name);
        Task<KeyVaultSecret> SetSecretAsync(string name, string value);
        Task UpdateSecretPropertiesAsync(SecretProperties properties);
    }
}