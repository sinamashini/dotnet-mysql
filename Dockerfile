
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source


COPY *.csproj .
RUN dotnet restore


COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "MyWebApi.dll"]
