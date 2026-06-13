export const environment = {
  production: false,
  // Catalog API base — includes the OpenAPI `servers` base path (/api/v1); the generated client
  // appends resource paths (e.g. /products) to this. The API allows CORS from http://localhost:4200.
  apiBaseUrl: 'http://localhost:58118/api/v1',
};
