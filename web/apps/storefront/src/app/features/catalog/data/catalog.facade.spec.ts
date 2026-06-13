import { TestBed } from '@angular/core/testing';
import {
  CatalogApiClient,
  PagedResultOfCategoryResponse,
  PagedResultOfProductSummaryResponse,
  ProductResponse,
  ProductStatus,
} from '@marketplace/catalog-api';
import { Observable, of } from 'rxjs';
import { CatalogFacade } from './catalog.facade';

/**
 * Stand-in for the generated client; only the read methods the facade uses are implemented.
 * Paged results are built via fromJS() — the same deserialization path the real HTTP client uses,
 * which correctly maps the nested items array (the bare constructor does not).
 */
class CatalogApiStub {
  categoriesGET(): Observable<PagedResultOfCategoryResponse> {
    return of(
      PagedResultOfCategoryResponse.fromJS({
        items: [{ id: 'c1', name: 'Books', slug: 'books', sortOrder: 1, isActive: true, createdAtUtc: '2026-01-01T00:00:00Z' }],
        page: 1,
        pageSize: 100,
        totalCount: 1,
        totalPages: 1,
      }),
    );
  }

  productsGET(): Observable<PagedResultOfProductSummaryResponse> {
    return of(
      PagedResultOfProductSummaryResponse.fromJS({
        items: [{ id: 'p1', sellerId: 's1', name: 'Widget', slug: 'widget', categoryId: 'c1', status: 'Active' }],
        page: 1,
        pageSize: 60,
        totalCount: 1,
        totalPages: 1,
      }),
    );
  }

  productsGET2(): Observable<ProductResponse> {
    return of(
      new ProductResponse({
        id: 'p1',
        sellerId: 's1',
        categoryId: 'c1',
        name: 'Widget',
        slug: 'widget',
        description: 'A widget',
        sku: 'SKU-1',
        status: ProductStatus.Active,
        attributes: [],
        images: [],
        createdAtUtc: new Date(),
      }),
    );
  }
}

describe('CatalogFacade', () => {
  let facade: CatalogFacade;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: CatalogApiClient, useClass: CatalogApiStub }],
    });
    facade = TestBed.inject(CatalogFacade);
  });

  it('loads active products into a signal and clears the loading flag', () => {
    facade.loadProducts(null);

    expect(facade.products().length).toBe(1);
    expect(facade.products()[0].name).toBe('Widget');
    expect(facade.loadingList()).toBe(false);
  });

  it('records the selected category', () => {
    facade.loadProducts('c1');

    expect(facade.selectedCategoryId()).toBe('c1');
  });

  it('loads categories', () => {
    facade.loadCategories();

    expect(facade.categories().length).toBe(1);
    expect(facade.categories()[0].slug).toBe('books');
  });

  it('loads a single product into the detail signal', () => {
    facade.loadProduct('p1');

    expect(facade.selectedProduct()?.sku).toBe('SKU-1');
    expect(facade.loadingDetail()).toBe(false);
  });
});
