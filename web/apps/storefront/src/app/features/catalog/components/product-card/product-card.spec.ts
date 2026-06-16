import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ProductStatus, ProductSummaryResponse } from '@marketplace/catalog-api';
import { providePrimeNG } from 'primeng/config';
import { ProductCard } from './product-card';

describe('ProductCard', () => {
  it('renders the product name and links to its detail page', async () => {
    await TestBed.configureTestingModule({
      imports: [ProductCard],
      providers: [provideRouter([]), providePrimeNG({}), provideNoopAnimations()],
    }).compileComponents();

    const fixture = TestBed.createComponent(ProductCard);
    fixture.componentRef.setInput(
      'product',
      new ProductSummaryResponse({
        id: 'p1',
        sellerId: 's1',
        name: 'Acme Widget',
        slug: 'acme-widget',
        categoryId: 'c1',
        status: ProductStatus.Active,
      }),
    );
    await fixture.whenStable();

    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.product-card__title')?.textContent).toContain('Acme Widget');
    expect(el.querySelector('a.product-card')?.getAttribute('href')).toContain('/product/p1');
  });
});
