# Utf8Json.FLE
## Field Level Encryption (FLE) plugin for Utf8Json
Utf8Json.FLE offers Field Level Encryption (FLE) when serializing/deserializing between .NET objects and JSON.

### Installation

```
// Package Manager
Install-Package Utf8Json.FLE

// .NET CLI
dotnet add package Utf8Json.FLE
```

### Motivation
Field Level Encryption of C# objects during JSON serialization/deserialization should be:
- Relatively easy to use
- Powered by industry-standard cryptography best-practices

#### Relatively Easy to Use
With default configuration, encrypting a field/property just requires decorating it with `EncryptAttribute`, and serializing the object as usual:
```
// decorate properties to be encrypted
class Foo
{
    [Encrypt]
    public string MySecret { get; set; }
}

// serialize as normal
Foo foo = new Foo() { ... };
JsonSerializer.Serialize(foo);
```

More details on usage scenarios can be found below.

#### Industry-standard Cryptography
Currently, Utf8Json.FLE is built on top of the `Microsoft.AspNetCore.DataProtection` library for handling encryption-related responsibilities:
- Encryption/decryption
- Key management
- Algorithm management
- etc.

Internally, we only depend on the single interface `IDataProtector`. If you don't want to use Microsoft's implementations, you could just depend on `Microsoft.AspNetCore.DataProtection.Abstractions` and provide an alternative implementation of `IDataProtector` when configuring Utf8Json.FLE.

In the future, I would like to build some more customization around this to enable advanced scenarios of building different instances of `IDataProtector`. One use case for this functionality would be creating a segregated `IDataProtector` per user, potentially making it easy to support GDPR's "right to forget" user data.

### Supported Types
Utf8Json.FLE should support any type serializable by Utf8Json. If you spot a missing type or find odd behavior, please let me know (or better yet, create a PR!).

### Getting Started
#### Configuration
##### Step 1: Configure Microsoft.AspNetCore.DataProtection
Utf8Json.FLE depends on the `Microsoft.AspNetCore.DataProtection` library. Therefore, you should first ensure that your DataProtection layer is [configured properly](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/).

##### Step 2: Configure Utf8Json
Next, you'll need to set your default `IJsonFormatterResolver` to be an instance of `EncryptedResolver`, which should have a Singleton lifetime in your app.

`EncryptedResolver` takes two arguments:
- An instance of `IJsonFormatterResolver`
- An instance of `IDataProtector`

The `IJsonFormatterResolver` serves two purposes. It is used to:
- Serialize/deserialize when encryption isn't needed for a given field/property
- Do unencrypted serialization/deserialization in the encrypted chain, prior to encrypting and after decrypting the resulting JSON string

The `IDataProtector` is what encrypts your data. In a future release, we will likely treat this internally as an `IDataProtectionProvider` to create segregated `IDataProtector` instances per `IJsonFormatter<T>`. Since `IDataProtectionProvider` inherits from `IDataProtector`, this shouldn't cause breaking changes IN THE CODE. But it will likely cause issues decrypting already encrypted data. Since I don't want to write migration support, this library will be officially a `beta` until this is finished.

```
var fallbackResolver = StandardResolver.AllowPrivate;
var dataProtector = ...;
var encryptedResolver = new EncryptedResolver(fallbackResolver, dataProtector);
JsonSerializer.SetDefaultResolver(encryptedResolver);
```

#### Usage
Once configured, using Utf8Json.FLE is just a matter of decorating the properties/fields you wish to encrypt with the `EncryptAttribute` and serializing your C# objects as you normally would:
```
var myFoo = new Foo("If the Foo shits, wear it.", "Utf8Json.FLE");

class Foo
{
    [Encrypt]
    public string LaunchCodes { get; }
  
    public string FavoriteNugetPackage { get; }
	
	public Foo(string launchCodes, string favoriteNugetPackage)
	{
		LaunchCodes = launchCodes;
		FavoriteNugetPackage = favoriteNugetPackage;
	}
}

// serializing
var bytes = JsonSerializer.Serialize(myFoo);
var json = Encoding.Utf8.GetString(bytes);

// deserializing
var fromBytes = JsonSerializer.Deserialize<Foo>(bytes);
var fromJson = JsonSerializer.Deserialize<Foo>(json);
```

### Special Stuff
As much as possible, I'm trying to keep annotations usage as close to parity with Utf8Json as possible. Here's a current sampling:

#### Constructors
Utf8Json.FLE resolves the constructor used during deserialization in a couple steps. It shouldn't matter whether or not the constructor is public or private. See the tests for details.
1. If a constructor is decorated with `SerializationConstructorAttribute`, it's the constructor that will be used
```
class Foo
{
	[Encrypt]
	public int MyInt { get; }
	
	// This constructor will be used
	[SerializationConstructor]
	private Foo() { }
	
	public Foo(int myInt) { ... }
}
```
2. Otherwise, we try to find the constructor with the most parameter matches (by name, case-insensitive)
```
class Foo
{
	[Encrypt]
	public int MyInt { get; }
	
	[Encrypt]
	public string MyString { get; }
	
	public Foo() { }
	public Foo(int myInt) { ... }
	
	// This constructor will be used
	public Foo(int myInt, string myString) { ... }
}
```
3. Otherwise, we use the default constructor

After the object is rehydrated via the resolved constructor, individual serialized fields and properties not covered by the constructor will still be set.

#### Non-public Properties and Fields
Set the `fallbackResolver` of the `EncryptedResolver` to any `IJsonFormatterResolver` with `AllowPrivate` set to true. Then it should just work.

#### Custom JSON Serialized Property Names
To customize the name used for the field/property in the resulting JSON, decorate the field/property with `DataMemberAttribute` and provide a `Name`:
```
class Foo
{
	[Encrypt]
	[DataMember(Name = "launchCode")]
	public int MyInt { get; }
}
```

#### Ignored Fields/Properties
To ignore a given field/property, decorate it with `IgnoreDataMemberAttribute`:
```
class Foo
{
	[IgnoreDataMember]
	public int MyInt { get; }
}
```

### Future Plans
Utf8Json.FLE is open to PRs...

Future projects/enhancements:
- Benchmarking
- Segregated `IDataProtector` instances per `IJsonFormatter<T>` by default
- Provide interface to create `IDataProtector` instances by any custom logic
