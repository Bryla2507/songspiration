# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiujemy pliki projektów, aby zcache'ować restore
COPY ["backend/SongSpiration.API/SongSpiration.API.csproj", "backend/SongSpiration.API/"]
COPY ["backend/SongSpiration.BLL/SongSpiration.BLL.csproj", "backend/SongSpiration.BLL/"]
COPY ["backend/SongSpiration.DAL/SongSpiration.DAL.csproj", "backend/SongSpiration.DAL/"]
COPY ["backend/SongSpiration.Models/SongSpiration.Models.csproj", "backend/SongSpiration.Models/"]

RUN dotnet restore "backend/SongSpiration.API/SongSpiration.API.csproj"

# Kopiujemy resztę kodu backendu
COPY backend/ ./backend/

# Budujemy API
WORKDIR "/src/backend/SongSpiration.API"
RUN dotnet publish "SongSpiration.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Ustawiamy port na 80 (EB tego oczekuje domyślnie)
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Kopiujemy bazę danych, jeśli musi być wewnątrz kontenera (widzę SongSpiration.db na screenie)
# UWAGA: Dane w kontenerze znikną przy restarcie. Docelowo użyj AWS RDS (Postgres/SQL Server).
COPY backend/SongSpiration.API/SongSpiration.db . 

ENTRYPOINT ["dotnet", "SongSpiration.API.dll"]
