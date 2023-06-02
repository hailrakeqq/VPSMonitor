import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SshkeysComponent } from './sshkeys.component';

describe('SshkeysComponent', () => {
  let component: SshkeysComponent;
  let fixture: ComponentFixture<SshkeysComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SshkeysComponent]
    });
    fixture = TestBed.createComponent(SshkeysComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
