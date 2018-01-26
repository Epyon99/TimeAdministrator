FROM microsoft/aspnetcore:1.1
COPY bin/release/netcoreapp1.1/publish /app
ENV ASPNETCORE_URLS "http://*:5020"
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_CONTENTROOT "/etc/CuotaDeTrabajo/"
EXPOSE 5020
ENTRYPOINT dotnet /app/EPY.Services.UserWorkQuota.dll