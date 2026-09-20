import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SwitchRolComponent } from './switch-rol-component';

describe('SwitchRolComponent', () => {
  let component: SwitchRolComponent;
  let fixture: ComponentFixture<SwitchRolComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SwitchRolComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SwitchRolComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
