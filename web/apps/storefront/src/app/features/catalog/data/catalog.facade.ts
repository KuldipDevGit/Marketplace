import { Injectable, inject, signal } from '@angular/core';
import {
  CatalogApiClient,
  CategoryResponse,
  ProductResponse,
  ProductStatus,
  ProductSummaryResponse,
} from '@marketplace/catalog-api';

/**
 * Owns catalog browsing state (FE-5) and the only place that talks to the API (FE-4/FE-6).
 * Components read the exposed signals and call the load* methods; errors are surfaced globally by
 * the HTTP error interceptor (FE-14), so here we only reset the loading flags.
 */
@Injectable({ providedIn: 'root' })
export class CatalogFacade {
  private readonly api = inject(CatalogApiClient);

  private readonly _categories = signal<readonly CategoryResponse[]>([]);
  private readonly _products = signal<readonly ProductSummaryResponse[]>([]);
  private readonly _selectedCategoryId = signal<string | null>(null);
  private readonly _selectedProduct = signal<ProductResponse | null>(null);
  private readonly _loadingList = signal(false);
  private readonly _loadingDetail = signal(false);

  readonly categories = this._categories.asReadonly();
  readonly products = this._products.asReadonly();
  readonly selectedCategoryId = this._selectedCategoryId.asReadonly();
  readonly selectedProduct = this._selectedProduct.asReadonly();
  readonly loadingList = this._loadingList.asReadonly();
  readonly loadingDetail = this._loadingDetail.asReadonly();

  loadCategories(): void {
    this.api.categoriesGET(undefined, undefined, 1, 100, 'sortOrder,name').subscribe({
      next: (page) => this._categories.set(page.items ?? []),
    });
  }

  loadProducts(categoryId: string | null): void {
    this._selectedCategoryId.set(categoryId);
    this._loadingList.set(true);
    this.api
      .productsGET(categoryId ?? undefined, undefined, undefined, ProductStatus.Active, undefined, 1, 60, 'name')
      .subscribe({
        next: (page) => {
          this._products.set(page.items ?? []);
          this._loadingList.set(false);
        },
        error: () => this._loadingList.set(false),
      });
  }

  loadProduct(id: string): void {
    this._selectedProduct.set(null);
    this._loadingDetail.set(true);
    this.api.productsGET2(id).subscribe({
      next: (product) => {
        this._selectedProduct.set(product);
        this._loadingDetail.set(false);
      },
      error: () => this._loadingDetail.set(false),
    });
  }
}
