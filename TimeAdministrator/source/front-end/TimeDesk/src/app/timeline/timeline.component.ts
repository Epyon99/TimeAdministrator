import { Component, OnInit, Input, Output, EventEmitter, SimpleChange, OnChanges } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/Rx';

import { Timeline } from './timeline';
import { Timeform } from '../timeform/timeform'
import { TimeLogService } from '../time-log.service';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';

@Component({
  selector: 'Epy-LogTiempo-timeline',
  templateUrl: './timeline.component.html',
  styleUrls: ['./timeline.component.css']
})
export class TimelineComponent implements OnInit, OnChanges {

  @Input() timeForm: Timeform;
  @Input() selectedTimeLine: Timeline;
  @Output() reload: EventEmitter<string> = new EventEmitter();
  endReasonValidation: boolean;
  startReasonValidation: boolean;
  oldEndReasonValidation: any;
  oldStartReasonValidation: any;
  @Input() favoriteWords: string[];

  constructor(private timeLogService: TimeLogService) {

    this.timeForm = new Timeform;
    this.timeForm.listOfWorklines = [];
    this.timeForm.listOfTimelines = [];
  }

  ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
    console.log("Timeline changes");
    let log: string[] = [];
    for (let propName in changes) {
      let changedProp = changes[propName];
      let to = JSON.stringify(changedProp.currentValue);
      if (changedProp.isFirstChange()) {
        log.push(`Initial value of ${propName} set to ${to}`);
      } else {
        let from = JSON.stringify(changedProp.previousValue);
        log.push(`${propName} changed from ${from} to ${to}`);
      }
    }
    this.clearTimelineSelection();
  }
  ngOnInit() {

    this.endReasonValidation = false;
    this.startReasonValidation = false;
    this.selectedTimeLine = new Timeline;
    this.selectedTimeLine.endTimeModel = {
      "hour": 0,
      "minute": 0
    };
    this.selectedTimeLine.startTimeModel = {
      "hour": 0,
      "minute": 0
    };
    this.selectedTimeLine.id = "";
    this.selectedTimeLine.endTimeReason = "";
    this.selectedTimeLine.startTimeReason = "";
  }

  observableSource(keyword: any) {
    let filteredList = this.favoriteWords.filter(el => el.indexOf(keyword) !== -1);
    return Observable.of(filteredList);
  }

  ///
  /// This event fill the timelines component when selected.
  ///
  public pickTimeline(id, logtype) {

    if (logtype == this.timeForm.baseTipoLogTiempo.id || logtype == null) {
      this.selectedTimeLine = (this.timeForm as Timeform).listOfWorklines.filter(x => x.id == id)[0] as Timeline;
    }
    else {
      this.selectedTimeLine = (this.timeForm as Timeform).listOfTimelines.filter(x => x.id == id)[0] as Timeline;
    }
    if ((this.selectedTimeLine.startTime != null || this.selectedTimeLine.startTime != undefined) && (this.selectedTimeLine.endTime != null || this.selectedTimeLine.endTime != undefined)) {
      this.selectedTimeLine.startTimeModel = {
        "hour": this.selectedTimeLine.startTime.getHours(),
        "minute": this.selectedTimeLine.endTime.getMinutes()
      };
      this.selectedTimeLine.endTimeModel = {
        "hour": this.selectedTimeLine.endTime.getHours(),
        "minute": this.selectedTimeLine.endTime.getMinutes()
      };
    }
    else {
      this.selectedTimeLine.startTimeModel = {
        "hour": 0,
        "minute": 0
      };
      this.selectedTimeLine.endTimeModel = {
        "hour": 0,
        "minute": 0
      };
    }
    this.oldEndReasonValidation = this.selectedTimeLine.endTimeModel;
    this.oldStartReasonValidation = this.selectedTimeLine.startTimeModel;
    if (this.selectedTimeLine.startTimeReason == null || this.selectedTimeLine.startTimeReason === undefined) {
      this.selectedTimeLine.startTimeReason = "";
    }
    if (this.selectedTimeLine.endTimeReason == null || this.selectedTimeLine.endTimeReason === undefined) {
      this.selectedTimeLine.endTimeReason = "";
    }
  }

  startTimePicker() {
    this.selectedTimeLine.startTime.setHours(this.selectedTimeLine.startTimeModel.hour);
    this.selectedTimeLine.startTime.setMinutes(this.selectedTimeLine.startTimeModel.minute);
  }

  endTimePicker() {
    this.selectedTimeLine.endTime.setHours(this.selectedTimeLine.endTimeModel.hour);
    this.selectedTimeLine.endTime.setMinutes(this.selectedTimeLine.endTimeModel.minute);
  }

  updateTimeline() {
    // sucess update the timlines in page
    // error revert the changes.
    var data = undefined;
    if (this.selectedTimeLine.isDateIncomplete) {
      data = {
        Id: this.selectedTimeLine.id, TimeStart: this.selectedTimeLine.startTime, TimeEnd: null,
        TimeStartReason: this.selectedTimeLine.startTimeReason, TimeEndReason: this.selectedTimeLine.endTimeReason
      };
    }
    else {
      data = {
        Id: this.selectedTimeLine.id, TimeStart: this.selectedTimeLine.startTime, TimeEnd: this.selectedTimeLine.endTime,
        TimeStartReason: this.selectedTimeLine.startTimeReason, TimeEndReason: this.selectedTimeLine.endTimeReason
      };
    }

    var timelogData = this.timeLogService.modifyTimelog(data).subscribe(
      (timeData) => {
        this.reload.emit("reloadTimelines");
      },
      (error) => {
        this.reload.emit("reloadTimelines");
      });
  }

  validateReasons() {
    if (this.selectedTimeLine != null) {
      if (this.selectedTimeLine.startTimeModel != this.oldStartReasonValidation) {
        if (this.selectedTimeLine.startTimeReason.length == 0 || !(/^\S*$/.test(this.selectedTimeLine.startTimeReason))) {
          this.startReasonValidation = true;
        }
      }
      if (this.selectedTimeLine.endTimeModel != this.oldEndReasonValidation) {
        if (this.selectedTimeLine.endTimeReason.length == 0 || !(/^\S*$/.test(this.selectedTimeLine.endTimeReason))) {
          this.endReasonValidation = true;
        }
      }
      if (this.endReasonValidation || this.startReasonValidation) {
        return true;
      }
      else {
        // Post to server.
        this.updateTimeline();
      }
    }
  }
  validateStartReason() {
    if (this.selectedTimeLine.startTimeModel != this.oldStartReasonValidation) {
      if (this.selectedTimeLine.startTimeReason.length == 0 || !(/^\S*$/.test(this.selectedTimeLine.startTimeReason))) {
        this.startReasonValidation = true;
      }
      else {
        this.startReasonValidation = false;
      }
    }
  }
  validateEndReason() {

    if (this.selectedTimeLine.endTimeModel != this.oldEndReasonValidation) {
      if (this.selectedTimeLine.endTimeReason.length == 0 || !(/^\S*$/.test(this.selectedTimeLine.endTimeReason))) {
        this.endReasonValidation = true;
      }
      else {
        this.endReasonValidation = false;
      }
    }
  }

  deleteTimeline() {
    if (this.selectedTimeLine != null) {
      var timelogData = this.timeLogService.deleteTimelog(this.selectedTimeLine.id).subscribe(
        (timeData) => {
          this.clearTimelineSelection();
          this.reload.emit("reloadTimelines");
        },
        (error) => {
          this.clearTimelineSelection();
          this.reload.emit("reloadTimelines");
        });
    }
  }
  
  clearTimelineSelection() {
    this.selectedTimeLine = new Timeline;
    this.selectedTimeLine.endTimeModel = {
      "hour": 0,
      "minute": 0
    };
    this.selectedTimeLine.startTimeModel = {
      "hour": 0,
      "minute": 0
    }
    this.selectedTimeLine.endTimeReason = "";
    this.selectedTimeLine.startTimeReason = "";
    this.selectedTimeLine.id = "";
    this.endReasonValidation = false;
    this.startReasonValidation = false;
  }
}
