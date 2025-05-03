FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Prima.Tcp.Test/Prima.Tcp.Test.csproj", "src/Prima.Tcp.Test/"]
RUN dotnet restore "src/Prima.Tcp.Test/Prima.Tcp.Test.csproj"
COPY . .
WORKDIR "/src/src/Prima.Tcp.Test"
RUN dotnet build "./Prima.Tcp.Test.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Prima.Tcp.Test.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Prima.Tcp.Test.dll"]
