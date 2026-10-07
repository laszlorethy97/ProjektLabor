import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Janitor } from './janitor';

describe('Janitor', () => {
  let component: Janitor;
  let fixture: ComponentFixture<Janitor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Janitor],
    }).compileComponents();

    fixture = TestBed.createComponent(Janitor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
