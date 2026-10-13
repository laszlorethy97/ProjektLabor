import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KeyHandoverComponent } from './key-handover-component';

describe('KeyHandoverComponent', () => {
  let component: KeyHandoverComponent;
  let fixture: ComponentFixture<KeyHandoverComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KeyHandoverComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(KeyHandoverComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should collect the handover form values in a mock DTO', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.keyCode = ' KEY-001 ';

    component.submit();

    expect(component.mockDto).toEqual({
      email: 'user@example.com',
      pinCode: '0042',
      keyCode: 'KEY-001',
    });
  });
});
