#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Worker.Scheduler.Sameday/Worker.Scheduler.Sameday.csproj", "Worker.Scheduler.Sameday/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ZscalerRootCertificate-2048-SHA256.crt ZscalerRootCertificate-2048-SHA256.crt
RUN cat ZscalerRootCertificate-2048-SHA256.crt >> /etc/ssl/certs/ca-certificates.crt
RUN dotnet restore "Worker.Scheduler.Sameday/Worker.Scheduler.Sameday.csproj"
COPY . .
WORKDIR "/src/Worker.Scheduler.Sameday"
RUN dotnet build "Worker.Scheduler.Sameday.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker.Scheduler.Sameday.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker.Scheduler.Sameday.dll"]