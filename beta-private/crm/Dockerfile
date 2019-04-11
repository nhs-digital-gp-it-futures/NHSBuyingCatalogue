FROM microsoft/dotnet:2.1-sdk

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

WORKDIR /crm
COPY ./GifService.sln /crm
COPY ./src/Gif.Datastore.CRM /crm/src/Gif.Datastore.CRM
COPY ./src/Gif.Service /crm/src/Gif.Service
COPY ./lib/Samc4.CipherUtil.dll /crm/lib/Samc4.CipherUtil.dll

RUN dotnet publish -c Release -o /app

WORKDIR /app

EXPOSE 5001/tcp

ENTRYPOINT ["dotnet", "Gif.Service.dll"]
