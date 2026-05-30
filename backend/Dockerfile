FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY UserSearch.sln .
COPY UserSearch.Api/UserSearch.Api.csproj UserSearch.Api/
RUN dotnet restore UserSearch.Api/UserSearch.Api.csproj
COPY UserSearch.Api/ UserSearch.Api/
RUN dotnet publish UserSearch.Api/UserSearch.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UserSearch.Api.dll"]
