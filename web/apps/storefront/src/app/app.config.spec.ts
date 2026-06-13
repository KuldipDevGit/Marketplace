import { TestBed } from '@angular/core/testing';
import { CatalogApiClient } from '@marketplace/catalog-api';
import { appConfig } from './app.config';

describe('appConfig', () => {
  it('provides the CatalogApiClient so the catalog facade can inject it', () => {
    // Guards the regression where the generated client (plain @Injectable, no providedIn) was not
    // registered: the facade's inject(CatalogApiClient) then threw and the routed pages rendered blank.
    TestBed.configureTestingModule({ providers: appConfig.providers });

    expect(TestBed.inject(CatalogApiClient)).toBeTruthy();
  });
});
