# Use official ASP.NET runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7198

# Copy build files into the container
COPY ./Publish /app

# Set entry point to run your app
ENTRYPOINT ["dotnet", "API.dll"]
