FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY NTPros.sln ./
COPY NT_MQPublisher/NT_MQPublisher.csproj NT_MQPublisher/
COPY NT_Common/NT_Common.csproj NT_Common/
COPY NT_Model/NT_Model.csproj NT_Model/
COPY NT_CommonConfig/NT_CommonConfig.csproj NT_CommonConfig/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/NT_MQPublisher
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NT_MQPublisher.dll"]
