FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/UsersService/FavoriteRates.UsersService/FavoriteRates.UsersService.csproj", "src/UsersService/FavoriteRates.UsersService/"]
RUN dotnet restore "src/UsersService/FavoriteRates.UsersService/FavoriteRates.UsersService.csproj"
COPY . .
WORKDIR "/src/src/UsersService/FavoriteRates.UsersService"
RUN dotnet build "./FavoriteRates.UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FavoriteRates.UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FavoriteRates.UsersService.dll"]
