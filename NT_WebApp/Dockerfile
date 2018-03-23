FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY NTPros.sln ./
COPY NT_WebApp/NT_WebApp.csproj NT_WebApp/
COPY NT_Common/NT_Common.csproj NT_Common/
COPY NT_WeChatUtilities/NT_WeChatUtilities.csproj NT_WeChatUtilities/
COPY NT_CommonConfig/NT_CommonConfig.csproj NT_CommonConfig/
COPY NT_Model/NT_Model.csproj NT_Model/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/NT_WebApp
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NT_WebApp.dll"]
