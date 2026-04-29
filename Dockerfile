# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Kopiujemy pliki projektów (.csproj) - zachowując strukturę folderów
COPY ["backend/SongSpiration.API/SongSpiration.API.csproj", "backend/SongSpiration.API/"]
COPY ["backend/SongSpiration.BLL/SongSpiration.BLL.csproj", "backend/SongSpiration.BLL/"]
COPY ["backend/SongSpiration.DAL/SongSpiration.DAL.csproj", "backend/SongSpiration.DAL/"]
COPY ["backend/SongSpiration.Models/SongSpiration.Models.csproj", "backend/SongSpiration.Models/"]

# 2. Restore dla architektury x64
RUN dotnet restore "backend/SongSpiration.API/SongSpiration.API.csproj" -r linux-x64

# 3. Kopiujemy całą zawartość backendu
COPY backend/ ./backend/

# 4. Publikacja pod Linux x64
WORKDIR "/src/backend/SongSpiration.API"
RUN dotnet publish "SongSpiration.API.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 5. Instalacja bibliotek SQLite i nadanie uprawnień do zapisu
RUN apt-get update && apt-get install -y libsqlite3-0 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# 6. Kopiujemy bazę danych
# Kopiujemy tylko plik .db, pliki -shm i -wal zostaną wygenerowane automatycznie przez silnik
COPY backend/SongSpiration.API/SongSpiration.db . 

# KLUCZOWE: Nadajemy uprawnienia do folderu /app, aby SQLite mógł tworzyć pliki tymczasowe (-shm, -wal)
RUN chmod -R 777 /app

# Konfiguracja środowiska
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80

# Uruchomienie
ENTRYPOINT ["dotnet", "SongSpiration.API.dll"]
