param (
    [string]$sourceVersion = "6.0",
    [string]$targetVersion = "9.0",
    [string]$solutionFile = "AspNetCore.JwtAuthentication.sln"
)

$examplesPath = "Examples"
$prefix = "Example.Net"
$sourceProjects = Get-ChildItem -Directory "$examplesPath\$($prefix)$sourceVersion.*"

Write-Host "source version: $sourceVersion"
Write-Host "target version: $targetVersion"

foreach ($src in $sourceProjects) {
    $name = $src.Name
    $newName = $name -replace "Net$sourceVersion", "Net$targetVersion"
    $destPath = Join-Path $examplesPath $newName

    if (Test-Path $destPath) {
        Write-Host "Skipping existing: $newName"
        continue
    }

    $srcPath = Join-Path $examplesPath $name

    # Copying (with no bin/obj/.vs)
    robocopy $srcPath $destPath /E /XD bin obj .vs | Out-Null
    Write-Host "Copied: $name → $newName"

    # Rename .csproj file
    $oldCsproj = Join-Path $destPath "$name.csproj"
    $newCsproj = Join-Path $destPath "$newName.csproj"
    if (Test-Path $oldCsproj) {
        Rename-Item -Path $oldCsproj -NewName "$newName.csproj"
    }

    # Renew .cs & .csproj files
    Get-ChildItem -Recurse -Path $destPath -Include *.cs,*.csproj | ForEach-Object {
        (Get-Content $_.FullName -Raw) `
            -replace "Net$($sourceVersion.Split('.')[0])", "Net$($targetVersion.Split('.')[0])" `
            -replace "$([Regex]::Escape($sourceVersion))", $targetVersion `
            -replace "net$($sourceVersion.Split('.')[0])", "net$($targetVersion.Split('.')[0])" `
            -replace "net$($sourceVersion.Replace('.', ''))", "net$($targetVersion.Replace('.', ''))" `
            -replace "<TargetFramework>.*?</TargetFramework>", "<TargetFramework>net$($targetVersion)</TargetFramework>" `
            | Set-Content $_.FullName
    }


    # Add to .sln
    if (Test-Path $newCsproj) {
        dotnet sln $solutionFile add $newCsproj | Out-Null
        Write-Host "Added to solution: $newName"
    }
}
