FROM microsoft/aspnetcore-build:latest AS build
WORKDIR /app
# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish --output /out/ --configuration Release

# Build runtime images
FROM microsoft/aspnetcore:latest
WORKDIR /app
COPY --from=build /out /app/
EXPOSE 80/tcp
WORKDIR /app
ENTRYPOINT ["dotnet", "WebMvc.dll"]