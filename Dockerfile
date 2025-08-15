FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Docker
WORKDIR /src
COPY ["FitnessTracker.Api/FitnessTracker.Api.csproj", "FitnessTracker.Api/"]
COPY ["FitnessTracker.DataAccess/FitnessTracker.DataAccess.csproj", "FitnessTracker.DataAccess/"]
COPY ["FitnessTracker.Domain/FitnessTracker.Domain.csproj", "FitnessTracker.Domain/"]
COPY ["FitnessTracker.Infrastructure/FitnessTracker.Infrastructure.csproj", "FitnessTracker.Infrastructure/"]
COPY ["FitnessTracker.Business/FitnessTracker.Business.csproj", "FitnessTracker.Business/"]
RUN dotnet restore "FitnessTracker.Api/FitnessTracker.Api.csproj"
COPY . .
WORKDIR "/src/FitnessTracker.Api"
RUN dotnet build "FitnessTracker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Docker
RUN dotnet publish "FitnessTracker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessTracker.Api.dll"]
