# TimeLog Type Service

This service provides authorization features.

## Build and run

### Local
Running identity service from source locally:
```
# $env:ASPNETCORE_URLS = "http://epyhost:5010"
# dotnet restore
# dotnet run
```

### Dockerfile

The service can be run in a docker container as follows:
```
# cd src/EPY.Services.TipoLogTiempoService
# dotnet publish -c Release
# docker build  -f .\Dockerfile -t adtd.services.TipoLogTiempo:latest .
# docker run -p 127.0.0.1:5010:5010 --rm adtd.services.TipoLogTiempo:latest
```

