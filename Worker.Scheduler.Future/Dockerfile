#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Worker.Scheduler.Future/Worker.Scheduler.Future.csproj", "Worker.Scheduler.Future/"]
COPY ZscalerRootCertificate-2048-SHA256.crt ZscalerRootCertificate-2048-SHA256.crt
RUN cat ZscalerRootCertificate-2048-SHA256.crt >> /etc/ssl/certs/ca-certificates.crt
RUN dotnet restore "Worker.Scheduler.Future/Worker.Scheduler.Future.csproj"
COPY . .
WORKDIR "/src/Worker.Scheduler.Future"
RUN dotnet build "Worker.Scheduler.Future.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker.Scheduler.Future.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker.Scheduler.Future.dll"]