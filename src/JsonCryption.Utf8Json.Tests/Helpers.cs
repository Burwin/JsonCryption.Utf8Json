using Microsoft.AspNetCore.DataProtection;
using Utf8Json;

namespace JsonCryption.Utf8Json.Tests
{
    class Helpers
    {
        public static string SerializedValueOf<T>(T value, IJsonFormatterResolver resolver, string name)
        {
            var serialized = JsonSerializer.ToJsonString<T>(value, resolver);
            return $"\"{name}\":{serialized}";
        }


        public static IJsonFormatterResolver GetEncryptedResolver(IJsonFormatterResolver fallbackResolver)
        {
            var dataProtectionProvider = DataProtectionProvider.Create("test");
            return new EncryptedResolver(fallbackResolver, dataProtectionProvider);
        }
    }
}
