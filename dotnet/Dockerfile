FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY ./app ./
ENTRYPOINT ["dotnet", "/App/Release/net8.0/publish/dotnet_pgsql.dll"]