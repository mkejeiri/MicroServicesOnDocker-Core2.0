FROM microsoft/aspnetcore-build:latest AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./

RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/aspnetcore:latest
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "OrderApi.dll"]