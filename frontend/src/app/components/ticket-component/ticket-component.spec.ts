import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketComponent } from './ticket-component';

describe('TicketComponent', () => {
  let component: TicketComponent;
  let fixture: ComponentFixture<TicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TicketComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TicketComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should collect the problem description in a DTO', () => {
    component.description = '  A projektor nem kapcsol be.  ';

    component.submit();

    expect(component.mockDto).toEqual({ description: 'A projektor nem kapcsol be.' });
  });

  it('should not create a DTO for an empty description', () => {
    component.description = '   ';

    component.submit();

    expect(component.mockDto).toBeNull();
  });
});
