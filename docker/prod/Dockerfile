FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

COPY web /web

# HTTP
EXPOSE 5000
WORKDIR /web
ENTRYPOINT ["dotnet", "PlumPack.IdentityServer.Web.dll"]