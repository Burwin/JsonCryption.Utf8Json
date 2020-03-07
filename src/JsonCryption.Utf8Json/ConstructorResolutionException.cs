using System;

namespace Utf8Json.FLE
{
    /// <summary>
    /// Exception thrown when an expected constructor for a given type cannot be found
    /// </summary>
    [Serializable]
    public sealed class ConstructorResolutionException : Exception
    {
        /// <summary>
        /// Create a new instance from the provided Type
        /// </summary>
        /// <param name="type">The Type for which an expected constructor cannot be found</param>
        public ConstructorResolutionException(Type type)
            : base($"No suitable constructor found for type {type.Name}")
        {
        }

        private ConstructorResolutionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
