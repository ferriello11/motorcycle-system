# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos do projeto
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "TesteTecnico.dll"]
