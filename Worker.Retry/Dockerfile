#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Worker.Retry/Worker.Retry.csproj", "Worker.Retry/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ZscalerRootCertificate-2048-SHA256.crt ZscalerRootCertificate-2048-SHA256.crt
RUN cat ZscalerRootCertificate-2048-SHA256.crt >> /etc/ssl/certs/ca-certificates.crt
RUN dotnet restore "Worker.Retry/Worker.Retry.csproj"
COPY . .
WORKDIR "/src/Worker.Retry"
RUN dotnet build "Worker.Retry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker.Retry.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker.Retry.dll"]