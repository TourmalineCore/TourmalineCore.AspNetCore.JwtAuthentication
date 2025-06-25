# TourmalineCore.AspNetCore.JwtAuthentication.Core
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/TourmalineCore/TourmalineCore.AspNetCore.JwtAuthentication/.NET?label=tests%20and%20build)

The library can be used for all projects based on .NET Core 3.0 - .NET 9.0.

Readme for usage on [.NET Core 6.0](README.md) or newer.

We are using Microsoft.AspNetCore.Authentication.JwtBearer with RSA for signing the keys.
This library contains middleware and authentication extensions.
With this library, you can very easily connect the JWT-based authentication to your project.
Also, this library allows to override the logic of username and password validation.
The library provides the ability to use a debug token to avoid the need to enter a username and password when the lifetime of the JWT expires.

# Installation
![Nuget](https://img.shields.io/nuget/v/TourmalineCore.AspNetCore.JwtAuthentication.Core?color=gre&label=stable%20version) ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/TourmalineCore.AspNetCore.JwtAuthentication.Core?label=pre-release%20version) ![Nuget](https://img.shields.io/nuget/dt/TourmalineCore.AspNetCore.JwtAuthentication.Core)

TourmalineCore.AspNetCore.JwtAuthentication.Core is available on [NuGet](https://www.nuget.org/packages/TourmalineCore.AspNetCore.JwtAuthentication.Core/). But also you can install latest stable version using **.NET CLI**
```
dotnet add package TourmalineCore.AspNetCore.JwtAuthentication.Core
```

# Table of Content

- [Authentication](#authentication)
- [Basic Usage](#basic)
- [Cookie](#cookie)
- [Options](#options)
- [Routing](#routing)
- [Login Request Example](#login-request)
- [Login Validation](#login-validation)
- [Token Usage](#token-usage)
- [Authorization](#authorization)
- [Callbacks](#callbacks)

# Authentication

## Basic

To start using JWT-based authentication, need to use one method in the Startup.cs file.
In this case, the default options will be used.

Then, the token will be required in the request header of every authorized endpoint, like this: `Authorization: Bearer {token}`.

```cs
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core;

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
        services.AddJwtAuthentication(authenticationOptions);
        ...
    }

    public async void Configure(IApplicationBuilder app, IHostingEnvironment env) 
    {
        ...
        app.UseDefaultLoginMiddleware()
        app.UseJwtAuthentication();
        ...
    }
}
```

## Cookie

This package also allows you to store the received token in a cookie. To do that you need to use Cookie login middleware instead of default login. After successful login the token will be added to a cookie, that user will receive in a response. Then they can use this cookie for the authentication instead of writing the token to the Authentication header of every request. 

```cs
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core;

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
        services.AddJwtAuthentication(authenticationOptions);
        ...
    }

    public async void Configure(IApplicationBuilder app, IHostingEnvironment env) 
    {
        ...
        app.UseCookieLoginMiddleware(new CookieAuthOptions{ Key = "ExampleCookieName" });
        app.UseJwtAuthentication();
        ...
    }
}
```

## Options

To use package you need to pass AuthenticationOptions to the AddJwtAuthentication().

| Name | Type | Default | Required | Description |
|-|-|-|-|-|
| PrivateSigningKey | string | null | yes | The base64-encoded RSA Private Key |
| PublicSigningKey | string | null | yes | The Matching base64-encoded RSA Public Key |
| Issuer | string | null | no | The Registered Issuer Value |
| Audience | string | null | no | The Registered Audience Value |
| AccessTokenExpireInMinutes | int | 10080 | no | Lifetime of the Access Token |
| IsDebugTokenEnabled | bool | false | no | If true, user credentials will not be checked during authentication |


```cs
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
...

public void ConfigureServices(IServiceCollection services) 
{
    ...
    var authenticationOptions = _configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>()
    services.AddJwtAuthentication(authenticationOptions);
    ...
}
```

Minimum `appsettings.json` configuration:
```json
{
	"AuthenticationOptions": {
		"PublicSigningKey": "<PUT YOUR PUBLIC RSA KEY HERE>",
		"PrivateSigningKey": "<PUT YOUR PRIVATE RSA KEY HERE>"
	}
}
```

For generate pair RSA keys, use [mkjwk](https://mkjwk.org/).
Here we can generate key pair in RSA521 algorithm and 2048 key size. In package we use X.509 PEM Format.

## Routing

The default route to the login endpoint is `/auth/login`.
You can change it by passing in a LoginEndpointOptions object to the UseDefaultLoginMiddleware extension. Like this:

```cs
public async void Configure(IApplicationBuilder app, IHostingEnvironment env) 
{
    ...
    app.UseDefaultLoginMiddleware(new LoginEndpointOptions{ LoginEndpointRoute = "/test/login" });
    app.UseJwtAuthentication();
    ...
}
```
**OR** like this if you are using cookie middleware:

```cs
public async void Configure(IApplicationBuilder app, IHostingEnvironment env) 
{
    ...
    app.UseCookieLoginMiddleware(
        new CookieAuthOptions{ Key = "ExampleCookieName" }, 
        new LoginEndpointOptions{ LoginEndpointRoute = "/test/login" });
    app.UseJwtAuthentication();
    ...
}
```

## Login request

You can call the login endpoint, you need to use the POST method, add to the header `Content-Type: application/json` and pass the login/password in the JSON format in the request body. Like this:
```json
{
  "login": "Admin",
  "password": "Admin"
}
```
 
As a successful result it will return **Access Token Model** json:

```json
{
    "accessToken": {
        "value": "{{ACCESS_TOKEN_VALUE}}",
        "expiresInUtc": "2021-01-01T00:00:00.0000000Z"
    }
}
```

## Login validation

By default, login will be valid only for `Login="Admin"` and `Password="Admin"`.
You can provide your own implementation of the IUserCredentialsValidator interface, in which implement your own logic for validation of the login and password.

```cs
...
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;

public class UserCredentialsValidator : IUserCredentialsValidator
{
    private const string Login = "User";
    private const string Password = "User";

    public Task<bool> ValidateUserCredentials(string login, string password)
    {
        return Task.FromResult(login == Login && password == Password);
    }
}
	
public class Startup
{
    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        services
          .AddJwtAuthentication(authenticationOptions)
          .AddUserCredentialValidator<UserCredentialsValidator>();
        ...
    }
}
```

## Token usage

To enable token validation, you must add the `[Authorize]` attribute before the controller or method, for example:

For methods:
```cs
[Authorize]
[HttpGet]
public IEnumerable<object> Get()
{
    //Make something
}
```

For controllers:
```cs
[Authorize]
[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    //Some methods
}
```

# Authorization

This library implements claims-based authorization. With this, claims are added to the token payload and verified upon request. In order to use this mechanism, you need:

1. Create a class that implements the IUserClaimsProvider interface that will return a list of the claims that you need. For example:
```cs
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

2. Connect this provider in the `Startup.cs`.
   You can pass the name of the claim type you want to use as a parameter. `Default claim type = "Permission"`.
```cs
using TourmalineCore.AspNetCore.JwtAuthentication.Core;

public void ConfigureServices(IServiceCollection services) 
{
    ...
    services.AddJwtAuthentication(authenticationOptions)
            .WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.ExampleClaimType);
    ...
}
```

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
```cs
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
```cs
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

# Callbacks

The library provides the ability to transfer callbacks for a call at the beginning and end of the execution of the authentication, logout and refresh token functions. This feature can be used for logging, to calculate your system usage statistics, and so on.

## Login

To use callbacks for authentication, follow these steps:

1. Create a function that will take `LoginModel` as a parameter and return a result of the Task type. For example:

```cs
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models;

private Task OnLoginExecuting(LoginModel data)
{
    //Make something
}
```

2. In the `Startup` class in the `Configure` method use

```cs
app
    .OnLoginExecuting(OnLoginExecuting)
    .OnLoginExecuted(OnLoginExecuted)
    .UseDefaultLoginMiddleware();
```

## Logout

To call callbacks during logout, you need to follow the same steps as for login. Only use methods `OnLogoutExecuted` and `OnLogoutExecuting`. And your function should take `LogoutModel` as a parameter

## Refresh

To call callbacks during refresh, you need to follow the same steps as for login. Only use methods `OnRefreshExecuted` and `OnRefreshExecuting`. And your function should take `RefreshModel` as a parameter