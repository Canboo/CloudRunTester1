FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Copy everything
WORKDIR /App

COPY . .
RUN dotnet restore
RUN dotnet publish -r linux-x64 -o out --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN groupadd -r appuser && useradd -r -g appuser appuser

WORKDIR /App
COPY --from=build /App/out .

USER appuser

ENTRYPOINT ["dotnet", "api.dll"]