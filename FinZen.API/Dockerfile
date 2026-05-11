FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ["FinZen.API.csproj", "./"]
RUN dotnet restore "./FinZen.API.csproj"

COPY . .
RUN dotnet publish "FinZen.API.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FinZen.API.dll"]