FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestMassTransit.GrpcService/TestMassTransit.GrpcService.csproj", "TestMassTransit.GrpcService/"]
COPY ["SampleEvent.ServiceDefaults/SampleEvent.ServiceDefaults.csproj", "SampleEvent.ServiceDefaults/"]
RUN dotnet restore "TestMassTransit.GrpcService/TestMassTransit.GrpcService.csproj"
COPY . .
WORKDIR "/src/TestMassTransit.GrpcService"
RUN dotnet build "TestMassTransit.GrpcService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TestMassTransit.GrpcService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestMassTransit.GrpcService.dll"]
