# Development stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /src
COPY ["BlogApi/*.csproj", "./BlogApi/"]
RUN dotnet restore "BlogApi/BlogApi.csproj"
COPY . .
# Use dotnet watch for hot reload
ENTRYPOINT ["dotnet", "watch", "--project", "BlogApi/BlogApi.csproj", "run", "--no-launch-profile"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BlogApi/*.csproj", "./BlogApi/"]
RUN dotnet restore "BlogApi/BlogApi.csproj"
COPY . .
RUN dotnet publish "BlogApi/BlogApi.csproj" -c Release -o /app/publish

# Production stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS production
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5077 5078
ENTRYPOINT ["dotnet", "BlogApi.dll"]