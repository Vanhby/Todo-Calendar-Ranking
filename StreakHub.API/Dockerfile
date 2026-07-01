# =========================
# Stage 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

# Thư mục làm việc trong container
WORKDIR /src

# Copy file project trước để tận dụng cache
COPY StreakHub.API.csproj .

# Restore các package NuGet
RUN dotnet restore

# Copy toàn bộ source
COPY . .

# Publish project
RUN dotnet publish -c Release -o /app/publish --no-restore

# =========================
# Stage 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "StreakHub.API.dll"]
