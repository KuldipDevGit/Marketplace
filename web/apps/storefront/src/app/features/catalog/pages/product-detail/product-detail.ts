import { ChangeDetectionStrategy, Component, OnInit, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductImageResponse, ProductResponse } from '@marketplace/catalog-api';
import { TagModule } from 'primeng/tag';
import { CatalogFacade } from '../../data/catalog.facade';

/** Smart (FE-3/FE-4): loads one product by route id (bound via withComponentInputBinding). */
@Component({
  selector: 'app-product-detail',
  imports: [RouterLink, TagModule],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductDetailPage implements OnInit {
  private readonly facade = inject(CatalogFacade);

  readonly id = input.required<string>();
  readonly product = this.facade.selectedProduct;
  readonly loading = this.facade.loadingDetail;

  ngOnInit(): void {
    this.facade.loadProduct(this.id());
  }

  primaryImage(product: ProductResponse): ProductImageResponse | null {
    return product.images.find((image) => image.isPrimary) ?? product.images[0] ?? null;
  }
}
