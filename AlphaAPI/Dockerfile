FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AlphaAPI/AlphaAPI.csproj", "AlphaAPI/"]
RUN dotnet restore AlphaAPI/AlphaAPI.csproj
COPY . .
WORKDIR "/src/AlphaAPI"
RUN dotnet build "AlphaAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlphaAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlphaAPI.dll"]