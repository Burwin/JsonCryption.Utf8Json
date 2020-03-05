using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Concurrent;

namespace Utf8Json.FLE
{
    internal sealed class DefaultDataProtectorFactory : IDataProtectorFactory
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ConcurrentDictionary<Type, IDataProtector> _cachedDataProtectors;

        public DefaultDataProtectorFactory(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _cachedDataProtectors = new ConcurrentDictionary<Type, IDataProtector>();
        }

        public IDataProtector Create(Type type) => _cachedDataProtectors.GetOrAdd(type, _dataProtectionProvider.CreateProtector(type.FullName));
    }
}
