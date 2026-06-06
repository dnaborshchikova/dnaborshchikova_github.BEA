# Stage 1 - build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY dnaborshchikova_github.Bea.Collector.sln ./
COPY dnaborshchikova_github.Bea.Collector.WorkerService/*.csproj ./dnaborshchikova_github.Bea.Collector.WorkerService/
COPY dnaborshchikova_github.Bea.Collector.Core/*.csproj ./dnaborshchikova_github.Bea.Collector.Core/
COPY dnaborshchikova_github.Bea.Collector.DataAccess/*.csproj ./dnaborshchikova_github.Bea.Collector.DataAccess/
COPY dnaborshchikova_github.Bea.Collector.Generator/*.csproj ./dnaborshchikova_github.Bea.Collector.Generator/
COPY dnaborshchikova_github.Bea.Collector.Parser/*.csproj ./dnaborshchikova_github.Bea.Collector.Parser/
COPY dnaborshchikova_github.Bea.Collector.Processor/*.csproj ./dnaborshchikova_github.Bea.Collector.Processor/
COPY dnaborshchikova_github.Bea.Collector.Sender/*.csproj ./dnaborshchikova_github.Bea.Collector.Sender/
COPY dnaborshchikova_github.Bea.Collector.Common/*.csproj ./dnaborshchikova_github.Bea.Collector.Common/

RUN dotnet restore ./dnaborshchikova_github.Bea.Collector.WorkerService/dnaborshchikova_github.Bea.Collector.WorkerService.csproj

COPY . .

RUN dotnet publish -c Release -o /app ./dnaborshchikova_github.Bea.Collector.WorkerService/dnaborshchikova_github.Bea.Collector.WorkerService.csproj

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "dnaborshchikova_github.Bea.Collector.WorkerService.dll"]
