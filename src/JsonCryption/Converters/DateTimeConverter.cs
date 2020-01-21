﻿using JsonCryption.Encrypters;
using System;
using System.Text.Json;

namespace JsonCryption.Converters
{
    internal sealed class DateTimeConverter : EncryptedConverter<DateTime>
    {
        public DateTimeConverter(Encrypter encrypter, JsonSerializerOptions options) : base(encrypter, options)
        {
        }

        protected override DateTime FromBytes(byte[] bytes)
        {
            var dateAsLong = BitConverter.ToInt64(bytes, 0);
            return DateTime.FromBinary(dateAsLong);
        }

        protected override byte[] ToBytes(DateTime value) => BitConverter.GetBytes(value.ToBinary());
    }
}
