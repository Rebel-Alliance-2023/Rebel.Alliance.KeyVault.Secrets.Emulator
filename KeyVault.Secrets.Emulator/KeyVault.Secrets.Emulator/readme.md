Here's a detailed Markdown documentation file for your Azure Key Vault emulator library. This document covers every class, property, and method, providing a thorough overview of the library.

### Azure Key Vault Emulator Library Documentation

# Rebel.Alliance.KeyVault.Secrets.Emulator

## Overview

The **Azure Key Vault Emulator Library** is designed to simulate the Azure Key Vault Secrets API in a local development environment. This library allows developers to perform typical operations with secrets, such as storing, retrieving, deleting, and recovering secrets. It provides an easy way to test and develop applications that rely on Azure Key Vault without connecting to the actual service.

## Classes

### 1. `KeyVaultSecret`

Represents a secret stored in the Key Vault.

#### Properties

- **`Name` (string)**:  
  The name of the secret.  
  *Example*: `"MySecretName"`

- **`Value` (string)**:  
  The value of the secret, which could be any sensitive information, including base64-encoded binary data.  
  *Example*: `"SecretValue"`

- **`Properties` (SecretProperties)**:  
  An instance of `SecretProperties` containing metadata associated with the secret.  
  *Example*: `new SecretProperties { Name = "MySecretName", Enabled = true }`

#### Constructor

- **`KeyVaultSecret(string name, string value)`**:  
  Initializes a new instance of the `KeyVaultSecret` class with the specified name and value.  
  *Parameters*:
  - `name` (string): The name of the secret.
  - `value` (string): The value of the secret.

### 2. `SecretProperties`

Defines the properties associated with a secret in the Key Vault.

#### Properties

- **`Name` (string)**:  
  The name of the secret.  
  *Example*: `"MySecretName"`

- **`Enabled` (bool)**:  
  Indicates whether the secret is enabled. Defaults to `true`.  
  *Example*: `true`

- **`NotBefore` (DateTimeOffset?)**:  
  The earliest date and time when the secret is valid. If `null`, the secret has no restriction on the start time.  
  *Example*: `new DateTimeOffset(new DateTime(2024, 1, 1))`

- **`ExpiresOn` (DateTimeOffset?)**:  
  The date and time when the secret expires and becomes invalid. If `null`, the secret does not have an expiration date.  
  *Example*: `new DateTimeOffset(new DateTime(2025, 1, 1))`

- **`ContentType` (string)**:  
  The content type of the secret, indicating its format or type.  
  *Example*: `"text/plain"`

- **`Tags` (IDictionary<string, string>)**:  
  A collection of key-value pairs associated with the secret for additional metadata.  
  *Example*: `new Dictionary<string, string> { { "Environment", "Production" } }`

- **`CreatedOn` (DateTimeOffset)**:  
  The timestamp representing when the secret was created. Automatically set to the current UTC time when initialized.  
  *Example*: `DateTimeOffset.UtcNow`

- **`UpdatedOn` (DateTimeOffset?)**:  
  The timestamp indicating the last time the secret's properties were updated.  
  *Example*: `DateTimeOffset.UtcNow`

- **`Deleted` (bool)**:  
  Indicates whether the secret has been marked as deleted. Defaults to `false`.  
  *Example*: `false`

### 3. `SecretClient`

Main client class for interacting with the secrets in the Key Vault emulator. It manages operations such as storing, retrieving, deleting, and recovering secrets.

#### Private Fields

- **`_secrets` (Dictionary<string, KeyVaultSecret>)**:  
  A collection that stores all secrets by their name.

- **`_deletedSecrets` (List<KeyVaultSecret>)**:  
  A list that stores secrets marked as deleted.

- **`_logger` (ILogger<SecretClient>)**:  
  The logger instance for logging operations.

#### Constructor

- **`SecretClient(ILogger<SecretClient> logger)`**:  
  Initializes a new instance of the `SecretClient` class with the specified logger.  
  *Parameters*:
  - `logger` (ILogger<SecretClient>): The logger used to log operations.

