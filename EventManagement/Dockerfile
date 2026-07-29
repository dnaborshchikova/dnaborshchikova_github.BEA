# Stage 1 - build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY dnaborshchikova_github.Bea.EventManagement.slnx ./
COPY dnaborshchikova_github.Bea.EventManagement.WorkerService/*.csproj ./dnaborshchikova_github.Bea.EventManagement.WorkerService/
COPY dnaborshchikova_github.Bea.EventManagement.Core/*.csproj ./dnaborshchikova_github.Bea.EventManagement.Core/
COPY dnaborshchikova_github.Bea.EventManagement.Infrastructure/*.csproj ./dnaborshchikova_github.Bea.EventManagement.Infrastructure/

RUN dotnet restore ./dnaborshchikova_github.Bea.EventManagement.WorkerService/dnaborshchikova_github.Bea.EventManagement.WorkerService.csproj

COPY . .

RUN dotnet publish -c Release -o /app ./dnaborshchikova_github.Bea.EventManagement.WorkerService/dnaborshchikova_github.Bea.EventManagement.WorkerService.csproj

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt-get update && apt-get install -y curl

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "dnaborshchikova_github.Bea.EventManagement.WorkerService.dll"]
