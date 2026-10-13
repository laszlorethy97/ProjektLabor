import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { Keyservice } from '../../services/keyservice';
import { KeyHandoverComponent } from './key-handover-component';

describe('KeyHandoverComponent', () => {
  let component: KeyHandoverComponent;
  let fixture: ComponentFixture<KeyHandoverComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KeyHandoverComponent],
      providers: [
        {
          provide: Keyservice,
          useValue: { returnKey: () => of(void 0) },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(KeyHandoverComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
