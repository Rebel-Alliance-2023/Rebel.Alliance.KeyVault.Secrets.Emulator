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
    }

}
