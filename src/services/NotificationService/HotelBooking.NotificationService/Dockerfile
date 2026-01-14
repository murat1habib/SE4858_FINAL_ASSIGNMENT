FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/services/NotificationService/HotelBooking.NotificationService/HotelBooking.NotificationService.csproj
RUN dotnet publish src/services/NotificationService/HotelBooking.NotificationService/HotelBooking.NotificationService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HotelBooking.NotificationService.dll"]
