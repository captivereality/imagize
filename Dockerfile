#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base

# Add font support for Linux.. see: https://stackoverflow.com/questions/60934639/install-fonts-in-linux-container-for-asp-net-core
#RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
#RUN apt-get update; apt-get install -y ttf-mscorefonts-installer fontconfig
#Add these two lines for fonts-liberation instead
RUN apt-get update; apt-get install -y fontconfig fonts-liberation
RUN fc-cache -f -v

WORKDIR /app
EXPOSE 80
# EXPOSE 443 # not required as load balancer takes care of SSL

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src
COPY ["Imagize/Imagize.csproj", "Imagize/"]
COPY ["Imagize.Core/Imagize.Core.csproj", "Imagize.Core/"]
COPY ["Imagize.Abstractions/Imagize.Abstractions.csproj", "Imagize.Abstractions/"]
COPY ["Imagize.Providers.SkiaSharp/Imagize.Providers.SkiaSharp.csproj", "Imagize.Providers.SkiaSharp/"]
RUN dotnet restore "Imagize/Imagize.csproj"
COPY . .
WORKDIR "/src/Imagize"
RUN dotnet build "Imagize.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Imagize.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Imagize.dll"]