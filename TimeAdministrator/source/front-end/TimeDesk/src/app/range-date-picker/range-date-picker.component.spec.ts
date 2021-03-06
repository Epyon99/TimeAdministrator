import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { NgbModule, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

import { RangeDatePickerComponent } from './range-date-picker.component';

describe('RangeDatePickerComponent', () => {
  let component: RangeDatePickerComponent;
  let fixture: ComponentFixture<RangeDatePickerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RangeDatePickerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RangeDatePickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
