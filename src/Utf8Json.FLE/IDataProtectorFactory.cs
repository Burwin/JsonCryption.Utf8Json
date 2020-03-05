using Microsoft.AspNetCore.DataProtection;
using System;

namespace Utf8Json.FLE
{
    /// <summary>
    /// Creates a new <see cref="IDataProtector"/>
    /// </summary>
    public interface IDataProtectorFactory
    {
        /// <summary>
        /// Creates a new <see cref="IDataProtector"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IDataProtector Create(Type type);
    }
}
