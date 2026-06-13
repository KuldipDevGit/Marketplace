# Catalog API — generated TypeScript/Angular client

`catalog-api.client.ts` is a **generated** typed Angular client for the Catalog API. It is produced
from the contract-first OpenAPI document [`openapi/catalog.v1.yaml`](../../../openapi/catalog.v1.yaml)
with NSwag (API-1, FE-6). **The contract is the source of truth — do not hand-edit the client.**

## Regenerate

Install the NSwag CLI once (requires the .NET SDK):

    dotnet tool install --global NSwag.ConsoleCore --version 14.2.0

Then, from the repository root:

    nswag openapi2tsclient /input:openapi/catalog.v1.yaml /output:web/libs/api-clients/catalog-api.client.ts /template:Angular /className:CatalogApiClient /generateClientInterfaces:true /typeScriptVersion:5.4 /rxJsVersion:7.0

Regenerate whenever `catalog.v1.yaml` changes so the client stays in lockstep with the API.

## Usage

`CatalogApiClient` is an Angular `@Injectable` service over `HttpClient`. It is consumed by the
storefront / seller-portal / admin apps under `web/apps/*` once the Angular workspace is scaffolded
(frontend phase), wired with the API base URL and the Entra access-token interceptor (FE-12).
