openssl req -new -x509 -newkey rsa:2048 -keyout NHSD.GPITF.BuyingCatalog.key -out NHSD.GPITF.BuyingCatalog.cer -days 3650 -subj /CN=NHSD.GPITF.BuyingCatalog
openssl pkcs12 -export -out NHSD.GPITF.BuyingCatalog.pfx -inkey NHSD.GPITF.BuyingCatalog.key -in NHSD.GPITF.BuyingCatalog.cer
