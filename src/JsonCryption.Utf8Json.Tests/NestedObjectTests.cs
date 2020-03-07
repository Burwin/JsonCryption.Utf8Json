﻿using Shouldly;
using System.Text;
using Utf8Json.Resolvers;
using Xunit;

namespace Utf8Json.FLE.Tests
{
    public class NestedObjectTests
    {
        [Fact]
        public void Single_level_nested_object()
        {
            var instance = new FooOne
            {
                MyFooZero = new FooZero
                {
                    MyInt = 75,
                    MyString = "secret"
                }
            };

            var resolver = Helpers.GetEncryptedResolver(StandardResolver.AllowPrivate);

            var bytes = JsonSerializer.Serialize(instance, resolver);
            var json = Encoding.UTF8.GetString(bytes);

            json.ShouldContain("MyFooZero");

            json.ShouldNotContain(Helpers.SerializedValueOf(instance.MyFooZero.MyInt, StandardResolver.AllowPrivate, "MyInt"));
            json.ShouldNotContain(Helpers.SerializedValueOf(instance.MyFooZero.MyString, StandardResolver.AllowPrivate, "MyString"));

            var deserialized = JsonSerializer.Deserialize<FooOne>(json, resolver);

            deserialized.MyFooZero.MyInt.ShouldBe(instance.MyFooZero.MyInt);
            deserialized.MyFooZero.MyString.ShouldBe(instance.MyFooZero.MyString);
        }

        class FooZero
        {
            [Encrypt]
            public int MyInt { get; set; }

            [Encrypt]
            public string MyString { get; set; }
        }

        class FooOne
        {
            public FooZero MyFooZero { get; set; }
        }

        [Fact]
        public void Two_level_nested_object()
        {
            var instance = new FooTwo
            {
                MyFooOne = new FooOne
                {
                    MyFooZero = new FooZero
                    {
                        MyInt = 75,
                        MyString = "secret"
                    }
                }
            };

            var resolver = Helpers.GetEncryptedResolver(StandardResolver.AllowPrivate);

            var bytes = JsonSerializer.Serialize(instance, resolver);
            var json = Encoding.UTF8.GetString(bytes);

            json.ShouldContain("MyFooOne");
            json.ShouldContain("MyFooZero");

            json.ShouldNotContain(Helpers.SerializedValueOf(instance.MyFooOne.MyFooZero.MyInt, StandardResolver.AllowPrivate, "MyInt"));
            json.ShouldNotContain(Helpers.SerializedValueOf(instance.MyFooOne.MyFooZero.MyString, StandardResolver.AllowPrivate, "MyString"));

            var deserialized = JsonSerializer.Deserialize<FooTwo>(json, resolver);

            deserialized.MyFooOne.MyFooZero.MyInt.ShouldBe(instance.MyFooOne.MyFooZero.MyInt);
            deserialized.MyFooOne.MyFooZero.MyString.ShouldBe(instance.MyFooOne.MyFooZero.MyString);
        }

        class FooTwo
        {
            public FooOne MyFooOne { get; set; }
        }

        [Fact]
        public void Three_level_nested_object()
        {
            var instance = new FooThree
            {
                MyFooTwo = new FooTwo
                {
                    MyFooOne = new FooOne
                    {
                        MyFooZero = new FooZero
                        {
                            MyInt = 75,
                            MyString = "secret"
                        }
                    }
                }
            };

            var resolver = Helpers.GetEncryptedResolver(StandardResolver.AllowPrivate);

            var bytes = JsonSerializer.Serialize(instance, resolver);
            var json = Encoding.UTF8.GetString(bytes);

            json.ShouldContain("MyFooTwo");
            json.ShouldContain("MyFooOne");
            json.ShouldContain("MyFooZero");

            json.ShouldNotContain(Helpers.SerializedValueOf(instance.MyFooTwo.MyFooOne.MyFooZero.MyInt, StandardResolver.AllowPrivate, "MyInt"));
            json.ShouldNotContain(Helpers.SerializedValueOf(instance.MyFooTwo.MyFooOne.MyFooZero.MyString, StandardResolver.AllowPrivate, "MyString"));

            var deserialized = JsonSerializer.Deserialize<FooThree>(json, resolver);

            deserialized.MyFooTwo.MyFooOne.MyFooZero.MyInt.ShouldBe(instance.MyFooTwo.MyFooOne.MyFooZero.MyInt);
            deserialized.MyFooTwo.MyFooOne.MyFooZero.MyString.ShouldBe(instance.MyFooTwo.MyFooOne.MyFooZero.MyString);
        }

        class FooThree
        {
            public FooTwo MyFooTwo { get; set; }
        }
    }
}