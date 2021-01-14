# TourmalineCore.AspNetCore.JwtAuthentication

The library can be used for all projects based on .NET Core 3.0 - .NET Core 5.0.

This library contains middleware and authentication extensions.
With this library, you can very easily connect the JWT-based authentication to your project.
Also, this library allows to override the logic of username and password validation.
The library provides the ability to use a debug token to avoid the need to enter a username and password when the lifetime of the JWT expires.

# Authentication

To start using JWT-based authentication, need to use one method in the Startup.cs file.
In this case, the default options will be used.

```
SigningKey = "jwtKeyjwtKeyjwtKeyjwtKeyjwtKey",
Issuer = null,
AccessTokenExpireInMinutes = 10080,
IsDebugTokenEnabled = false,
```

```csharp
public void ConfigureServices(IServiceCollection services) {
    ...
    services.AddJwtAuthentication();
    ...
}

public async void Configure(IApplicationBuilder app, IHostingEnvironment env) {
    ...
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
public void ConfigureServices(IServiceCollection services) {
    ...
    services.Configure<AuthenticationOptions>(Configuration.GetSection(nameof(AuthenticationOptions)));
    var authenticationOptions = services.BuildServiceProvider().GetService<IOptions<AuthenticationOptions>>().Value; 
    services.AddJwtAuthentication(authenticationOptions);
    ...
}
```

##Routing

The default route to the login endpoint is `/auth/login`.
You can change it by calling `OverrideLoginRoute`.

```csharp
public void ConfigureServices(IServiceCollection services) {
    ...
    services
        .AddJwtAuthentication()
        .OverrideLoginRoute("/test/login");
    ...
}
```

##Login validation

By default, login will be valid only for `Login="Admin"` and `Password="Admin"`
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
	
public void ConfigureServices(IServiceCollection services) {
    ...
    services
        .AddJwtAuthentication()
        .AddUserCredentialValidator<UserCredentialsValidator>();
    ...
}
```