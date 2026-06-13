import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CategoryNav } from '../../components/category-nav/category-nav';
import { ProductCard } from '../../components/product-card/product-card';
import { CatalogFacade } from '../../data/catalog.facade';

/** Smart (FE-3/FE-4): wires the facade state to the presentational grid + filter. */
@Component({
  selector: 'app-product-list',
  imports: [CategoryNav, ProductCard],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductListPage implements OnInit {
  private readonly facade = inject(CatalogFacade);

  readonly products = this.facade.products;
  readonly categories = this.facade.categories;
  readonly selectedCategoryId = this.facade.selectedCategoryId;
  readonly loading = this.facade.loadingList;

  ngOnInit(): void {
    this.facade.loadCategories();
    this.facade.loadProducts(null);
  }

  onCategoryChange(categoryId: string | null): void {
    this.facade.loadProducts(categoryId);
  }
}
