﻿ARG ASPNET_VERSION="7.0.0-preview.7"
ARG SDK_VERSION="7.0.100-preview.7"
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

COPY ./src/Services/GrpcService/*.csproj ./Services/GrpcService/
COPY ./src/Services/Data/*.csproj ./Services/Data/
COPY ./src/Contracts/*.csproj ./Contracts/

RUN dotnet restore -v m ./Services/GrpcService/

COPY ./src/Services/GrpcService/. ./Services/GrpcService/
COPY ./src/Services/Data/. ./Services/Data/
COPY ./src/Contracts/. ./Contracts/

WORKDIR /src/Services/GrpcService

RUN dotnet build -c Release --no-restore -v m -o /app/build 

FROM build AS publish
RUN dotnet dev-certs https --trust
RUN dotnet publish -c Release --no-restore -v m -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["dotnet", "GrpcService.dll"]