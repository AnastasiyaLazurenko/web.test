dotnet restore WebSite.sln
dotnet build WebSite.sln --configuration Release --no-restore
cd ./WebSite/bin/Release/net8.0
WebSite.exe