FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY KeyManagement.Api.csproj .
RUN dotnet restore KeyManagement.Api.csproj
COPY . .
RUN dotnet publish KeyManagement.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "KeyManagement.Api.dll"]