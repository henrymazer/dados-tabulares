FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Data/Data.csproj", "src/Data/"]
RUN dotnet restore "src/Api/Api.csproj"

COPY src/ src/
WORKDIR /src/src/Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
