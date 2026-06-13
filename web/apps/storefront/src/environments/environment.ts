export const environment = {
  production: false,
  // Catalog API (standalone local run on LocalDB). The dev server proxies nothing — the SPA calls
  // this origin directly, so the API enables a Development CORS policy for http://localhost:4200.
  apiBaseUrl: 'http://localhost:58118',
};
