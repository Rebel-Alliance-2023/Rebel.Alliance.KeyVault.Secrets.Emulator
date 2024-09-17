using System;
using System.Threading.Tasks;
using Xunit;
using Serilog;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

using System.Collections.Generic;
using KeyVault.Secrets.Emulator.Rebel.Alliance.KeyVault.Secrets.Emulator;

namespace Tests.Rebel.Alliance.KeyVault.Secrets.Emulator
{
    public class SecretClientTests : IDisposable
    {
        private readonly ILogger<SecretClient> _logger;
        private readonly SecretClient _client;

        public SecretClientTests(ITestOutputHelper output)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.TestOutput(output)
                .CreateLogger();

            _logger = new SerilogLoggerFactory(logger).CreateLogger<SecretClient>();
            _client = new SecretClient(_logger);
        }

        [Fact]
        public async Task SetSecretAsync_ShouldStoreSecret()
        {
            var secretName = "TestSecret";
            var secretValue = "SecretValue";

            var secret = await _client.SetSecretAsync(secretName, secretValue);

            Assert.Equal(secretName, secret.Name);
            Assert.Equal(secretValue, secret.Value);
        }

        [Fact]
        public async Task GetSecretAsync_ShouldRetrieveSecret()
        {
            var secretName = "TestSecret";
            var secretValue = "SecretValue";

            await _client.SetSecretAsync(secretName, secretValue);
            var retrievedSecret = await _client.GetSecretAsync(secretName);

            Assert.Equal(secretName, retrievedSecret.Name);
            Assert.Equal(secretValue, retrievedSecret.Value);
        }

        [Fact]
        public async Task SetSecretAsync_WithByteArray_ShouldStoreSecret()
        {
            var secretName = "ByteArraySecret";
            byte[] data = new byte[] { 1, 2, 3, 4, 5 };
            string base64String = Convert.ToBase64String(data);

            var secret = await _client.SetSecretAsync(secretName, base64String);

            Assert.Equal(secretName, secret.Name);
            Assert.Equal(base64String, secret.Value);
        }

        [Fact]
        public async Task GetSecretAsync_WithByteArray_ShouldRetrieveAndConvertSecret()
        {
            var secretName = "ByteArraySecret";
            byte[] originalData = new byte[] { 1, 2, 3, 4, 5 };
            string base64String = Convert.ToBase64String(originalData);

            await _client.SetSecretAsync(secretName, base64String);
            var retrievedSecret = await _client.GetSecretAsync(secretName);

            Assert.Equal(secretName, retrievedSecret.Name);
            Assert.Equal(base64String, retrievedSecret.Value);

            byte[] retrievedData = Convert.FromBase64String(retrievedSecret.Value);
            Assert.Equal(originalData, retrievedData);
        }

        [Fact]
        public async Task DeleteSecretAsync_ShouldMarkSecretAsDeleted()
        {
            var secretName = "TestSecretToDelete";
            var secretValue = "SecretValueToDelete";

            await _client.SetSecretAsync(secretName, secretValue);
            await _client.DeleteSecretAsync(secretName);

            var deletedSecret = await Assert.ThrowsAsync<KeyNotFoundException>(() => _client.GetSecretAsync(secretName));
            Assert.Equal($"Secret with name '{secretName}' not found.", deletedSecret.Message);
        }

        [Fact]
        public async Task PurgeDeletedSecretAsync_ShouldRemoveSecretPermanently()
        {
            var secretName = "TestSecretToPurge";
            var secretValue = "SecretValueToPurge";

            await _client.SetSecretAsync(secretName, secretValue);
            await _client.DeleteSecretAsync(secretName);
            await _client.PurgeDeletedSecretAsync(secretName);

            var purgedSecret = await Assert.ThrowsAsync<KeyNotFoundException>(() => _client.RecoverDeletedSecretAsync(secretName));
            Assert.Equal($"Deleted secret with name '{secretName}' not found.", purgedSecret.Message);
        }

        [Fact]
        public async Task RecoverDeletedSecretAsync_ShouldRestoreDeletedSecret()
        {
            var secretName = "TestSecretToRecover";
            var secretValue = "SecretValueToRecover";

            await _client.SetSecretAsync(secretName, secretValue);
            await _client.DeleteSecretAsync(secretName);
            await _client.RecoverDeletedSecretAsync(secretName);

            var recoveredSecret = await _client.GetSecretAsync(secretName);

            Assert.Equal(secretName, recoveredSecret.Name);
            Assert.Equal(secretValue, recoveredSecret.Value);
        }

        [Fact]
        public async Task UpdateSecretPropertiesAsync_ShouldUpdateProperties()
        {
            var secretName = "TestSecretToUpdate";
            var secretValue = "SecretValueToUpdate";

            await _client.SetSecretAsync(secretName, secretValue);
            var properties = new SecretProperties
            {
                Name = secretName,
                ContentType = "text/plain",
                Tags = new Dictionary<string, string> { { "Environment", "Test" } }
            };

            await _client.UpdateSecretPropertiesAsync(properties);
            var updatedSecret = await _client.GetSecretAsync(secretName);

            Assert.Equal(secretName, updatedSecret.Properties.Name);
            Assert.Equal("text/plain", updatedSecret.Properties.ContentType);
            Assert.True(updatedSecret.Properties.Tags.ContainsKey("Environment"));
            Assert.Equal("Test", updatedSecret.Properties.Tags["Environment"]);
        }

        public void Dispose()
        {
            // Cleanup logic if needed
        }
    }
}
