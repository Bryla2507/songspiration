FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalacja SQLite (jeśli faktycznie go potrzebujesz wewnątrz kontenera)
RUN apt-get update && apt-get install -y libsqlite3-0 && rm -rf /var/lib/apt/lists/*

# Kopiujemy WSZYSTKO z bieżącego kontekstu (czyli zawartość folderu publish) do /app
COPY . .

# Nadajemy uprawnienia do bazy danych SQLite
RUN chmod -R 777 /app

# EB domyślnie nasłuchuje na porcie 80 lub 8080
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80

ENTRYPOINT ["dotnet", "SongSpiration.API.dll"]
