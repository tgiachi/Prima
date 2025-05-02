# Base image for the final container
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app

# Install curl for healthcheck
RUN apk add --no-cache curl

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["./", "./"]
COPY . .
WORKDIR "/src/src/Prima.Server"
RUN dotnet restore "Prima.Server.csproj" -a $TARGETARCH
RUN dotnet build "Prima.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH
# Publish image with single file
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
RUN dotnet publish "Prima.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    -a $TARGETARCH \
    -p:PublishSingleFile=true \
    -p:PublishReadyToRun=true


RUN rm .git/ -Rf
RUN rm .github/ -Rf
RUN rm .gitignore -Rf
RUN rm .dockerignore -Rf
RUN rm ./assets -Rf
RUN rm src -Rf
# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV PRIMA_SERVER_ROOT=/app

# Set non-root user for better security
# Creating user inside container rather than using $APP_UID since Alpine uses different user management
RUN adduser -D -h /app prima && \
    chown -R prima:prima /app

# Create directories for data persistence
RUN mkdir -p /app/data /app/logs /app/scripts && \
    chown -R prima:prima /app/data /app/logs /app/scripts

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:${PRIMA_HTTP_PORT:-23000}/api/v1/status/health || exit 1

USER prima
ENV ASPNETCORE_URLS=""
ENTRYPOINT ["./Prima.Server"]
