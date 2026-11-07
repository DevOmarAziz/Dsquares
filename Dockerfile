FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# restore dependencies
COPY ["LoyaltyApi/LoyaltyApi.csproj", "LoyaltyApi/"]
RUN dotnet restore "LoyaltyApi/LoyaltyApi.csproj"

# publish
COPY ["LoyaltyApi/", "LoyaltyApi/"]
RUN dotnet publish "LoyaltyApi/LoyaltyApi.csproj" -c Release -o /app/publish 

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LoyaltyApi.dll"]
