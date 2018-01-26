# TimeLog Type Service

This service provides authorization features.

## Build and run

### Local
Running identity service from source locally:
```
# $env:ASPNETCORE_URLS = "http://epyhost:5020"
# dotnet restore
# dotnet run
```

### Dockerfile

The service can be run in a docker container as follows:
```
# cd src/EPY.Services.UserWorkQuota
# dotnet publish -c Release
# docker build  -f .\Dockerfile -t EPY.services.workquota:1.0.0-alpha1 .
# docker run -p 127.0.0.1:5020:5020 --rm EPY.services.workquota:1.0.0-alpha1
```

