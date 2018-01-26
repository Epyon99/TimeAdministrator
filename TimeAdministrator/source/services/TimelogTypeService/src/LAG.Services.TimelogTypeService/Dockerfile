FROM microsoft/aspnetcore:1.1
COPY bin/Release/netcoreapp1.1/publish /app
ENV ASPNETCORE_URLS "http://*:5010"
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_CONTENTROOT "/etc/TipoLogTiemposervice/"
EXPOSE 5010
ENTRYPOINT dotnet /app/EPY.Services.TipoLogTiempoService.dll