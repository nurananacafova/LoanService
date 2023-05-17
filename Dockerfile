#FROM nurananajafova/loanservice:v1
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
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
