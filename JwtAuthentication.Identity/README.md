# TourmalineCore.AspNetCore.JwtAuthentication.Identity

The library can be used for all projects based on .NET Core 3.0 - .NET Core 5.0.

This library contains middleware and authentication extensions.
With this library, you can very easily connect the JWT-based authentication to your project with usage of EF Core and Identity to store users data.
Optianally, you can enable usage of Refresh token to provide additional level of security to your app. 
Also, this library allows to easily implement registration and logout functionality.

**NOTE**: This package is an extension of TourmalineCore.AspNetCore.JwtAuthentication.Core package, that contains basic functionality of JWT-based authentication. You can find more info about this package [here](https://github.com/TourmalineCore/TourmalineCore.AspNetCore.JwtAuthentication/tree/master/JwtAuthentication.Core)

# Table of Content

- [Basic Usage](#basic-usage)
- [Registration](#registration)
    - [Registration Request](#registration-request)
    - [Registration Routing](#registration-routing)
- [Refresh](#refresh-token)
    - [Login request with a Refresh Token](#Login-request-with-a-Refresh-Token)
    - [Refresh Token Request](#Refresh-Token-Request)
    - [Refresh Token Options](#Refresh-Token-Options)
    - [Refresh Routing](#Refresh-Token-Options)
    - [Logout](#logout)
        - [Logout Request](#logout-request)
- [Authorization](#authorization)


# Basic usage

1. You will need to inherit your context from TourmalineDbContext, provided by this package. It uses a generic parameter of user entity. This entity must inherit from **IdentityUser** class of Microsoft.Identity package.
```csharp
...
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

public class AppDbContext : TourmalineDbContext<CustomUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
```

2. Then you need to update startup like this:
```csharp
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        var authenticationOptions = (_configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>());
        services
                .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
                .AddBaseLogin(authenticationOptions);
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app.UseJwtAuthentication();
        ...
    }
}
```

3. Optionally you can add the default user to the database
```csharp
public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app.UseDefaultDbUser<AppDbContext, CustomUser>("Admin", "Admin");
        ...
    }
}
```

# Registration

Using Identity allows you to easily implement regestration flow. To do that add the `AddRegistration` extension to **ConfigureServices**, and `UseRegistration` method to **Configure**. Both methods requires two generic parameters: 
- **User**: Entity representing the user of your app. It is must be inhereted from IdentityUser.
- **RegistrationRequestModel**: Model that will be passed to registration endpoint. Basically, it contains only two properties necessary for basic registration flow - login and password. You can use **RegistrationRequestModel** class, provided by this package, or your own model inherited from this class. In `UseRegistration` you will also need to pass a mapping function, which will be used to convert **RegistrationRequestModel** to **User** entity.

```csharp
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        var authenticationOptions = (_configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>());
        services
            .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
            .AddBaseLogin(authenticationOptions)
            .AddRegistration();
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app.UseRegistration<CustomUser, RegistrationRequestModel>(requestModel => new CustomUser()
            {
                UserName = requestModel.Login,
                NormalizedUserName = requestModel.Login,
            });
        ...
    }
}
```

## Registration request

You can call the registration endpoint, you need to use the POST method, add to the header `Content-Type: application/json` and pass the JSON object representing chosen **RegistrationRequestModel** in the request body. Like this:
```json
{ 
    "login": "Admin", 
    "password": "Admin" 
}
```

As a successful result it will return **Access Token Model**, so user will be automaically logged in.

## Registration Routing

The default route to the Registration endpoint is `/auth/register`.
You can change it by passing in a **RegistrationEndpointOptions** object to the **UseRegistration** extension. Like this:

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseRegistration<CustomUser, CustomRegistrationRequest>(requestModel => new CustomUser()
        {
            UserName = requestModel.Login,
            NormalizedUserName = requestModel.Login,
        },
        new RegistrationEndpointOptions()
        { 
            RegistrationEndpointRoute = "/new-user" 
        });
    ...
}
```

# Refresh token

If you want to add another layer of security to your application, you can use the refresh token. By using it you can reduce the lifetime of the access token, but  provide the ability to update it without re-login with an additional long-live token stored in your database.

```csharp
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        var authenticationOptions = (_configuration.GetSection(nameof(AuthenticationOptions)).Get<RefreshAuthenticationOptions>());
        services
            .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
            .AddLoginWithRefresh(authenticationOptions);
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app
            .UseJwtAuthentication()
            .UseDefaultLoginMiddleware()
            .UseRefreshTokenMiddleware();
        ...
    }
}
```
## Login request with a Refresh Token

Requesting login endpoint will be much the same, but you can optionally add a **clientFingerPrint** parameter, that will be saved in the database with a genereted access token. If token has fingerprint, it can only be accessed by providing the same fingerprint value.

In addition to a access token login request will also return a **refresh token** in the response.
```json
{
  "login": "Admin",
  "password": "Admin",
  "clientFingerPrint": "{{FINGERPRINT}}"
}
```

When you use a refresh token, its value will be added to every successful login response (**Access Token Model**), so it will look like this:

```json
{
    "accessToken": {
        "value": "{{ACCESS_TOKEN_VALUE}}",
        "expiresInUtc": "2021-01-01T00:00:00.0000000Z"
    },
    "refreshToken": {
        "value": "{{REFRESH_TOKEN_VALUE}}",
        "expiresInUtc": "2021-01-01T00:00:00.0000000Z"
    }
}
```

## Refresh Token Request

To call the Refresh Token Endpoind, you need to use the POST method, add to the header `Content-Type: application/json` and pass the token value (and optionally fingerprint) in the JSON format in the request body. Like this:
```json
{ 
    "refreshTokenValue": "{{REFRESH_TOKEN}}", 
    "clientFingerPrint": "{{FINGERPRINT}}" 
}
```

As a successful result it will return **Access Token Model**.

## Refresh Token Options

If you want to use your own values for options, then you need to pass RefreshAuthenticationOptions to the AddJwtAuthenticationWithRefreshToken(). This is inherit from basic AuthenticationOptions and share all the default parameters.

To use package you need to pass AuthenticationOptions to the AddJwtAuthentication().

| Name | Type | Default | Required | Description |
|-|-|-|-|-|
| PrivateSigningKey | string | null | yes | The base64-encoded RSA Private Key |
| PublicSigningKey | string | null | yes | The Matching base64-encoded RSA Public Key |
| Issuer | string | null | no | The Registered Issuer Value |
| Audience | string | null | no | The Registered Audience Value |
| AccessTokenExpireInMinutes | int | 15 | no | Lifetime of the Access Token |
| RefreshTokenExpireInMinutes | int | 10080 | no | Lifetime of the Refresh Token |
| IsDebugTokenEnabled | bool | false | no | If true, user credentials will not be checked during authentication |


```csharp
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
...

public void ConfigureServices(IServiceCollection services) 
{
    ...
    var authenticationOptions = _configuration.GetSection("AuthenticationOptions").Get<RefreshAuthenticationOptions>();

    services
        .AddJwtAuthenticationWithIdentity<AppDbContext, User>()
        .AddLoginWithRefresh(authenticationOptions);
    ...
}
```

Minimum appsettings.json configuration:
```json
{
	"AuthenticationOptions": {
		"PublicSigningKey": "<PUT YOUR PUBLIC RSA KEY HERE>",
		"PrivateSigningKey": "<PUT YOUR PRIVATE RSA KEY HERE>"
	}
}
```

## Refresh Routing

The default route to the refresh endpoint is `/auth/refresh`.
You can change it by passing in a **RefreshEndpointOptions** object to the **UseRefreshTokenMiddleware** extension. Like this:

```csharp
...
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
...

public async void Configure(IApplicationBuilder app, IHostingEnvironment env) 
{
    ...
    app
        .UseJwtAuthentication()
        .UseDefaultLoginMiddleware();
        .UseRefreshTokenMiddleware(new RefreshEndpointOptions
        { 
            RefreshEndpointRoute = "/test/refresh",
        });
    ...
}
```

# Logout

If you are using the refresh token, you will probably want to have a possibility to remove token's data from the database, when user requests it. This can be achieved by implementing the Logout mechanism. You can simply enable it like this:

```csharp
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

public class Startup
{
    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        services
            .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
            .AddLoginWithRefresh(authenticationOptions)
            .AddLogout();
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app
            .UseJwtAuthentication()
            .UseDefaultLoginMiddleware()
            .UseRefreshTokenMiddleware()
            .UseRefreshTokenLogoutMiddleware();
        ...
    }
}
```

## Logout request

To call the Logout Endpoind, you need to use the POST method, add to the header `Content-Type: application/json` and pass the refresh token value (and optionally fingerprint) in the JSON format in the request body. Like this:
```json
{ 
    "refreshTokenValue": "{{REFRESH_TOKEN}}", 
    "clientFingerPrint": "{{FINGERPRINT}}" 
}
```

If it was successful, it will return `true` in a response body.

# Authorization

This library implements claims-based authorization. With this, claims are added to the token payload and verified upon request. In order to use this mechanism, you need:

1. Create a class that implements the IUserClaimsProvider interface that will return a list of the claims that you need. For example:
```csharp
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;

public class UserClaimsProvider : IUserClaimsProvider
{
    public const string ExampleClaimType = "ExamplePermission";
    
    private const string FirstExampleClaimName = "CanUseExampleFirst";

    private const string SecondExampleClaimName = "CanUseExampleSecond";
	
    public Task<List<Claim>> GetUserClaimsAsync(string login)
    {
        return Task.FromResult(new List<Claim>
            {
                new Claim(ExampleClaimType, FirstExampleClaimName),
                new Claim(ExampleClaimType, SecondExampleClaimName),
            });
    }
}
```

2. Connect this provider in the Startup.cs.
   You can pass the name of the claim type you want to use as a parameter. `Default claim type = "Permission"`.
```csharp
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

public void ConfigureServices(IServiceCollection services) 
{
    ...
    services.AddJwtAuthenticationWithIdentity<AppDbContext, User>()
            .AddLoginWithRefresh(authenticationOptions)
            .WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.ExampleClaimType);
    ...
}
```

Please note that if you enable functionality for the refresh token, then `WithUserClaimsProvider` should be called after `AddLoginWithRefresh`.

The claims in the token will look like this:
```
{
  "ExamplePermission": [ // instead "ExamplePermission" will be "Permission" when using the default option
    "CanUseExampleFirst",
    "CanUseExampleSecond"
  ],
  "exp": 1611815230
}
```

3. To enable checking of permissions, you must add the `RequiresPermission` attribute before the controller or method and pass as a parameter all permissions that are needed , for example:
```csharp
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

[Authorize]
[RequiresPermission(UserClaimsProvider.FirstExampleClaimName)]
[HttpGet]
public IEnumerable<object> Get()
{
    //Make something
}
```

For controllers:
```csharp
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

[ApiController]
[Route("[controller]")]
[Authorize]
[RequiresPermission(UserClaimsProvider.FirstExampleClaimName, UserClaimsProvider.SecondExampleClaimName)]
public class ExampleController : ControllerBase
{
    //Some methods
}
```

Thus, only those users who have the desired permission will have access to the controller or controller method.