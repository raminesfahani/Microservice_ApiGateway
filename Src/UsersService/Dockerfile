FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Src/UsersService/UsersService.csproj", "Src/UsersService/"]
COPY ["Src/UsersService/DataModel/DataModel.csproj", "Src/UsersService/DataModel/"]
COPY ["Src/Events/Events.csproj", "Src/Events/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Src/UsersService/UsersService.csproj"
COPY . .
WORKDIR "/src/Src/UsersService"
RUN dotnet build "UsersService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UsersService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.dll"]