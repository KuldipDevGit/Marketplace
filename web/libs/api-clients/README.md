# Catalog API — generated TypeScript/Angular client

`catalog-api.client.ts` is a **generated** typed Angular client for the Catalog API, produced from
the contract-first OpenAPI document [`openapi/catalog.v1.yaml`](../../../openapi/catalog.v1.yaml)
with NSwag (API-1, FE-6). **The contract is the source of truth — do not hand-edit the client.**

It is imported via the `@marketplace/catalog-api` path alias (see `web/tsconfig.json`) and given its
base URL through the `API_BASE_URL` token in `apps/storefront/src/app/app.config.ts`.

## Regenerate

Install the NSwag CLI once (requires the .NET SDK):

    dotnet tool install --global NSwag.ConsoleCore --version 14.2.0

Then, from the repository root:

    nswag openapi2tsclient /input:openapi/catalog.v1.yaml /output:web/libs/api-clients/catalog-api.client.ts /template:Angular /className:CatalogApiClient /generateClientInterfaces:true /injectionTokenType:InjectionToken /typeScriptVersion:5.4 /rxJsVersion:7.0

`/injectionTokenType:InjectionToken` is required: the NSwag default emits the long-removed
`OpaqueToken`, which does not compile on modern Angular. Regenerate whenever `catalog.v1.yaml`
changes so the client stays in lockstep with the API.
