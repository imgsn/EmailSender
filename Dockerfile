FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/EmailService.Worker/EmailService.Worker.csproj", "src/EmailService.Worker/"]
COPY ["src/EmailService.Api/EmailService.Api.csproj", "src/EmailService.Api/"]
COPY ["src/EmailService.Core/EmailService.Core.csproj", "src/EmailService.Core/"]
COPY ["src/EmailService.Data/EmailService.Data.csproj", "src/EmailService.Data/"]
RUN dotnet restore "src/EmailService.Worker/EmailService.Worker.csproj"
RUN dotnet restore "src/EmailService.Api/EmailService.Api.csproj"
COPY . .
WORKDIR "/src/src/EmailService.Worker"
RUN dotnet build "EmailService.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmailService.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailService.Worker.dll"]