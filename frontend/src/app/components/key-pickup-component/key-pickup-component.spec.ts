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

  it('should collect the pickup form values in a mock DTO', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.key = 'Room 12';

    component.submit();

    expect(component.mockDto).toEqual({
      email: 'user@example.com',
      pinCode: '0042',
      key: 'Room 12',
    });
  });
});
