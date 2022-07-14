FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/gRPC.WebApi/gRPC.WebApi.csproj", "gRPC.WebApi/"]
RUN dotnet restore "src/gRPC.WebApi/gRPC.WebApi.csproj"
COPY . .
WORKDIR "/src/gRPC.WebApi"
RUN dotnet build "gRPC.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gRPC.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gRPC.WebApi.dll"]
