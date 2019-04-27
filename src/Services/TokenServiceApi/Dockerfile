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
ENTRYPOINT ["dotnet", "TokenServiceApi.dll"]