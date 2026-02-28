FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FinanceService/FavoriteRates.FinanceService/FavoriteRates.FinanceService.csproj", "src/FinanceService/FavoriteRates.FinanceService/"]
RUN dotnet restore "src/FinanceService/FavoriteRates.FinanceService/FavoriteRates.FinanceService.csproj"
COPY . .
WORKDIR "/src/src/FinanceService/FavoriteRates.FinanceService"
RUN dotnet build "./FavoriteRates.FinanceService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FavoriteRates.FinanceService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FavoriteRates.FinanceService.dll"]
