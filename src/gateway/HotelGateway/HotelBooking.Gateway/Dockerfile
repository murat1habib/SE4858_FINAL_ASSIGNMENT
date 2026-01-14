# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["HotelBooking.Gateway/HotelBooking.Gateway.csproj", "HotelBooking.Gateway/"]
RUN dotnet restore "HotelBooking.Gateway/HotelBooking.Gateway.csproj"

# Copy everything and publish
COPY . .
WORKDIR "/src/HotelBooking.Gateway"
RUN dotnet publish "HotelBooking.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HotelBooking.Gateway.dll"]
