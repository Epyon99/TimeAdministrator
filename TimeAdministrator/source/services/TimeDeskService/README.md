# LogTiempo Service

## Build and run

### Local
Running LogTiempo service from source locally:
```
# $env:ASPNETCORE_URLS = "http://epyhost:5000"
# dotnet restore
# dotnet run
```

### Dockerfile

The service can be run in a docker container as follows:
```
# cd src/EPY.Services.LogTiempo
# dotnet publish -c Release
# docker build  -f .\Dockerfile -t EPY.services.LogTiempo:1.0.0-alpha1 .
# docker run -p 127.0.0.1:5000:5000 --rm EPY.services.LogTiempo:1.0.0-alpha1
```

