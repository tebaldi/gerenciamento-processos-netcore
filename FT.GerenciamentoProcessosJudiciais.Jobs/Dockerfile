FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY /src/FT.GerenciamentoProcessosJudiciais.Dominio/ FT.GerenciamentoProcessosJudiciais.Dominio/
COPY /src/FT.GerenciamentoProcessosJudiciais.Relatorios/ FT.GerenciamentoProcessosJudiciais.Relatorios/
COPY /src/FT.GerenciamentoProcessosJudiciais.Infra/ FT.GerenciamentoProcessosJudiciais.Infra/
COPY /src/FT.GerenciamentoProcessosJudiciais.Jobs/ FT.GerenciamentoProcessosJudiciais.Jobs/
WORKDIR /src/FT.GerenciamentoProcessosJudiciais.Jobs
RUN dotnet build -c Release -o /app

FROM build AS test
WORKDIR /src
COPY /src/FT.GerenciamentoProcessosJudiciais.TestesUnidade/ FT.GerenciamentoProcessosJudiciais.TestesUnidade/

WORKDIR /src/FT.GerenciamentoProcessosJudiciais.TestesUnidade
RUN dotnet test -c Release --logger "trx;LogFileName=testresults.trx"
FROM build AS publish
WORKDIR /src/FT.GerenciamentoProcessosJudiciais.Jobs
RUN dotnet publish -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FT.GerenciamentoProcessosJudiciais.Jobs.dll"]