import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TutorComponent } from './tutor-component';

describe('TutorComponent', () => {
  let component: TutorComponent;
  let fixture: ComponentFixture<TutorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TutorComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TutorComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
