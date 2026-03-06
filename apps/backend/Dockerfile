# ============================================================
# Stage 1: Build
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Önce sadece csproj dosyalarını kopyala — Docker layer cache için
# (bağımlılıklar değişmediği sürece restore tekrar çalışmaz)
COPY ["src/WebApi/WebApi.csproj",         "src/WebApi/"]
COPY ["src/Core/Application/Application.csproj", "src/Core/Application/"]
COPY ["src/Core/Domain/Domain.csproj",    "src/Core/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

RUN dotnet restore "src/WebApi/WebApi.csproj"

# Tüm kaynak kodunu kopyala ve publish et
COPY . .
WORKDIR /src/src/WebApi
RUN dotnet publish "WebApi.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ============================================================
# Stage 2: Runtime
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Güvenlik: root olmayan kullanıcı ile çalıştır
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Publish çıktısını kopyala
COPY --from=build /app/publish .

# Firebase Service Account secrets klasörü için volume mount noktası
RUN mkdir -p /app/secrets && chown appuser:appgroup /app/secrets

USER appuser

# ASP.NET Core'un dinleyeceği port
EXPOSE 8080

# Sağlık kontrolü
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WebApi.dll"]
