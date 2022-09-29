FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY /AlphaAPI/AlphaAPI.csproj .
RUN dotnet restore "AlphaAPI.csproj"
COPY /AlphaAPI .
RUN dotnet build "AlphaAPI.csproj" -c Release

FROM build AS publish
WORKDIR "/src"
RUN dotnet publish "AlphaAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AlphaAPI.dll"]