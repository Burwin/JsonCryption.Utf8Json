using Microsoft.AspNetCore.DataProtection;

namespace Utf8Json.FLE.Tests
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
            var dataProtectorFactory = new DefaultDataProtectorFactory(dataProtectionProvider);
            return new EncryptedResolver(fallbackResolver, dataProtectorFactory);
        }
    }
}
