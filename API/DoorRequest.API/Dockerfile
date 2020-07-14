FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["DoorRequest.API.csproj","."]
RUN dotnet restore "DoorRequest.API.csproj"
COPY . .
RUN dotnet build "DoorRequest.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoorRequest.API.csproj" -c Release -o /app/publish

FROM base AS final
RUN mkdir /logging
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoorRequest.API.dll"]