import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCreatePermissionComponent } from './admin-create-permission-component';

describe('AdminCreatePermissionComponent', () => {
  let component: AdminCreatePermissionComponent;
  let fixture: ComponentFixture<AdminCreatePermissionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreatePermissionComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreatePermissionComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
