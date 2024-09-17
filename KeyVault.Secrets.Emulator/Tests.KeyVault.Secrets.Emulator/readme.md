Here is a detailed Markdown documentation file for the **Test Project** that accompanies your Azure Key Vault emulator library. This document covers every class, property, and method, providing a comprehensive overview of the test cases and their purposes.

### Azure Key Vault Emulator Test Project Documentation

# Tests.Rebel.Alliance.KeyVault.Secrets.Emulator

## Overview

The **Azure Key Vault Emulator Test Project** provides a suite of unit tests to ensure the functionality and reliability of the `Rebel.Alliance.KeyVault.Secrets.Emulator` library. The tests cover all critical operations of the emulator, including storing, retrieving, deleting, and recovering secrets. The tests utilize the xUnit testing framework with `Serilog` for logging.

## Classes

### 1. `SecretClientTests`

Contains all the unit tests for the `SecretClient` class of the Key Vault emulator.

#### Fields

- **`_logger` (ILogger<SecretClient>)**:  
  The logger instance used for logging test operations. Configured to log output to both the console and xUnit test output.

- **`_client` (SecretClient)**:  
  An instance of the `SecretClient` class, which is the subject under test.

#### Constructor

- **`SecretClientTests(ITestOutputHelper output)`**:  
  Initializes a new instance of the `SecretClientTests` class, setting up the logger and creating the `SecretClient` instance.  
  *Parameters*:
  - `output` (ITestOutputHelper): The xUnit test output helper used for logging.

#### Methods

- **`SetSecretAsync_ShouldStoreSecret()`**:  
  Verifies that a secret is correctly stored when using the `SetSecretAsync` method of the `SecretClient`.  
  *Asserts*:
  - The secret's name matches the input.
  - The secret's value matches the input.

- **`GetSecretAsync_ShouldRetrieveSecret()`**:  
  Verifies that a stored secret can be correctly retrieved using the `GetSecretAsync` method of the `SecretClient`.  
  *Asserts*:
  - The retrieved secret's name matches the stored secret's name.
  - The retrieved secret's value matches the stored secret's value.

- **`SetSecretAsync_WithByteArray_ShouldStoreSecret()`**:  
  Verifies that a secret with a base64-encoded byte array value is correctly stored using the `SetSecretAsync` method of the `SecretClient`.  
  *Asserts*:
  - The secret's name matches the input.
  - The secret's value (base64 string) matches the input.

- **`GetSecretAsync_WithByteArray_ShouldRetrieveAndConvertSecret()`**:  
  Verifies that a stored secret with a base64-encoded byte array value can be correctly retrieved and decoded.  
  *Asserts*:
  - The retrieved secret's name matches the stored secret's name.
  - The retrieved secret's value (base64 string) matches the stored secret's value.
  - The decoded byte array matches the original byte array.

- **`DeleteSecretAsync_ShouldMarkSecretAsDeleted()`**:  
  Verifies that a secret is correctly marked as deleted when using the `DeleteSecretAsync` method of the `SecretClient`.  
  *Asserts*:
  - A `KeyNotFoundException` is thrown when attempting to retrieve the deleted secret.
  - The exception message correctly indicates the secret is not found.

- **`PurgeDeletedSecretAsync_ShouldRemoveSecretPermanently()`**:  
  Verifies that a deleted secret is permanently removed when using the `PurgeDeletedSecretAsync` method of the `SecretClient`.  
  *Asserts*:
  - A `KeyNotFoundException` is thrown when attempting to recover a purged secret.
  - The exception message correctly indicates the deleted secret is not found.

- **`RecoverDeletedSecretAsync_ShouldRestoreDeletedSecret()`**:  
  Verifies that a deleted secret is correctly restored when using the `RecoverDeletedSecretAsync` method of the `SecretClient`.  
  *Asserts*:
  - The recovered secret's name matches the original secret's name.
  - The recovered secret's value matches the original secret's value.

- **`UpdateSecretPropertiesAsync_ShouldUpdateProperties()`**:  
  Verifies that a secret's properties are correctly updated when using the `UpdateSecretPropertiesAsync` method of the `SecretClient`.  
  *Asserts*:
  - The updated secret's name matches the original secret's name.
  - The updated content type matches the input.
  - The updated tags contain the expected key-value pair.

- **`Dispose()`**:  
  Cleans up resources after each test run.  
  *Usage*: Used for any necessary cleanup operations after tests are executed.

## Usage Examples

### Example Test Execution

To run the test suite, you can use any of the following methods:

1. **Visual Studio Test Explorer**:  
   Open the `Test Explorer` in Visual Studio, and run all or specific tests by clicking the "Run" button.

2. **Command Line with .NET CLI**:  
   Run the following command in your terminal or command prompt:
   ```sh
   dotnet test
   ```
   This command will execute all tests in the project and display the results in the console.

3. **Continuous Integration (CI) Pipelines**:  
   Integrate the test suite with your CI pipeline (e.g., GitHub Actions, Azure Pipelines) by configuring the pipeline to run `dotnet test` as part of the build and test stages.

### Example Test Output with Serilog Logging

```plaintext
[Information] Searching for secrets with tag: Environment = Production
[Information] Getting secret: MySecret
[Information] Setting secret: MySecret
[Information] Deleting secret: MySecretToDelete
[Information] Recovering deleted secret: MySecretToRecover
```

### Setting Up the Test Environment

1. **Ensure All Dependencies Are Installed**:  
   Make sure all necessary packages, such as `xunit`, `Serilog`, `Serilog.Sinks.XUnit`, and `Microsoft.NET.Test.Sdk`, are installed. You can restore packages using:
   ```sh
   dotnet restore
   ```

2. **Run the Tests**:  
   Run the tests using your preferred method (e.g., Visual Studio, .NET CLI).

## Conclusion

The **Azure Key Vault Emulator Test Project** provides a comprehensive suite of tests to ensure the `SecretClient` class's correct and reliable behavior. These tests cover all essential secret management functionalities, including storing, retrieving, deleting, recovering, and updating secrets. By running these tests, you can verify that your emulator implementation closely mimics Azure Key Vault behavior and functions as expected in various scenarios.

---

Feel free to update or expand this documentation as you add more tests or modify existing ones.