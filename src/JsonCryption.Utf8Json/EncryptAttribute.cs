using System;

namespace Utf8Json.FLE
{
    /// <summary>
    /// Decorate fields and properties to encrypt/decrypt when serializing/deserializing
    /// 
    /// class Foo
    /// {
    ///     [Encrypt]
    ///     private string _myPrivateEncryptedField;
    /// }
    /// </summary>
    public sealed class EncryptAttribute : Attribute
    {
    }
}
