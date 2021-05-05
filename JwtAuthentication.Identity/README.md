# TourmalineCore.AspNetCore.JwtAuthentication.Identity

The library can be used for all projects based on .NET Core 3.0 - .NET Core 5.0.

This library contains middleware and authentication extensions.
With this library, you can very easily connect the JWT-based authentication to your project with usage of EF Core and Identity to store users data.
Optianally, you can enable usage of Refresh token to provide additional level of security to your app. 
Also, this library allows to easily implement registration and logout functionality.

**NOTE**: This package is an extension of TourmalineCore.AspNetCore.JwtAuthentication.Core package, that contains basic functionality of JWT-based authentication. You can find more info about this package [here](https://github.com/TourmalineCore/TourmalineCore.AspNetCore.JwtAuthentication/tree/master/JwtAuthentication.Core)


## Basic usage

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
    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        services
                .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
                .AddBaseLogin();
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

## Registration

Using Identity allows you to easily implement regestration flow. To do that add the `AddRegistration` extension to **ConfigureServices**, and `UseRegistration` method to **Configure**. Both methods requires two generic parameters: 
- **User**: Entity representing the user of your app. It is must be inhereted from IdentityUser.
- **RegistrationRequestModel**: Model that will be passed to registration endpoint. Basically, it contains only two properties necessary for basic registration flow - login and password. You can use **RegistrationRequestModel** class, provided by this package, or your own model inherited from this class. In `UseRegistration` you will also need to pass a mapping function, which will be used to convert **RegistrationRequestModel** to **User** entity.

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
            .AddBaseLogin()
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

### Registration Routing

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

## Refresh token

If you want to add another layer of security to your application, you can use the refresh token. By using it you can reduce the lifetime of the access token, but  provide the ability to update it without re-login with an additional long-live token stored in your database.

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
            .AddLoginWithRefresh();
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
### Login request with a Refresh Token

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

### Refresh token request

To call the Refresh Token Endpoind, you need to use the POST method, add to the header `Content-Type: application/json` and pass the token value (and optionally fingerprint) in the JSON format in the request body. Like this:
```json
{ 
    "refreshTokenValue": "{{REFRESH_TOKEN}}", 
    "clientFingerPrint": "{{FINGERPRINT}}" 
}
```

As a successful result it will return **Access Token Model**.

### Options of Refresh Token

If you want to use your own values for options, then you need to pass RefreshAuthenticationOptions to the AddJwtAuthenticationWithRefreshToken(). This is inherit from basic AuthenticationOptions and share all the default parameters.

Default values:
```
SigningKey = "jwtKeyjwtKeyjwtKeyjwtKeyjwtKey",
Issuer = null,
AccessTokenExpireInMinutes = 15,
RefreshTokenExpireInMinutes = 10080,
IsDebugTokenEnabled = false
```

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    ...
    services.Configure<RefreshAuthenticationOptions>(Configuration.GetSection(nameof(RefreshAuthenticationOptions)));
    var authenticationOptions = services.BuildServiceProvider().GetService<IOptions<RefreshAuthenticationOptions>>().Value; 
    services.AddJwtAuthenticationWithRefreshToken(authenticationOptions);
    ...
}
```

### Refresh Routing

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

### Logout

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
            .AddLoginWithRefresh()
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

### Logout request

To call the Logout Endpoind, you need to use the POST method, add to the header `Content-Type: application/json` and pass the refresh token value (and optionally fingerprint) in the JSON format in the request body. Like this:
```json
{ 
    "refreshTokenValue": "{{REFRESH_TOKEN}}", 
    "clientFingerPrint": "{{FINGERPRINT}}" 
}
```

If it was successful, it will return `true` in a response body.