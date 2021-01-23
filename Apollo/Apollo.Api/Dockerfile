#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Apollo.Api/Apollo.Api.csproj", "Apollo.Api/"]
COPY ["Apollo.Core/Apollo.Core.csproj", "Apollo.Core/"]
COPY ["Apollo.Core.Validation/Apollo.Core.Validation.csproj", "Apollo.Core.Validation/"]
COPY ["Apollo.Core.Dto/Apollo.Core.Dto.csproj", "Apollo.Core.Dto/"]
COPY ["Apollo.Util/Apollo.Util.csproj", "Apollo.Util/"]
COPY ["Apollo.Core.Exception/Apollo.Core.Exception.csproj", "Apollo.Core.Exception/"]
COPY ["Apollo.Domain/Apollo.Domain.csproj", "Apollo.Domain/"]
COPY ["Apollo.Persistence.Attributes/Apollo.Persistence.Attributes.csproj", "Apollo.Persistence.Attributes/"]
COPY ["Apollo.Payment.Adapter/Apollo.Payment.Adapter.csproj", "Apollo.Payment.Adapter/"]
COPY ["Apollo.Payment.Domain/Apollo.Payment.Domain.csproj", "Apollo.Payment.Domain/"]
COPY ["Apollo.Payment/Apollo.Payment.csproj", "Apollo.Payment/"]
COPY ["Apollo.Core.External/Apollo.Core.External.csproj", "Apollo.Core.External/"]
COPY ["Apollo.UnitOfWork/Apollo.UnitOfWork.csproj", "Apollo.UnitOfWork/"]
COPY ["Apollo.Repository/Apollo.Repository.csproj", "Apollo.Repository/"]
COPY ["Apollo.Persistence/Apollo.Persistence.csproj", "Apollo.Persistence/"]
COPY ["Apollo.Persistence.Exception/Apollo.Persistence.Exception.csproj", "Apollo.Persistence.Exception/"]
RUN dotnet restore "Apollo.Api/Apollo.Api.csproj"
COPY . .
WORKDIR "/src/Apollo.Api"
RUN dotnet build "Apollo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Apollo.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Apollo.Api.dll"]
