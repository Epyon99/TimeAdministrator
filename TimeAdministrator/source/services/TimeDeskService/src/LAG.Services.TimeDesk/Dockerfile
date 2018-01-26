FROM microsoft/aspnetcore:1.1
COPY bin/release/netcoreapp1.1/publish /app
ENV ASPNETCORE_URLS "http://*:5000"
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_CONTENTROOT "/etc/LogDeTiempo/"
EXPOSE 5000
ENTRYPOINT dotnet /app/EPY.Services.LogTiempo.dll