FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["backend/backend.csproj", "backend/"]
RUN dotnet restore "backend/backend.csproj"
COPY . .
WORKDIR "/src/backend"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh


COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "backend.dll"]
