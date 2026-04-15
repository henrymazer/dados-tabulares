# Nominatim Local

This directory contains the versioned local stack for the Nominatim geocoder. It is pinned to `mediagis/nominatim:5.3`, uses a local project directory, and expects a Paraná OSM extract mounted at `infra/nominatim/data/parana-latest.osm.pbf`.

## Setup

```bash
cd infra/nominatim
cp .env.example .env
mkdir -p data project
```

Place the Paraná extract in `data/parana-latest.osm.pbf` before starting the stack. Keep `.env` local; only `.env.example` is versioned.

## Start

```bash
docker compose up -d
```

The service exposes the API on `http://localhost:8088` by default. Adjust `NOMINATIM_HTTP_PORT` in `.env` if that port is already in use.

## Verify

After the import completes, verify the installation with a simple search:

```bash
curl "http://localhost:8088/search?q=Curitiba&format=jsonv2&limit=1"
```

For a deeper sanity check, you can also inspect the container logs or run:

```bash
docker compose exec nominatim nominatim admin --check-database
```

## Notes

- `NOMINATIM_PASSWORD` stays in `.env`, not in version control.
- `FREEZE=true` keeps this dev stack reproducible by avoiding update replication in the first slice.
- To switch the dev extract later, replace `NOMINATIM_PBF_PATH` and the file under `data/`.
