import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KeyPickupComponent } from './key-pickup-component';

describe('KeyPickupComponent', () => {
  let component: KeyPickupComponent;
  let fixture: ComponentFixture<KeyPickupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KeyPickupComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(KeyPickupComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
