# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/services/HotelService/HotelBooking.HotelService/HotelBooking.HotelService.csproj
RUN dotnet publish src/services/HotelService/HotelBooking.HotelService/HotelBooking.HotelService.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HotelBooking.HotelService.dll"]
