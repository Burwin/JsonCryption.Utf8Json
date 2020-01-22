﻿using JsonCryption.Converters;
using JsonCryption.Encrypters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonCryption
{
    public class Coordinator
    {
        private readonly Dictionary<Type, Func<Encrypter, JsonSerializerOptions, JsonConverter>> _defaultConverterFactories;
        private readonly Dictionary<(Type Type, JsonSerializerOptions Options), JsonConverterFactory> _specialConverterFactories;
        private readonly Dictionary<Type, JsonConverter> _converters; 
        
        public static Coordinator Singleton { get; private set; }
        public CoordinatorOptions Options { get; }
        internal Encrypter Encrypter => Options.Encrypter;
        public JsonSerializerOptions JsonSerializerOptions => Options.JsonSerializerOptions;

        private Coordinator(CoordinatorOptions options)
        {
            Options = options;

            _specialConverterFactories = new Dictionary<(Type Type, JsonSerializerOptions Options), JsonConverterFactory>();

            _defaultConverterFactories = BuildConverterFactories();
            _converters = _defaultConverterFactories
                .Select(kvp => (kvp.Key, Converter: kvp.Value.Invoke(Encrypter, JsonSerializerOptions)))
                .ToDictionary(x => x.Key, x => x.Converter);
        }

        internal JsonConverter GetConverter(Type typeToConvert, JsonSerializerOptions options)
            => options is null ? _converters[typeToConvert] : CreateConverter(typeToConvert, options);

        private JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            options ??= JsonSerializerOptions;
            
            if (typeToConvert.IsEnum)
            {
                var key = (typeToConvert, options);
                if (!_specialConverterFactories.TryGetValue(key, out var factory))
                    _specialConverterFactories[key] = factory = new EnumConverterFactory(Encrypter, options);

                return factory.CreateConverter(typeToConvert, options);
            }

            // byte arrays are handled more natively
            if (EnumerableConverterFactory.CanConvertType(typeToConvert) && typeToConvert != typeof(byte[]))
            {
                var key = (typeToConvert, options);
                if (!_specialConverterFactories.TryGetValue(key, out var factory))
                    _specialConverterFactories[key] = factory = new EnumerableConverterFactory(Encrypter, options);

                return factory.CreateConverter(typeToConvert, options);
            }
            
            if (!_defaultConverterFactories.TryGetValue(typeToConvert, out var factoryMethod))
                throw new InvalidOperationException($"No Converter for type {typeToConvert.FullName}");

            return factoryMethod.Invoke(Encrypter, options);
        }

        internal bool HasConverter(Type type)
        {
            if (type.IsEnum)
                return true;

            if (type.IsArray)
                return true;

            return _converters.ContainsKey(type);
        }

        public static Coordinator Configure(Action<CoordinatorOptions> configure)
        {
            var options = CoordinatorOptions.Empty;
            configure(options);
            options.Validate();

            Singleton = new Coordinator(options);
            return Singleton;
        }

        public static Coordinator ConfigureDefault(byte[] key) => Configure(options => options.Encrypter = new AesManagedEncrypter(key));

        private Dictionary<Type, Func<Encrypter, JsonSerializerOptions, JsonConverter>> BuildConverterFactories()
        {
            return new Dictionary<Type, Func<Encrypter, JsonSerializerOptions, JsonConverter>>
            {
                {typeof(bool), (encrypter, options) => new BooleanConverter(encrypter, options) },
                {typeof(byte), (encrypter, options) => new ByteConverter(encrypter, options) },
                {typeof(byte[]), (encrypter, options) => new ByteArrayConverter(encrypter, options) },
                {typeof(char), (encrypter, options) => new CharConverter(encrypter, options) },
                {typeof(DateTime), (encrypter, options) => new DateTimeConverter(encrypter, options) },
                {typeof(DateTimeOffset), (encrypter, options) => new DateTimeOffsetConverter(encrypter, options) },
                {typeof(decimal), (encrypter, options) => new DecimalConverter(encrypter, options) },
                {typeof(double), (encrypter, options) => new DoubleConverter(encrypter, options) },
                {typeof(short), (encrypter, options) => new ShortConverter(encrypter, options) },
                {typeof(int), (encrypter, options) => new IntConverter(encrypter, options) },
                {typeof(long), (encrypter, options) => new LongConverter(encrypter, options) },
                {typeof(sbyte), (encrypter, options) => new SByteConverter(encrypter, options) },
                {typeof(float), (encrypter, options) => new FloatConverter(encrypter, options) },
                {typeof(string), (encrypter, options) => new StringConverter(encrypter, options) },
                {typeof(ushort), (encrypter, options) => new UnsignedShortConverter(encrypter, options) },
                {typeof(uint), (encrypter, options) => new UnsignedIntConverter(encrypter, options) },
                {typeof(ulong), (encrypter, options) => new UnsignedLongConverter(encrypter, options) },
                {typeof(Guid), (encrypter, options) => new GuidConverter(encrypter, options) },
            };
        }
    }
}
