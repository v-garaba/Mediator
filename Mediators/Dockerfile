FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

ARG BUILD_CONFIGURATION=Release
ARG BUILD_VERSION=0.0.1

WORKDIR /app

COPY . ./

RUN dotnet publish ./Mediators/Mediators.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:Version=$BUILD_VERSION

FROM mcr.microsoft.com/dotnet/runtime:10.0-preview AS runtime

WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "Mediators.dll"]