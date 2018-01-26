import { Component, OnInit, Output, Input, EventEmitter } from '@angular/core';
import { NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'Epy-LogTiempo-range-date-picker',
  templateUrl: './range-date-picker.component.html',
  styleUrls: ['./range-date-picker.component.css']
})
export class RangeDatePickerComponent implements OnInit {

  @Input() dateFrom: Date;
  @Output() dateFromChange: EventEmitter<Date> = new EventEmitter();
  @Input() dateTill: Date;
  @Output() dateTillChange: EventEmitter<Date> = new EventEmitter();

  @Input() RangeSelected: boolean;

  dateFromModel: any;
  dateTillModel: any;

  ///
  /// Removable Section just for testing 
  ///

  @Input() simpleModelPicker: any;
  @Output() simpleModelPickerChange: EventEmitter<any> = new EventEmitter();

  @Input() simpleModelList: any[] = [];
  @Output() simpleModelListChange: EventEmitter<any> = new EventEmitter();

  addToList() {
    var index =  this.simpleModelPicker.year + "-"  + this.simpleModelPicker.month + "-" + this.simpleModelPicker.day;
    var result = this.simpleModelList.find(g => g.id == index);
    if (result != undefined) {
      return;
    }

    if (this.simpleModelList == undefined) {
      this.simpleModelList = [];
    }

    var z = { "index": this.simpleModelList.length, "date": this.simpleModelPicker, "id": index };
    this.simpleModelList.push(z);
    this.simpleModelList = this.simpleModelList.sort(this.sortComparer);
  }

  removeFromList(id) {
    var index = this.simpleModelList.findIndex(g=>g.id == id);
    this.simpleModelList.splice(index, 1);
  }

  sortComparer(a,b) {
    return Date.parse(a.id) - Date.parse(b.id);
  }

  ///
  /// End of the removable section 
  ///


  constructor(private ngbDateParserFormatter: NgbDateParserFormatter) {
    var today = new Date();
    this.dateFromModel = { year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate() };
    this.dateTillModel = { year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate() };
    this.dateTill = new Date(this.dateTillModel.year, this.dateTillModel.month - 1, this.dateTillModel.day, 0, -new Date().getTimezoneOffset(), 0, 0);
    this.dateFrom = new Date(this.dateFromModel.year, this.dateFromModel.month - 1, this.dateFromModel.day, 0, -new Date().getTimezoneOffset(), 0, 0);
    this.dateTillChange.emit(this.dateTill);
    this.dateFromChange.emit(this.dateFrom);
  }

  ngOnInit() {

  }

  selectedFromDate() {
    this.dateFrom = new Date(this.dateFromModel.year, this.dateFromModel.month - 1, this.dateFromModel.day, 0, -new Date().getTimezoneOffset(), 0, 0);
    this.dateFromChange.emit(this.dateFrom);
  }
  selectedTillDate() {
    this.dateTill = new Date(this.dateTillModel.year, this.dateTillModel.month - 1, this.dateTillModel.day, 0, -new Date().getTimezoneOffset(), 0, 0);
    this.dateTillChange.emit(this.dateTill);
  }

  isRangeSelected() {
    if (this.dateFrom != null && this.dateTill != null) {
      this.RangeSelected = true;
    }
    else {
      this.RangeSelected = false;
    }
  }
}
