#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AlphaApiService/AlphaApiService.csproj", "AlphaApiService/"]
#COPY ["AlphaApiService/Configuration/kafka.properties", "AlphaApiService/"]
RUN dotnet restore "AlphaApiService/AlphaApiService.csproj"
COPY . .
WORKDIR "/src/AlphaApiService"
RUN dotnet build "AlphaApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlphaApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlphaApiService.dll"]