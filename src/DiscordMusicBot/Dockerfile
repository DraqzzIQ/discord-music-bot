FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM base AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DiscordMusicBot/DiscordMusicBot.csproj", "DiscordMusicBot.csproj"]
RUN dotnet restore "DiscordMusicBot.csproj"
COPY src/DiscordMusicBot .
WORKDIR "/src/"
RUN dotnet build "DiscordMusicBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscordMusicBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordMusicBot.dll"]
