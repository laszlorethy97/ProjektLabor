import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterKeyComponent } from './master-key-component';

describe('MasterKeyComponent', () => {
  let component: MasterKeyComponent;
  let fixture: ComponentFixture<MasterKeyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MasterKeyComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MasterKeyComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
