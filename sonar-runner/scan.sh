cd ..
mono /usr/lib/sonar-scanner/SonarQube.Scanner.MSBuild.exe begin /s:${PWD}/SonarQube.Analysis.xml /d:sonar.url=${SONARQUBE_URL} /k:"org.sonarqube:bcgov-hets-all-pipeline" /n:"HETS - Pipeline" /v:"1.$BUILD_NUMBER"
dotnet restore
dotnet build
dotnet test
mono /usr/lib/sonar-scanner/SonarQube.Scanner.MSBuild.exe end