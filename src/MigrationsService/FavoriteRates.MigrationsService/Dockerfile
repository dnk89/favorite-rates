FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MigrationsService/FavoriteRates.MigrationsService/FavoriteRates.MigrationsService.csproj", "src/MigrationsService/FavoriteRates.MigrationsService/"]
RUN dotnet restore "src/MigrationsService/FavoriteRates.MigrationsService/FavoriteRates.MigrationsService.csproj"
COPY . .
WORKDIR "/src/src/MigrationsService/FavoriteRates.MigrationsService"
RUN dotnet build "./FavoriteRates.MigrationsService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FavoriteRates.MigrationsService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FavoriteRates.MigrationsService.dll"]
