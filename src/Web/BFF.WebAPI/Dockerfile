ARG ASPNET_VERSION="7.0.0-preview.6-alpine3.16"
ARG SDK_VERSION="7.0.100-preview.6-alpine3.16"
ARG BASE_ADRESS="mcr.microsoft.com/dotnet"

FROM $BASE_ADRESS/aspnet:$ASPNET_VERSION AS base
WORKDIR /app

EXPOSE 80
EXPOSE 443

FROM $BASE_ADRESS/sdk:$SDK_VERSION AS build

COPY ./global.json ./
COPY ./nuget.config ./
COPY ./Directory.Build.props ./

WORKDIR /src

COPY ./src/Web/BFF.WebAPI/*.csproj ./Web/BFF.WebAPI/
COPY ./src/Contracts/*.csproj ./Contracts/

RUN dotnet restore -v m ./Web/BFF.WebAPI

COPY ./src/Web/BFF.WebAPI/. ./Web/BFF.WebAPI/
COPY ./src/Contracts/. ./Contracts/

WORKDIR /src/Web/BFF.WebAPI
RUN dotnet build -c Release --no-restore -v m -o /app/build

FROM build AS publish
RUN dotnet publish -c Release --no-restore -v m -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BFF.WebAPI.dll"]
