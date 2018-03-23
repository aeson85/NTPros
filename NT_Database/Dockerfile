FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY NTPros.sln ./
COPY NT_Database/NT_Database.csproj NT_Database/
COPY NT_Common/NT_Common.csproj NT_Common/
COPY NT_Model/NT_Model.csproj NT_Model/
COPY NT_CommonConfig/NT_CommonConfig.csproj NT_CommonConfig/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/NT_Database
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NT_Database.dll"]
