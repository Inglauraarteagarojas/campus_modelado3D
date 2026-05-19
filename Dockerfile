FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

RUN apt-get update \
    && apt-get install -y git git-lfs \
    && git lfs install \
    && rm -rf /var/lib/apt/lists/*

COPY . ./

RUN git lfs pull || true

RUN dotnet restore ./CampusVR.csproj
RUN dotnet publish ./CampusVR.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "CampusVR.dll"]
