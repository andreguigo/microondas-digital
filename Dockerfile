FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ARG TAG

COPY . .
RUN dotnet restore
RUN dotnet publish "Microondas.Api/Microondas.Api.csproj" -c Release -o /app/publish
RUN echo ${TAG} > /app/publish/versao.txt

FROM base AS final
WORKDIR /src
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microondas.Api.dll"]
