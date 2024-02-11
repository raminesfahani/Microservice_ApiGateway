FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Src/MoviesService/MoviesService.csproj", "Src/MoviesService/"]
COPY ["Src/MoviesService/DataModel/DataModel.csproj", "Src/MoviesService/DataModel/"]
COPY ["Src/Events/Events.csproj", "Src/Events/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Src/MoviesService/MoviesService.csproj"
COPY . .
WORKDIR "/src/Src/MoviesService"
RUN dotnet build "MoviesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MoviesService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoviesService.dll"]