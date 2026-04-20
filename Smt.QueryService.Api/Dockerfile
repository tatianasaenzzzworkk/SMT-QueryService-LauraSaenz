# STAGE 1: BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Smt.QueryService.Api/Smt.QueryService.Api.csproj Smt.QueryService.Api/
RUN dotnet restore Smt.QueryService.Api/Smt.QueryService.Api.csproj

COPY Smt.QueryService.Api/ Smt.QueryService.Api/
WORKDIR /src/Smt.QueryService.Api
RUN dotnet publish Smt.QueryService.Api.csproj -c Release -o /app/publish

# STAGE 2: RUN
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Smt.QueryService.Api.dll"]
