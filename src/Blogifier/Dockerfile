#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal  AS base
#-arm32v7
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Blogifier/Blogifier.csproj", "src/Blogifier/"]
COPY ["src/Blogifier.Admin/Blogifier.Admin.csproj", "src/Blogifier.Admin/"]
COPY ["src/Blogifier.Shared/Blogifier.Shared.csproj", "src/Blogifier.Shared/"]
COPY ["src/Blogifier.Core/Blogifier.Core.csproj", "src/Blogifier.Core/"]
RUN dotnet restore "src/Blogifier/Blogifier.csproj"
COPY . .
WORKDIR "/src/src/Blogifier"
RUN dotnet build "Blogifier.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blogifier.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blogifier.dll"]
