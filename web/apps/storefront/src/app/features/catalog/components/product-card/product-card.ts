import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductSummaryResponse } from '@marketplace/catalog-api';
import { TagModule } from 'primeng/tag';

/** Presentational (FE-3): renders one product summary; no injected services, input only. */
@Component({
  selector: 'app-product-card',
  imports: [RouterLink, TagModule],
  templateUrl: './product-card.html',
  styleUrl: './product-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductCard {
  readonly product = input.required<ProductSummaryResponse>();
}
