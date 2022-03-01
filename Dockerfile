#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SilverScreen/SilverScreen.csproj", "SilverScreen/"]
RUN dotnet restore "SilverScreen/SilverScreen.csproj"
COPY . .
WORKDIR "/src/SilverScreen"
RUN dotnet build "SilverScreen.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SilverScreen.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SilverScreen.dll"]