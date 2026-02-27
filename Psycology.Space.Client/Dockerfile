FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Psycology.Space.Client/Psycology.Space.Client.csproj", "Psycology.Space.Client/"]
RUN dotnet restore "Psycology.Space.Client/Psycology.Space.Client.csproj"
COPY . .
WORKDIR "/src/Psycology.Space.Client"
RUN dotnet build "./Psycology.Space.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Psycology.Space.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Psycology.Space.Client.dll"]
