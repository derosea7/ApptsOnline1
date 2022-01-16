#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Appts.Web.Ui.Scheduler/Appts.Web.Ui.Scheduler.csproj", "Appts.Web.Ui.Scheduler/"]
COPY ["Appts.Models.Domain/Appts.Models.Domain.csproj", "Appts.Models.Domain/"]
COPY ["Appts.Web.ApiClient/Appts.Web.ApiClient.csproj", "Appts.Web.ApiClient/"]
COPY ["Appts.Models.Rest/Appts.Models.Rest.csproj", "Appts.Models.Rest/"]
COPY ["Appts.Models.Api/Appts.Models.Api.csproj", "Appts.Models.Api/"]
COPY ["Appts.Models.Document/Appts.Models.Document.csproj", "Appts.Models.Document/"]
COPY ["Appts.Extensions/Appts.Extensions.csproj", "Appts.Extensions/"]
COPY ["Appts.Dal.RedisCache/Appts.Dal.RedisCache.csproj", "Appts.Dal.RedisCache/"]
COPY ["Appts.Web.Api.Scheduler/Appts.Web.Api.Scheduler.csproj", "Appts.Web.Api.Scheduler/"]
COPY ["Appts.Models.Docs/Appts.Models.Docs.csproj", "Appts.Models.Docs/"]
COPY ["Appts.Messaging.ServiceBus/Appts.Messaging.ServiceBus.csproj", "Appts.Messaging.ServiceBus/"]
COPY ["Appts.Models.SendGrid/Appts.Models.SendGrid.csproj", "Appts.Models.SendGrid/"]
COPY ["Appts.Dal.Cosmos/Appts.Dal.Cosmos.csproj", "Appts.Dal.Cosmos/"]
COPY ["Appts.Common.Constants/Appts.Common.Constants.csproj", "Appts.Common.Constants/"]
COPY ["Appts.Models.View/Appts.Models.View.csproj", "Appts.Models.View/"]
RUN dotnet restore "Appts.Web.Ui.Scheduler/Appts.Web.Ui.Scheduler.csproj"
COPY . .
WORKDIR "/src/Appts.Web.Ui.Scheduler"
RUN dotnet build "Appts.Web.Ui.Scheduler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Appts.Web.Ui.Scheduler.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Appts.Web.Ui.Scheduler.dll"]