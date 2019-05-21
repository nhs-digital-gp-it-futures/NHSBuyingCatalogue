FROM microsoft/dotnet-framework:4.7.2-sdk as build

WORKDIR /api/NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App
COPY . .

WORKDIR /api
RUN nuget restore NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App\NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App.csproj -PackagesDirectory packages

WORKDIR /api/NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App
RUN msbuild /p:Configuration=Release


FROM microsoft/dotnet-framework:4.7.2-sdk as runtime

WORKDIR /app
COPY --from=build /api/NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App/bin/Release/. ./

ENTRYPOINT ["NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App.exe"]
