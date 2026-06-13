import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryResponse } from '@marketplace/catalog-api';
import { SelectModule } from 'primeng/select';

interface CategoryOption {
  readonly label: string;
  readonly value: string;
}

/** Presentational (FE-3): a category filter; emits the chosen id (or null to clear). */
@Component({
  selector: 'app-category-nav',
  imports: [FormsModule, SelectModule],
  templateUrl: './category-nav.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoryNav {
  readonly categories = input.required<readonly CategoryResponse[]>();
  readonly selectedId = input<string | null>(null);
  readonly categoryChange = output<string | null>();

  readonly options = computed<CategoryOption[]>(() =>
    this.categories().map((category) => ({ label: category.name, value: category.id })),
  );
}
