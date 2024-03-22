cd\
cd Code
cd mystuff
cd MITechCon
cd Code
cd fileuploader.ui

Docker build -t blazor-server-docker .

Docker run -p 8080:80 -d blazor-server-docker