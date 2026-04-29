FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalacja SQLite
RUN apt-get update && apt-get install -y libsqlite3-0 && rm -rf /var/lib/apt/lists/*

# Kopiujemy gotowe pliki z folderu publish (który stworzy GitHub)
COPY publish/ .

# Nadajemy uprawnienia
RUN chmod -R 777 /app

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80

ENTRYPOINT ["dotnet", "SongSpiration.API.dll"]
