FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/REST.WebApi/REST.WebApi.csproj", "REST.WebApi/"]
RUN dotnet restore "src/REST.WebApi/REST.WebApi.csproj"
COPY . .
WORKDIR "/src/REST.WebApi"
RUN dotnet build "REST.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "REST.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "REST.WebApi.dll"]
