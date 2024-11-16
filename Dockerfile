# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Switch to root user to set permissions
USER root

# Create the uploads directory and set permissions
RUN mkdir -p /app/wwwroot/uploads && chmod -R 777 /app/wwwroot/uploads

# Ensure the uploads directory is owned by the app user
RUN mkdir -p /app/wwwroot/uploads && chown -R app:app /app/wwwroot/uploads && chmod -R 777 /app/wwwroot/uploads


# Copy CSV files and set permissions
COPY wwwroot/uploads/customers.csv /app/wwwroot/uploads/customers.csv
COPY wwwroot/uploads/purchase_history.csv /app/wwwroot/uploads/purchase_history.csv
RUN chown app:app /app/wwwroot/uploads/*.csv && chmod 666 /app/wwwroot/uploads/*.csv

# Copy the SQLite database file and set permissions
COPY crms.db /app/crms.db
RUN chown app:app /app/crms.db && chmod 666 /app/crms.db

# Switch back to non-root user
USER app

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["crms2.csproj", "."]
RUN dotnet restore "./././crms2.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./crms2.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish Stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./crms2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final Stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Switch to root user in the final stage to set permissions
USER root

# Ensure the uploads directory and crms.db have the correct permissions
RUN mkdir -p /app/wwwroot/uploads && chmod -R 777 /app/wwwroot/uploads
RUN chown app:app /app/crms.db && chmod 666 /app/crms.db

# Switch back to non-root user
USER app

# Entry point
ENTRYPOINT ["dotnet", "crms2.dll"]
