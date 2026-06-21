# TourmalineCore.AspNetCore.JwtAuthentication

This project contains libraries that will help you very easily connect the JWT-based authentication to your project.
The libraries can be used for all projects based on .NET Core 3.0 - .NET 9.0.

## [JwtAuthentication.Core](https://github.com/TourmalineCore/TourmalineCore.AspNetCore.JwtAuthentication/tree/master/JwtAuthentication.Core)
Basic JWT-based authentication implementation.

## [JwtAuthentication.Identity](https://github.com/TourmalineCore/TourmalineCore.AspNetCore.JwtAuthentication/tree/master/JwtAuthentication.Identity)
JWT-based authentication implemented with usage of EF Core and Identity to store user's data. Features Refresh tokens, Registration and Logout functionality.

## How to Add Support for a New .NET Version
In the root folder of the project, there is a PowerShell script named `Add-ExampleVersion.ps1`. It is designed to automate the creation of new example and test projects for a specific .NET version by copying existing ones.


The script requires two parameters: `sourceVersion` and `targetVersion`. 
- `sourceVersion` — the version to copy projects from
- `targetVersion` — the new version to create projects for

Example of calling the script:
```powershell
.\Add-ExampleVersion.ps1 -sourceVersion 7.0 -targetVersion 10.0
```

### Steps to update:
1. Run the script as shown above;
1. In each `.csproj` (`Core` and `Identity`) file, update the `TargetFrameworks` element to include the new version;
1. Update the `Version` field if needed;
1. Review and update NuGet package dependencies in `<ItemGroup>` sections;
1. Run tests locally to verify everything works as expected.