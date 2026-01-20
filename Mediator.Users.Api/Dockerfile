FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

ARG BUILD_CONFIGURATION=Release
ARG BUILD_VERSION=0.0.1

WORKDIR /app

COPY . ./

RUN dotnet publish ./Mediator.Users.Api/Mediator.Users.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:Version=$BUILD_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "Mediator.Users.Api.dll"]