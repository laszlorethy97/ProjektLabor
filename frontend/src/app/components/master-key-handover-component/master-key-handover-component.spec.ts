import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterKeyHandoverComponent } from './master-key-handover-component';

describe('MasterKeyHandoverComponent', () => {
  let component: MasterKeyHandoverComponent;
  let fixture: ComponentFixture<MasterKeyHandoverComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MasterKeyHandoverComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MasterKeyHandoverComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
