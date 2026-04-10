# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore
COPY ["Solder.ContainerManager/Solder.ContainerManager.csproj", "Solder.ContainerManager/"]
COPY ["Solder.Shared/Solder.Shared.csproj", "Solder.Shared/"]
RUN dotnet restore "Solder.ContainerManager/Solder.ContainerManager.csproj"

# Copy everything else and publish
COPY . .
WORKDIR "/src/Solder.ContainerManager"

RUN dotnet publish "./Solder.ContainerManager.csproj" \
    -c Release \
    -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "Solder.ContainerManager.dll"]
