FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . ./
WORKDIR /src/src/Host
RUN dotnet restore Ginecologia.Host.csproj
RUN dotnet publish Ginecologia.Host.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ginecologia.Host.dll"]
