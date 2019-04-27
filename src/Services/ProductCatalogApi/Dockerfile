#build is an alias for the downloaded image
FROM microsoft/aspnetcore-build:latest AS build
#if code folder doesn't docker will create it
WORKDIR /code
#copy project file from my current local p (host) WORKDIR to the current docker container (image)
COPY . .
#one all files are copies to the docker container (image), we call restore
RUN dotnet restore

#We publish, configuration Release, i.e a release build, this images will have source files + the build
#it could easily reach 2GB, the best plan is to have runtime+dll's (a smaller runnable image version of our app) (**)
RUN dotnet publish --output /out/ --configuration Release

####  to solve the issue (**) we use the docker multistage build
#use the smaller image (runtime)
FROM microsoft/aspnetcore:latest
#copy the only the built artifacts from the previous stages (out folder) to the downloaded runtime container inside folder app
COPY --from=build /out /app/
WORKDIR /app
EXPOSE 80/tcp
#this is the entry point for the app to run, dotnet runtime + dll
ENTRYPOINT ["dotnet", "ProductCatalogApi.dll"]

##this is an autogen sample
#FROM microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
#WORKDIR /code
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
#WORKDIR /src
#COPY ["src/Services/ProductCatalogApi/ProductCatalogApi.csproj", "src/Services/ProductCatalogApi/"]
#RUN dotnet restore "src/Services/ProductCatalogApi/ProductCatalogApi.csproj"
#COPY . .
#WORKDIR "/src/src/Services/ProductCatalogApi"
#RUN dotnet build "ProductCatalogApi.csproj" -c Release -o /code
#
#FROM build AS publish
#RUN dotnet publish "ProductCatalogApi.csproj" -c Release -o /code
#
#FROM base AS final
#WORKDIR /code
#COPY --from=publish /code .
#ENTRYPOINT ["dotnet", "ProductCatalogApi.dll"]