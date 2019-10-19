FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

COPY web /web

# HTTP
EXPOSE 5000
# HTTPS
EXPOSE 5001
WORKDIR /web
ENTRYPOINT ["dotnet", "PlumPack.IdentityServer.Web.dll"]