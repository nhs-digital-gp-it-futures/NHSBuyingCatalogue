FROM microsoft/dotnet:2.0-sdk

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

WORKDIR /app/IO.Swagger
COPY . /app/IO.Swagger

EXPOSE 5000/tcp

RUN ["dotnet", "restore"]
ENTRYPOINT ["dotnet", "run"]
