import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./features/catalog/catalog.routes').then((m) => m.catalogRoutes),
  },
  { path: '**', redirectTo: '' },
];
