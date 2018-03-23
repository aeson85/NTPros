FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY NTPros.sln ./
COPY NT_Cache/NT_Cache.csproj NT_Cache/
COPY NT_CommonConfig/NT_CommonConfig.csproj NT_CommonConfig/
COPY NT_Common/NT_Common.csproj NT_Common/
COPY NT_Model/NT_Model.csproj NT_Model/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/NT_Cache
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NT_Cache.dll"]
