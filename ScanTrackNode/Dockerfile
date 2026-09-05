# Din Dockerfile — fyll i de saknade raderna
# Använd multi-stage build precis som på lektionen

# Stage 1: Bygg
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiera projektfilen och kör restore
COPY ScanTrackNode/ScanTrackNode.csproj ScanTrackNode/
COPY data/ data/

RUN dotnet restore ScanTrackNode/ScanTrackNode.csproj

# Kopiera resten av koden och publicera
COPY ScanTrackNode/ ScanTrackNode/

RUN dotnet publish ScanTrackNode/ScanTrackNode.csproj \
    -c Release \
    -o /app/publish



# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Kopiera det publicerade projektet från build-steget
COPY --from=build /app/publish .

# Kopiera CSV-filen i runtime-imagen 
COPY --from=build /src/data/cities.csv /app/data/cities.csv

# Porten appen lyssnar på inuti containern
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Starta appen
ENTRYPOINT ["dotnet", "ScanTrackNode.dll"]