#### Methods

- **`Task<KeyVaultSecret> GetSecretAsync(string name)`**:  
  Retrieves a secret by its name.  
  *Parameters*:
  - `name` (string): The name of the secret to retrieve.  
  *Returns*: The secret associated with the specified name.  
  *Throws*: `KeyNotFoundException` if the secret does not exist.

- **`Task<KeyVaultSecret> SetSecretAsync(string name, string value)`**:  
  Stores or updates a secret.  
  *Parameters*:
  - `name` (string): The name of the secret.
  - `value` (string): The value of the secret.  
  *Returns*: The stored or updated secret.

- **`Task DeleteSecretAsync(string name)`**:  
  Marks a secret as deleted by its name.  
  *Parameters*:
  - `name` (string): The name of the secret to delete.  
  *Throws*: `KeyNotFoundException` if the secret does not exist.

- **`Task PurgeDeletedSecretAsync(string name)`**:  
  Permanently removes a previously deleted secret.  
  *Parameters*:
  - `name` (string): The name of the secret to purge.  
  *Throws*: `KeyNotFoundException` if the deleted secret does not exist.

- **`Task RecoverDeletedSecretAsync(string name)`**:  
  Recovers a deleted secret by its name.  
  *Parameters*:
  - `name` (string): The name of the secret to recover.  
  *Throws*: `KeyNotFoundException` if the deleted secret does not exist.

- **`Task UpdateSecretPropertiesAsync(SecretProperties properties)`**:  
  Updates the properties of an existing secret.  
  *Parameters*:
  - `properties` (SecretProperties): The properties to update.  
  *Throws*: `KeyNotFoundException` if the secret does not exist.

- **`IAsyncEnumerable<SecretProperties> ListPropertiesOfSecretsAsync()`**:  
  Lists all properties of secrets in the Key Vault.  
  *Returns*: An asynchronous enumerable of `SecretProperties`.

- **`IAsyncEnumerable<KeyVaultSecret> ListDeletedSecretsAsync()`**:  
  Lists all secrets that have been marked as deleted.  
  *Returns*: An asynchronous enumerable of deleted `KeyVaultSecret` instances.

## Usage Examples

### 1. Storing and Retrieving a Secret

```csharp
var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var secretClient = new SecretClient(new SerilogLoggerFactory(logger).CreateLogger<SecretClient>());

await secretClient.SetSecretAsync("MySecret", "SuperSecretValue");

var secret = await secretClient.GetSecretAsync("MySecret");
Console.WriteLine($"Secret '{secret.Name}' retrieved with value '{secret.Value}'");
```

### 2. Deleting and Recovering a Secret

```csharp
await secretClient.SetSecretAsync("MySecret", "SuperSecretValue");
await secretClient.DeleteSecretAsync("MySecret");

try
{
    var deletedSecret = await secretClient.GetSecretAsync("MySecret");
}
catch (KeyNotFoundException ex)
{
    Console.WriteLine(ex.Message); // Expected, since the secret is deleted
}

await secretClient.RecoverDeletedSecretAsync("MySecret");
var recoveredSecret = await secretClient.GetSecretAsync("MySecret");
Console.WriteLine($"Recovered Secret '{recoveredSecret.Name}' with value '{recoveredSecret.Value}'");
```

### 3. Listing Secrets by Properties

```csharp
await foreach (var secretProperties in secretClient.ListPropertiesOfSecretsAsync())
{
    Console.WriteLine($"Secret: {secretProperties.Name}, Enabled: {secretProperties.Enabled}, Tags: {string.Join(", ", secretProperties.Tags)}");
}
```

## Conclusion

This Azure Key Vault emulator library provides a simple yet powerful way to simulate secret management in a local environment, closely mimicking the behavior of Azure Key Vault. Use this library to test and develop applications that interact with Azure Key Vault without needing a live connection to the Azure service.

---

Feel free to extend this documentation further as you add more features or modify the emulator's behavior.