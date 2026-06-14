# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["EmployeeManagement.Api/EmployeeManagement.Api.csproj", "EmployeeManagement.Api/"]
COPY ["EmployeeManagement.Core/EmployeeManagement.Core.csproj", "EmployeeManagement.Core/"]

RUN dotnet restore "EmployeeManagement.Api/EmployeeManagement.Api.csproj"

COPY . .

WORKDIR "/src/EmployeeManagement.Api"

RUN dotnet publish "EmployeeManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]