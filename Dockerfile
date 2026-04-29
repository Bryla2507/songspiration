# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiujemy wszystko z folderu backend do kontenera
COPY backend/ .

# Przywracamy i publikujemy projekt API
# Ścieżka SongSpiration.API/SongSpiration.API.csproj musi być poprawna względem folderu backend/
RUN dotnet publish "SongSpiration.API/SongSpiration.API.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalacja SQLite
RUN apt-get update && apt-get install -y libsqlite3-0 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Jeśli masz bazę danych w folderze projektu API, skopiuj ją
# Zakładam, że jest w backend/SongSpiration.API/SongSpiration.db
COPY SongSpiration.API/SongSpiration.db . 

RUN chmod -R 777 /app

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80

ENTRYPOINT ["dotnet", "SongSpiration.API.dll"]
