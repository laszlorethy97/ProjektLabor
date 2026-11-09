import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminCreateMasterkeyComponent } from './admin-create-masterkey-component';

describe('AdminCreateMasterkeyComponent', () => {
  let component: AdminCreateMasterkeyComponent;
  let fixture: ComponentFixture<AdminCreateMasterkeyComponent>;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateMasterkeyComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateMasterkeyComponent);
    component = fixture.componentInstance;
    httpTestingController = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
  });

  afterEach(() => {
    httpTestingController.verify();
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

  it('creates a master key and displays success feedback', () => {
    fixture.detectChanges();

    const button = (fixture.nativeElement as HTMLElement).querySelector('button');
    expect(button).toBeTruthy();
    button?.click();

    const request = httpTestingController.expectOne('/api/MasterKey/create');
    expect(request.request.method).toBe('POST');
    request.flush({ id: 1 }, { status: 201, statusText: 'Created' });

    expect(component.statusMessage()).toBe('A mesterkulcs sikeresen létrejött.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
  });

  it('displays the backend error when master key creation fails', () => {
    component.createMasterKey();

    const request = httpTestingController.expectOne('/api/MasterKey/create');
    request.flush({ message: 'Forbidden' }, { status: 403, statusText: 'Forbidden' });

    expect(component.statusMessage()).toBe('Forbidden');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
  });
});
