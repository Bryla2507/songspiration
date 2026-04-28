# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Kopiujemy pliki projektów (struktura folderów musi być zachowana dla referencji)
COPY ["backend/SongSpiration.API/SongSpiration.API.csproj", "backend/SongSpiration.API/"]
COPY ["backend/SongSpiration.BLL/SongSpiration.BLL.csproj", "backend/SongSpiration.BLL/"]
COPY ["backend/SongSpiration.DAL/SongSpiration.DAL.csproj", "backend/SongSpiration.DAL/"]
COPY ["backend/SongSpiration.Models/SongSpiration.Models.csproj", "backend/SongSpiration.Models/"]

# 2. Przywracamy zależności z jawnym wskazaniem architektury linux-x64
RUN dotnet restore "backend/SongSpiration.API/SongSpiration.API.csproj" -r linux-x64

# 3. Kopiujemy resztę kodu backendu
COPY backend/ ./backend/

# 4. Publikujemy aplikację
# Dodajemy -r linux-x64 aby uniknąć błędu 139 (Segmentation Fault) na AWS
WORKDIR "/src/backend/SongSpiration.API"
RUN dotnet publish "SongSpiration.API.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalujemy bibliotekę potrzebną dla SQLite na Linuxie (częsty powód wywalania się apki)
RUN apt-get update && apt-get install -y libsqlite3-0 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# 5. Kopiujemy bazę danych bezpośrednio do folderu /app
# Upewnij się, że plik nazywa się dokładnie SongSpiration.db (wielkość liter!)
COPY backend/SongSpiration.API/SongSpiration.db . 

# Konfiguracja środowiska
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80

# Używamy jawnej nazwy DLL
ENTRYPOINT ["dotnet", "SongSpiration.API.dll"]
