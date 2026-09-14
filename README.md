# Egyetemi Kulcs- és Teremnyilvántartó Rendszer

Projekt Labor 2026/27/1

## Gyors indítás
- API (Swagger UI): http://localhost:8081/swagger

Előfeltétel: Docker Desktop

​```bash
cp .env.example .env
docker compose up --build
​```

- Frontend: http://localhost:4200
- API (OpenAPI JSON): http://localhost:8081/openapi/v1.json


## Adatbázis

MSSQL 2022 konténerben, a hoston **`localhost,1434`** (belül 1433 — azért nem az,
mert sok gépen fut lokális SQL Server, ami ütközne).

SSMS: `localhost,1434`, SQL Server Authentication, user `sa`, jelszó a `.env`-ből,
Trust Server Certificate bepipálva. A vessző+port kötelező, `localhost` önmagában
a lokális példányhoz megy.

Migráció hostról (`appsettings.json`-ben `localhost,1434` kell):

```bash
cd backend
dotnet ef database update

## Architektúra

TODO