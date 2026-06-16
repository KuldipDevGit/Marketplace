# Local authentication (development)

The Catalog API is protected with JWT bearer auth. Read endpoints are anonymous; **writes require an
`Admin` or `Seller` role** (plus resource-ownership checks — a seller may only touch their own
products). Production uses **Microsoft Entra External ID** (SEC-4). Locally you mint your own tokens
with the built-in **`dotnet user-jwts`** tool; the signing key is stored in user-secrets, never in
the repo (SEC-9).

## Mint a token

From the API project folder:

```
cd src/Services/Catalog/Marketplace.Catalog.Api

# Admin — manage categories, brands, and any product
dotnet user-jwts create --role Admin

# Seller — manage their own products. Use the seeded demo seller id so you can edit the seeded
# products (which are owned by it):
dotnet user-jwts create --role Seller --claim seller_id=c0000000-0000-0000-0000-000000000001
```

The first run provisions a dev signing key in user-secrets and prints a token. Copy it.

## Use it in Scalar

1. Run the API (`dotnet run`) and open <http://localhost:58118/scalar>.
2. Click **Authorize**, pick **Bearer**, and paste the token.
3. Call a write endpoint — e.g. `POST /api/v1/products` (Seller) or `POST /api/v1/categories`
   (Admin). The created product then appears in the storefront.

## Notes

- Tokens expire. Re-mint with `dotnet user-jwts create …`; manage them with `dotnet user-jwts list`
  / `dotnet user-jwts clear`.
- The claims map to the API's caller: `sub`/`oid` → user id, `seller_id` → the seller, `role` → RBAC.
- **Development only** — the key is self-issued on your machine; this is not real authentication. In
  production the same code validates Entra-issued tokens via configuration
  (`Authentication:Schemes:Bearer:{Authority,Audience}` from Key Vault) — no code change.
