#FROM nurananajafova/loanservice:v1
#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS c
#WORKDIR /app   
#
#COPY ["LoanService/LoanService.csproj", "Task8/"]
#
#RUN dotnet restore "LoanService/LoanService.csproj" 
#
#COPY . .
#WORKDIR "/app/LoanService"
#RUN dotnet build "LoanService.csproj" -c Release -o /app/build
#
#from c as publish
#RUN dotnet publish "LoanService.csproj" -c Release -o /app/publish
# 
#
#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
#WORKDIR /app 
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "LoanService.dll"]


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
#COPY ["LoanServiceIntegration.Tests/LoanServiceIntegration.Tests.csproj", "LoanServiceIntegration.Tests/"]
COPY ["LoanService/LoanService.csproj", "LoanService/"]
RUN dotnet restore "LoanService/LoanService.csproj"
COPY . .
WORKDIR "/src/LoanService"
RUN dotnet build "LoanService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoanService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoanService.dll"]
