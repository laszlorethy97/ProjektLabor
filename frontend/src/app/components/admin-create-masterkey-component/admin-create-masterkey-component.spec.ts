import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminCreateMasterkeyComponent } from './admin-create-masterkey-component';

describe('AdminCreateMasterkeyComponent', () => {
  let component: AdminCreateMasterkeyComponent;
  let fixture: ComponentFixture<AdminCreateMasterkeyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateMasterkeyComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateMasterkeyComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should explain that the plus button creates a new master key', () => {
    fixture.detectChanges();

    const pageText = (fixture.nativeElement as HTMLElement).textContent ?? '';

    expect(pageText).toContain('Új mesterkulcs létrehozásához nyomd meg a plusz gombot.');
    expect(pageText).not.toContain('adatküldés nem történik');
  });

  it('should confirm that master key details were requested', () => {
    fixture.detectChanges();

    const button = (fixture.nativeElement as HTMLElement).querySelector('button');
    expect(button).toBeTruthy();
    button?.click();

    expect(component.statusMessage).toBe('A mesterkulcs adatai bekérve.');
  });
});
