FROM microsoft/dotnet:2.1-sdk

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

WORKDIR /crm
COPY ./GifService.sln /crm
COPY ./Gif.Datastore.CRM /crm/Gif.Datastore.CRM
COPY ./Gif.Service /crm/Gif.Service

RUN dotnet publish -c Release -o /app

WORKDIR /app

EXPOSE 5001/tcp

ENTRYPOINT ["dotnet", "Gif.Service.dll"]
