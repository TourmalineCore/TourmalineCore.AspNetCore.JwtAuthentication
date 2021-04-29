# TourmalineCore.AspNetCore.JwtAuthentication

The library can be used for all projects based on .NET Core 3.0 - .NET Core 5.0.

This library contains middleware and authentication extensions.
With this library, you can very easily connect the JWT-based authentication to your project.
Also, this library allows to override the logic of username and password validation.
The library provides the ability to use a debug token to avoid the need to enter a username and password when the lifetime of the JWT expires.

# Authentication

## Basic

To start using JWT-based authentication, need to use one method in the Startup.cs file.
In this case, the default options will be used.

Then, the token will be required in the request header of every authorized endpoint, like this: `Authorization: Bearer {token}`.

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    ...
    services.AddJwtAuthentication();
    ...
}

public async void Configure(IApplicationBuilder app, IHostingEnvironment env) 
{
    ...
    app.UseDefaultLoginMiddleware()
    app.UseJwtAuthentication();
    ...
}
```

## Cookie

This package also allows you to store the received token in a cookie. To do that you need to use Cookie login middleware instead of default login. After successful login the token will be added to a cookie, that user will receive in a response. Then they can use this cookie for the authentication instead of writing the token to the Authentication header of every request. 

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    ...
    services.AddJwtAuthentication();
    ...
}

public async void Configure(IApplicationBuilder app, IHostingEnvironment env) 
{
    ...
    app.UseCookieLoginMiddleware(new CookieAuthOptions{ Key = "ExampleCookieName" });
    app.UseJwtAuthentication();
    ...
}
```

## Options

If you want to use your own values for options, then you need to pass AuthenticationOptions to the AddJwtAuthentication().

Default values:
```
SigningKey = "jwtKeyjwtKeyjwtKeyjwtKeyjwtKey",
Issuer = null,
AccessTokenExpireInMinutes = 10080,
IsDebugTokenEnabled = false,
```

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    ...
    services.Configure<AuthenticationOptions>(Configuration.GetSection(nameof(AuthenticationOptions)));
    var authenticationOptions = services.BuildServiceProvider().GetService<IOptions<AuthenticationOptions>>().Value; 
    services.AddJwtAuthentication(authenticationOptions);
    ...
}
```


## Routing

The default route to the login endpoint is `/auth/login`.
You can change it by passing in a LoginEndpointOptions object to the UseDefaultLoginMiddleware extension. Like this:

```csharp
public async void Configure(IApplicationBuilder app, IHostingEnvironment env) 
{
    ...
    app.UseDefaultLoginMiddleware(new LoginEndpointOptions{ LoginEndpointRoute = "/test/login" });
    app.UseJwtAuthentication();
    ...
}
```
**OR** like this if you are using cookie middleware:

```csharp
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
`
{
  "login": "Admin",
  "password": "Admin"
}
`
 
## Login validation

By default, login will be valid only for `Login="Admin"` and `Password="Admin"`.
You can provide your own implementation of the IUserCredentialsValidator interface, in which implement your own logic for validation of the login and password.

```csharp
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
          .AddJwtAuthentication()
          .AddUserCredentialValidator<UserCredentialsValidator>();
        ...
    }
}
```

## Token usage

To enable token validation, you must add the `[Authorize]` attribute before the controller or method, for example:

For methods:
```csharp
[Authorize]
[HttpGet]
public IEnumerable<object> Get()
{
    //Make something
}
```

For controllers:
```csharp
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
```csharp
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
public void ConfigureServices(IServiceCollection services) 
{
    ...
    services.AddJwtAuthentication()
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

3. To enable checking of permissions, you must add the `RequiredPermission` attribute before the controller or method and pass as a parameter all permissions that are needed , for example:
```csharp
[Authorize]
[RequiredPermission(UserClaimsProvider.FirstExampleClaimName)]
[HttpGet]
public IEnumerable<object> Get()
{
    //Make something
}
```

For controllers:
```csharp
[ApiController]
[Route("[controller]")]
[Authorize]
[RequiredPermission(UserClaimsProvider.FirstExampleClaimName, UserClaimsProvider.SecondExampleClaimName)]
public class ExampleController : ControllerBase
{
    //Some methods
}
```

Thus, only those users who have the desired permission will have access to the controller or controller method.

# Identity
If you are using EF Core, you can use JwtAuthentication.Identity package. It will allow you to use advantages of JWT and store necessary users data in a database.

## Basic usage

1. You will need to inherit your context from JwtAuthIdentityDbContext, provided by this package. It uses a generic parameter of user entity. This entity must inherit from **IdentityUser** class of Microsoft.Identity package.
```csharp
public class AppDbContext : JwtAuthIdentityDbContext<CustomUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
```

2. Then you need to update startup like this:
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        services.AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>();
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
- **RegistrationRequestModel**: Model that will be passed to registration endpoint. It must inherit from the **RegistrationRequestModel** class, provided by this package.

In `UseRegistration` you will also need to pass a mapping function, which will be used to convert **RegistrationRequestModel** to **User** entity.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        services.AddRegistration<CustomUser, CustomRegistrationRequest>();
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app.UseRegistration<CustomUser, CustomRegistrationRequest>(requestModel => new CustomUser()
            {
                UserName = requestModel.Login,
                NormalizedUserName = requestModel.Login,
            });
        ...
    }
}
```

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

1. You will need to inherit your context from JwtAuthIdentityRefreshTokenDbContext, provided by this package.
```csharp
public class AppDbContext : JwtAuthIdentityRefreshTokenDbContext<CustomUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
```

2. Then you need to update startup like this:
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services) 
	{
        ...
        services.AddJwtAuthenticationWithRefreshToken<AppDbContext, CustomUser>();
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app
            .UseJwtAuthentication();
            .UseDefaultLoginMiddleware();
            .UseRefreshTokenMiddleware();
        ...
    }
}
```

### Options of Refresh Token

If you want to use your own values for options, then you need to pass RefreshAuthenticationOptions to the AddJwtAuthenticationWithRefreshToken(). This is inherit from basic AuthenticationOptions and share all the default parameters.

Default values:
```
SigningKey = "jwtKeyjwtKeyjwtKeyjwtKeyjwtKey",
Issuer = null,
AccessTokenExpireInMinutes = 10080,
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

### Login request with a Refresh Token

Requesting login endpoint will be much the same, but you can optionally add a **clientFingerPrint** parameter, that will be saved in the database with a genereted access token. If token has fingerprint, it can only be accessed by providing the same fingerprint value.

In addition to a access token login request will also return a **refresh token** in the response.
`
{
  "login": "Admin",
  "password": "Admin",
  "clientFingerPrint": "fingerprint"
}
`

### Refresh token request

To call the Refresh Token Endpoind, you need to use the POST method, add to the header `Content-Type: application/json` and pass the token value (and optionally fingerprint) in the JSON format in the request body. Like this:
`
{
  "refreshTokenValue": "token", 
  "clientFingerPrint": "fingerprint"
}
`