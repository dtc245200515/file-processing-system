FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/FileProcessing.Shared/FileProcessing.Shared.csproj src/FileProcessing.Shared/
COPY src/FileProcessing.Api/FileProcessing.Api.csproj src/FileProcessing.Api/
RUN dotnet restore src/FileProcessing.Api/FileProcessing.Api.csproj

COPY src/FileProcessing.Shared/ src/FileProcessing.Shared/
COPY src/FileProcessing.Api/ src/FileProcessing.Api/
RUN dotnet publish src/FileProcessing.Api/FileProcessing.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FileProcessing.Api.dll"]
