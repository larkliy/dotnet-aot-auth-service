FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN apt-get update && apt-get install -y --no-install-recommends clang zlib1g-dev

COPY ["AuthService.csproj", "./"]
RUN dotnet restore "./AuthService.csproj" -r linux-x64

COPY . .

RUN dotnet publish "./AuthService.csproj" \
    -c Release \
    -r linux-x64 \
    --no-restore \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-bookworm-slim
WORKDIR /app
EXPOSE 8080

RUN useradd -m -u 1000 appuser
USER appuser

COPY --from=build /app/publish .

ENTRYPOINT ["./AuthService"]