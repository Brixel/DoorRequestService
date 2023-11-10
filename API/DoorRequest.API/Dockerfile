FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "API/DoorRequest.API/DoorRequest.API.csproj"
RUN dotnet build "API/DoorRequest.API/DoorRequest.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API/DoorRequest.API/DoorRequest.API.csproj" -c Release -o /app/publish

FROM base AS final
RUN mkdir /logging
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoorRequest.API.dll"]
