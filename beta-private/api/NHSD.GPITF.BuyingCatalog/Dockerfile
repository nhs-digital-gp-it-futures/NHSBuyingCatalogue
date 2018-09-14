FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 8000

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY NHSD.GPITF.BuyingCatalog/NHSD.GPITF.BuyingCatalog.csproj NHSD.GPITF.BuyingCatalog/
COPY NHSD.GPITF.BuyingCatalog.Datastore.Database/NHSD.GPITF.BuyingCatalog.Datastore.Database.csproj NHSD.GPITF.BuyingCatalog.Datastore.Database/
COPY NHSD.GPITF.BuyingCatalog.Datastore.Database.Tests/NHSD.GPITF.BuyingCatalog.Datastore.Database.Tests.csproj NHSD.GPITF.BuyingCatalog.Datastore.Database.Tests/
COPY NHSD.GPITF.BuyingCatalog.Datastore.CRM/NHSD.GPITF.BuyingCatalog.Datastore.CRM.csproj NHSD.GPITF.BuyingCatalog.Datastore.CRM/
COPY NHSD.GPITF.BuyingCatalog.Logic/NHSD.GPITF.BuyingCatalog.Logic.csproj NHSD.GPITF.BuyingCatalog.Logic/
COPY NHSD.GPITF.BuyingCatalog.Logic.Tests/NHSD.GPITF.BuyingCatalog.Logic.Tests.csproj NHSD.GPITF.BuyingCatalog.Logic.Tests/
COPY NHSD.GPITF.BuyingCatalog.Tests/NHSD.GPITF.BuyingCatalog.Tests.csproj NHSD.GPITF.BuyingCatalog.Tests/
RUN dotnet restore

COPY . .

WORKDIR /src/NHSD.GPITF.BuyingCatalog
RUN dotnet build -c Release -o /app

WORKDIR /src/NHSD.GPITF.BuyingCatalog.Datastore.Database
RUN dotnet build -c Release -o /app

WORKDIR /src/NHSD.GPITF.BuyingCatalog.Datastore.Database.Tests
RUN dotnet build -c Release -o /app
RUN dotnet test -c Release

WORKDIR /src/NHSD.GPITF.BuyingCatalog.Datastore.CRM
RUN dotnet build -c Release -o /app

WORKDIR /src/NHSD.GPITF.BuyingCatalog.Logic
RUN dotnet build -c Release -o /app

WORKDIR /src/NHSD.GPITF.BuyingCatalog.Logic.Tests
RUN dotnet build -c Release -o /app
RUN dotnet test -c Release

WORKDIR /src/NHSD.GPITF.BuyingCatalog.Tests
RUN dotnet build -c Release -o /app
RUN dotnet test -c Release

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NHSD.GPITF.BuyingCatalog.dll"]
