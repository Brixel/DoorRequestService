FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
#RUN apk add nodejs
#RUN apk add npm
RUN apt-get update && apt-get install -y python3
WORKDIR /app
COPY . ./
RUN dotnet workload restore
#RUN npm --prefix Web install
RUN dotnet build "Web/Web.csproj" -c Release
RUN dotnet publish "Web/Web.csproj" -c Release -o output
RUN rm output/wwwroot/appsettings.json.br && rm output/wwwroot/appsettings.json.gz

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build-env /app/output/wwwroot .
COPY Web/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80