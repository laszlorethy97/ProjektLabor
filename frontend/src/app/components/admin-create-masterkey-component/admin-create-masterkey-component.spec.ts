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

  it('should create a master key and display success', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const button = element.querySelector('button');
    expect(button).toBeTruthy();
    button?.click();

    const request = httpTestingController.expectOne('/api/MasterKey/create');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({});
    expect(component.isSubmitting()).toBe(true);
    request.flush({ id: 5 });

    expect(component.statusMessage()).toBe('A mesterkulcs sikeresen létrejött.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
    fixture.detectChanges();
    expect(element.textContent).toContain('A mesterkulcs sikeresen létrejött.');
  });

  it('should display an error if master key creation fails', () => {
    component.createMasterKey();

    httpTestingController.expectOne('/api/MasterKey/create').flush(
      { message: 'Administrator not found.' },
      { status: 404, statusText: 'Not Found' },
    );

    expect(component.statusMessage()).toBe('Administrator not found.');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
  });
});
