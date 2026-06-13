import { Routes } from '@angular/router';

// Lazy-loaded per route (FE-10).
export const catalogRoutes: Routes = [
  {
    path: '',
    title: 'Catalog — Marketplace',
    loadComponent: () => import('./pages/product-list/product-list').then((m) => m.ProductListPage),
  },
  {
    path: 'product/:id',
    title: 'Product — Marketplace',
    loadComponent: () => import('./pages/product-detail/product-detail').then((m) => m.ProductDetailPage),
  },
];
