# Wykorzystujemy lekki obraz runtime dla .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 1. Instalacja SQLite (wymagane, jeśli używasz SongSpiration.db wewnątrz kontenera)
# Używamy użytkownika root do instalacji pakietów
RUN apt-get update && \
    apt-get install -y libsqlite3-0 && \
    rm -rf /var/lib/apt/lists/*

# 2. Kopiujemy zawartość folderu publish do katalogu roboczego /app
# Ponieważ w deploy.yml kopiujemy Dockerfile do środka folderu publish,
# używamy ".", co oznacza "skopiuj wszystko, co jest obok tego pliku Dockerfile"
COPY . .

# 3. Nadajemy uprawnienia do zapisu (kluczowe dla bazy SQLite i folderu wwwroot/uploads)
# chmod 777 jest drastyczne, ale rozwiązuje problemy z uprawnieniami w EB na start
RUN chmod -R 777 /app

# 4. Konfiguracja zmiennych środowiskowych
# Elastic Beanstalk domyślnie mapuje port 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# 5. Ekspozycja portu
EXPOSE 80

# 6. Uruchomienie aplikacji
# Upewnij się, że nazwa DLL odpowiada nazwie Twojego projektu wyjściowego
ENTRYPOINT ["dotnet", "SongSpiration.API.dll"]
