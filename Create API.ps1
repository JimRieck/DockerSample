cd\
cd Code
cd mystuff
cd MITechCon
cd Code
cd fileupload.api

docker build -f "C:\Code\mystuff\MITechCon\Code\FileUpload.Api\Dockerfile" --force-rm -t fileuploadapi:dev --target base  --build-arg "BUILD_CONFIGURATION=Debug" --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=FileUpload.Api" "C:\Code\mystuff\MITechCon\Code\FileUpload.Api"
docker rm -f fcde2a01f8032e06eb1d81a6c5d23f2b9bc31ad269c0c4c3e3418c3b6b4d3ba3

docker run -dt -v "C:\Users\JimRieck\vsdbg\vs2017u5:/remote_debugger:rw" -v "C:\Users\JimRieck\AppData\Local\AzureFunctionsTools\Releases\4.67.0\cli_x64_Linux:/functions_debugging_cli:ro" -v "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Sdks\Microsoft.Docker.Sdk\tools\TokenService.Proxy\linux-x64\net7.0:/TokenService.Proxy:ro" -v "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Sdks\Microsoft.Docker.Sdk\tools\HotReloadProxy\linux-x64\net7.0:/HotReloadProxy:ro" -v "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\HotReload:/HotReloadAgent:ro" -v "C:\Code\mystuff\MITechCon\Code\FileUpload.Api:/home/site/wwwroot" -v "C:\Code\mystuff\MITechCon\Code\FileUpload.Api:/src/" -v "C:\Users\JimRieck\.nuget\packages\:/.nuget/fallbackpackages2" -v "C:\Program Files (x86)\Microsoft Visual Studio\Shared\NuGetPackages:/.nuget/fallbackpackages" -e "ASPNETCORE_ENVIRONMENT=Development" -e "DOTNET_USE_POLLING_FILE_WATCHER=1" -e "NUGET_PACKAGES=/.nuget/fallbackpackages2" -e "NUGET_FALLBACK_PACKAGES=/.nuget/fallbackpackages;/.nuget/fallbackpackages2" -p 32944:32944 --name FileUpload.Api --entrypoint tail fileuploadapi:dev -f /dev/null

docker logs FileUpload.Api


# Pull the Azure Function image if it doesn't exist
if (-not (docker images -q mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 2>$null)) {
    docker pull mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
}


docker pull mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0

Write-Host "Waiting for things to catch up"
Start-Sleep $timeout

#--build, restore and deploy Doc Gen code ---------------------------+
Write-Host "Preparing code for file upload API container..."
dotnet restore "FileUpload.Api.csproj" 

dotnet build "FileUpload.Api.csproj" -c Release -o /app/build 
dotnet publish "FileUpload.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false  

Write-Host "Waiting for things to catch up"
Start-Sleep $timeout




docker run -e AzureWebJobsScriptRoot=/home/site/wwwroot -e ASPNETCORE_URLS=http://*:34808   -e AzureFunctionsJobHost__Logging__Console__IsEnabled=true -p 34808:80  --name fileupload_API --hostname fileupload_API -d mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 

cd\
cd app
cd publish

Write-Host "Copying code for document generator API container..."

#-- this next line copies the files to the container and usually fails the first time.  Simply, select it and re-run it.
docker cp . fileupload_api:/home/site/wwwroot