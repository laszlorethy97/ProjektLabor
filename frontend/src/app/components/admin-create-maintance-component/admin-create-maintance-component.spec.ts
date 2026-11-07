import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, ActivatedRoute } from '@angular/router';

import { AdminCreateMaintanceComponent } from './admin-create-maintance-component';

describe('AdminCreateMaintanceComponent', () => {
  let component: AdminCreateMaintanceComponent;
  let fixture: ComponentFixture<AdminCreateMaintanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateMaintanceComponent],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (name: string) => name === 'roomId' ? '42' : null,
              },
            },
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateMaintanceComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display the room ID from the route', () => {
    fixture.detectChanges();

    expect(component.roomId).toBe(42);
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('42. terem');
  });

  it('should require a valid from-to date and time period before requesting maintenance data', () => {
    component.requestMaintenance();

    expect(component.statusMessage).toBe('Add meg a karbantartás kezdő és záró időpontját.');
    expect(component.statusIsError).toBe(true);

    component.startDate = '2026-11-10T10:00';
    component.endDate = '2026-11-10T09:59';
    component.requestMaintenance();

    expect(component.statusMessage).toBe('A záró időpont nem lehet korábbi a kezdő időpontnál.');
    expect(component.statusIsError).toBe(true);

    component.endDate = '2026-11-10T10:00';
    component.requestMaintenance();

    expect(component.statusMessage).toBe('A karbantartás adatai bekérve.');
    expect(component.statusIsError).toBe(false);
  });

  it('should render date-time fields with minute precision', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const startInput = element.querySelector('input[name="startDate"]');
    const endInput = element.querySelector('input[name="endDate"]');

    expect(startInput?.getAttribute('type')).toBe('datetime-local');
    expect(startInput?.getAttribute('step')).toBe('60');
    expect(endInput?.getAttribute('type')).toBe('datetime-local');
    expect(endInput?.getAttribute('step')).toBe('60');
  });
});
