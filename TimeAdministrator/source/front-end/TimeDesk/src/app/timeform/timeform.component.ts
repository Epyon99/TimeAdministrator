import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { NgbModule, NgbDropdown, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

import { Timeform } from './timeform';
import { Timeline } from '../timeline/timeline';
import { TipoLogTiempo } from './TipoLogTiempo'
import { TimeLogService } from '../time-log.service';
import { TimeLogResponse } from '../time-log-response';
import { Router, ActivatedRoute, Params } from '@angular/router';

const BLOCK_UI_TEXT = "block_ui_text";

const WORKTYPES: TipoLogTiempo[] = [
  { id: "1", color: "blue", name: "Work", factor: 0 },
  { id: "2", color: "green", name: "Funeral", factor: 0 },
  { id: "3", color: "red", name: "Holidays", factor: 0 },
  { id: "4", color: "purple", name: "Birth", factor: 0 },
  { id: "5", color: "black", name: "Marriage", factor: 0 },
  { id: "6", color: "navy", name: "Compensation", factor: 0 },
  { id: "7", color: "gray", name: "Sick", factor: 0 },
  { id: "8", color: "yellow", name: "Military Service", factor: 0 },
  { id: "9", color: "orange", name: "School", factor: 0 },
  { id: "10", color: "brown", name: "Accident", factor: 0 },
  { id: "11", color: "beige", name: "Fatherhood", factor: 0 },
  { id: "12", color: "pink", name: "Timeadjust", factor: 0 },
  { id: "13", color: "lightblue", name: "Movement", factor: 0 },
]

@Component({
  selector: 'Epy-LogTiempo-timeform',
  templateUrl: './timeform.component.html',
  styleUrls: ['./timeform.component.css'],
  providers: [TimeLogService]
})

export class TimeformComponent implements OnInit {

  @ViewChild('buchenDrop') buchenDrop: NgbDropdown;

  timeform: any;

  selectedTimeLine: Timeline;

  listofTipoLogTiempos: Array<TipoLogTiempo> = [];

  ///
  /// this property helps to find out wich method would be colled create or close.
  ///
  creatingLog: Boolean;
  creatingType: TipoLogTiempo;
  ///
  editedTimeline: Timeline;

  user: string;
  favoriteWords: string[];
  // Range Dates configuration.
  rangeDateTill: Date;
  rangeDateFrom: Date;
  rangeSelected: boolean;
  rangeActive: boolean;
  rangeList: any[] = [];

  errorMessage: string;
  buchenEnable: Boolean;

  constructor(private timeLogService: TimeLogService,
    private ngbDateParserFormatter: NgbDateParserFormatter) {
    debugger;
    var url: string = window.location.toString();
    var data: string[] = url.split('#');
    if (data.length > 1) {
      var token = data[1].split('=')[1];
      timeLogService.addToken(token);
      window.history.pushState("object or string", "Title", "/" );
    }
    if (timeLogService.temporalToken == "" || timeLogService.temporalToken == undefined) {
      window.location.href = "http://epyhost:6001/v1/account/ExternalLogin?provider=aad&returnUrl=http://epyhost:4200";
    }

    this.favoriteWords = [];
    this.timeform = new Timeform();
    var today = new Date();
    this.timeform.date = { year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate() };
    this.timeform.listOfTimelines = [];
    this.listofTipoLogTiempos = [];
    var TipoLogTiemposData = this.timeLogService.getTipoLogTiempos().subscribe(
      (typeData) => {
        for (let l of (typeData as TipoLogTiempo[])) {
          var tlt = new TipoLogTiempo;
          tlt.color = l.color;
          tlt.factor = l.factor;
          tlt.id = l.id;
          tlt.name = l.name;
          this.listofTipoLogTiempos.push(tlt);
        }
        this.timeform.baseTipoLogTiempo = this.listofTipoLogTiempos[0];
        this.loadTimeLogs();

      },
      (error) => {
        this.loadTimeLogs();
      },
    );
    this.buchenEnable = true;
    this.rangeActive = false;
  }


  ngOnInit() {
    this.listofTipoLogTiempos = [];
  }

  ///
  /// Load timelogs from a specific day.
  ///
  public loadTimeLogs() {
    this.clearTimelineSelection();
    var lastTime = new Date();
    var timeline = new Timeline();
    // the .getTimezoneOffset is substracted from the minutes so achive the UTC hours of the date
    // when using the method .toISOString
    var fromDate = new Date(this.timeform.date.year, this.timeform.date.month - 1, this.timeform.date.day, 0, -new Date().getTimezoneOffset(), 0, 0);
    var tillDate = new Date(this.timeform.date.year, this.timeform.date.month - 1, this.timeform.date.day + 1, 0, -new Date().getTimezoneOffset(), 0, 0);

    var timelogData = this.timeLogService.getRangeLog(fromDate.toISOString(), tillDate.toISOString()).subscribe(
      (timeData) => {
        (this.timeform as Timeform).listOfTimelines = [];
        (this.timeform as Timeform).listOfWorklines = [];
        for (let l of (timeData as TimeLogResponse[])) {
          timeline = new Timeline();
          timeline.id = l.id;
          timeline.startTime = new Date(l.startTime);
          timeline.type = l.type;
          if (l.type == null || l.type === undefined || l.type == "00000000-0000-0000-0000-000000000000") {
            timeline.color = this.listofTipoLogTiempos[0].color;
          }
          else {
            var t = this.listofTipoLogTiempos.find(g => g.id == l.type);
            timeline.color = t.color;
            timeline.TipoLogTiempoName = t.name;
          }
          debugger;
          if (l.endTime != null) {
            timeline.endTime = new Date(l.endTime);
            timeline.length = this.timeSizeConvertor(timeline.startTime as Date, timeline.endTime as Date);
            timeline.isDateIncomplete = false;
          }
          else {
            console.log("End Time Open");
            // Color to difference the editing field.
            timeline.color = "navy";
            timeline.isDateIncomplete = true;
            timeline.endTime = new Date(Date.now());
            this.buchenEnable = false;
            this.editedTimeline = timeline;
            this.creatingLog = true;
            this.creatingType = this.listofTipoLogTiempos.find(g => g.id == timeline.type);
            if (l.type == null || l.type === undefined || l.type == "00000000-0000-0000-0000-000000000000") {
              this.creatingType = this.listofTipoLogTiempos[0];
            }
            timeline.length = this.timeSizeConvertor(timeline.startTime as Date, timeline.endTime as Date);
          }



          timeline.startTimeReason = l.startReason;
          timeline.endTimeReason = l.endReason;

          // Verify the baseTipoLogTiempo (Work) and the Type is the same.
          if (this.timeform.baseTipoLogTiempo.id != l.type && l.type != null && l.type != undefined) {
            if (timeline.startTime.getDate() != fromDate.getDate()) {
              console.log("dia diferente:" + timeline.startTime.getDate() + "-" + fromDate.getDate());
              break;
            }
            (this.timeform as Timeform).listOfTimelines.push(timeline);
          }
          else {
            // Add dummy timeline
            if (lastIterationTime == null) {
              var lastIterationTime = new Date();
              lastIterationTime.setHours(0, 0, 0);
            }

            this.timeSeparator(lastIterationTime, new Date(l.startTime));
            lastIterationTime = timeline.endTime;
            (this.timeform as Timeform).listOfWorklines.push(timeline);
          }
          this.buchenEnable = true;
          var subscrition = this.timeLogService.getListofReasonsForUser("").subscribe(
            (words) => {

              this.favoriteWords = [];
              for (let l of (words as string[])) {
                this.favoriteWords.push(l);
              }
              console.log("Subscription ended: " + this.favoriteWords);
            },
            (error) => { console.log("error") }
          );
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  ///
  /// This method add a new timeline to the timeform collection and save it the the server.
  ///
  public createTimeLog(logtype: any, color: any) {
    debugger;
    var today = new Date();
    var date = null;
    if (logtype == this.listofTipoLogTiempos[0].id && (this.timeform.date.year == today.getFullYear() && (this.timeform.date.month - 1) == today.getMonth() && this.timeform.date.day == today.getDate())) {
      date = null;
    }
    else {
      var d = new Date(this.timeform.date.year, this.timeform.date.month - 1, this.timeform.date.day, 0, 10, 0, 0);
      date = d.toISOString();
    }
    // It returns a timeline with starttime from the server.
    var response: any = this.timeLogService.openTimeLog(date, logtype).subscribe(
      (timelog) => {
        var timelogresponse: TimeLogResponse = timelog as TimeLogResponse;
        var timeline = new Timeline();
        timeline.id = timelogresponse.id;
        timeline.startTime = new Date(timelogresponse.startTime);
        timeline.color = color;
        timeline.type = logtype;
        this.editedTimeline = timeline;
        this.buchenEnable = true;
        if (date != null || timelogresponse.endTime != null) {
          timeline.endTime = new Date(timelogresponse.endTime);
          this.buchenEnable = false;
          this.editedTimeline = null;
          this.creatingLog = false;
          timeline.length = this.timeSizeConvertor(timeline.startTime as Date, timeline.endTime as Date);
        }
        this.refresh("reloadTimelines");
        return timeline;
      },
      (error) => {
        this.buchenEnable = true;
        this.errorMessage = error;
      }
    );
  }

  public createRangeTimeLogs(type: any, color: any) {
    var rangeListFormatted = [];
    for (let d of this.rangeList) {
      rangeListFormatted.push(new Date(Date.parse(d.id)).toISOString());
    }
    var response: any = this.timeLogService.createRangeLogs(rangeListFormatted, type).subscribe(
      (timelogs) => {
        this.buchenEnable = true;
        this.clearTimelineSelection();
        this.creatingLog = false;
        console.log("Response from RangeLogs");
        console.log(timelogs);
        this.refresh("reloadTimelines");
      },
      (error) => {
        this.clearTimelineSelection();
        this.buchenEnable = true;
        this.editedTimeline = null;
        this.creatingLog = false;
      }
    );
  }

  ///
  /// Closes a Timeline register. 
  ///
  public closeTimeLog(typeLog: string) {
    var date = null;
    var result = this.timeLogService.openTimeLog(date, typeLog).subscribe(
      (timelog) => {

        var timelogresponse = timelog as TimeLogResponse;
        this.editedTimeline.endTime = new Date(timelogresponse.startTime);
        var barPercentage = this.timeSizeConvertor(this.editedTimeline.startTime, this.editedTimeline.endTime);
        this.editedTimeline.length = barPercentage;
        this.creatingLog = false;
        this.refresh("reloadTimelines");
        this.buchenEnable = true;
        this.editedTimeline = null;
      },
      (error) => {
        this.errorMessage = error;
        this.buchenEnable = true;
      }
    );
  }

  ///
  /// This event is for the parent buchen buttom so it does not display when 
  /// a timelog is going to be closed.
  ///
  public buchenParentBtnEvent() {
    debugger;
    if (this.editedTimeline != null || this.editedTimeline != undefined) {
      console.log("days:" + this.timeform.date.day + "-" + this.editedTimeline.startTime.getDate());
      console.log("days:" + this.timeform.date.month + "-" + (this.editedTimeline.startTime.getMonth() + 1));
      console.log("days:" + this.timeform.date.year + "-" + this.editedTimeline.startTime.getFullYear());

    }
    if (this.creatingLog && this.timeform.date.day == this.editedTimeline.startTime.getDate() && this.timeform.date.month == this.editedTimeline.startTime.getMonth() + 1 && this.timeform.date.year == this.editedTimeline.startTime.getFullYear()) {
      console.log("Date of editing and editing");
      this.buchenEnable = false;
      this.closeTimeLog(this.creatingType.id);
      this.buchenDrop.close();
    }
  }

  ///
  /// This is the generic event for the list of buchen buttoms for types.
  ///
  public buchenBtnEvent(reason: TipoLogTiempo) {

    this.creatingType = reason;
    debugger;
    if (!(this.creatingLog && this.timeform.date.day == this.editedTimeline.startTime.getDate() && this.timeform.date.month == this.editedTimeline.startTime.getMonth() + 1 && this.timeform.date.year == this.editedTimeline.startTime.getFullYear())) {
      this.creatingLog = true;
      this.buchenEnable = false;
      if (this.rangeActive) {
        this.createRangeTimeLogs(reason.id, reason.color);
      }
      else {
        this.createTimeLog(this.creatingType.id, this.creatingType.color);
      }
    }
    else {
      this.buchenEnable = false;
      this.creatingLog = false;
      this.closeTimeLog(this.creatingType.id);
    }
  }

  ///
  /// Calculate the timelines size
  /// Returns a string in the following format dd%
  ///
  private timeSizeConvertor(start: Date, end: Date) {
    /// The size is calculated in base of the percentage of a 100% bar that represents 24 hours.
    /// Every timeline inside is a relative percentage base on the hours it last.
    /// Eg. A timeline of 12 hours is 50% of 24 hours.

    /// It follows the relation as a rule of 3.
    /// (DurationOfWork*100%)/MinutesInADay = Percentual Value of time.
    var startHours = start.getUTCHours();
    var startMinutes = start.getUTCMinutes();

    /// if startDate is a day before the selected date then startHours and minutes are 0
    /// Todo: Experimental 396 
    // if ((start.getDate() < this.timeform.date.day && start.getMonth() + 1 <= this.timeform.date.month) ||
    //   (start.getDate() > this.timeform.date.day && start.getMonth() + 1 < this.timeform.date.month)) {
    //   startHours = 0;
    //   startMinutes = 0;
    // }

    var startTotalMinutes = (startHours * 60) + startMinutes;

    var endHours = end.getUTCHours();
    var endMinutes = end.getUTCMinutes();

    // Todo: Experimental 396
    // if ((end.getDate() > this.timeform.date.day && end.getMonth() + 1 == this.timeform.date.month) ||
    //  (end.getDate() < this.timeform.date.day && end.getMonth() + 1 > this.timeform.date.month)) {
    //  endHours = 0;
    //  endMinutes = 0;
    //  }
    var endTotalMinutes = (endHours < startHours) ? ((24 + endHours) * 60) + endMinutes : ((endHours) * 60) + endMinutes;
    /// 24 hours casted to minutes.
    var totalDayMinutes = 24 * 60;//1440

    /// Duration of the line. 
    var durationOfWork = Math.abs(endTotalMinutes - startTotalMinutes);
    durationOfWork = parseInt((durationOfWork * 100).toString()) / totalDayMinutes;

    if (durationOfWork < 1) {
      durationOfWork = 1;
    }

    return durationOfWork + "%";
  }

  ///
  /// Create a fake timeline to fill spaces in the list.
  ///
  private timeSeparator(lastTime: Date, nextTime: Date) {
    if (lastTime != null && nextTime != null) {
      if (lastTime > nextTime) {
        lastTime.setFullYear(nextTime.getFullYear());
        lastTime.setDate(nextTime.getDate());
        lastTime.setMonth(nextTime.getMonth());
      }
      if (parseInt(this.timeSizeConvertor(lastTime, nextTime)) > 2) {
        // return a new timeline with the size between that inverval

        var dummyline = new Timeline;
        dummyline.startTime = lastTime;
        dummyline.endTime = nextTime;
        dummyline.type = "fill";
        dummyline.length = this.timeSizeConvertor(lastTime, nextTime);
        dummyline.color = "#eceeee";
        this.timeform.listOfWorklines.push(dummyline);
      }
    }
  }

  private clearTimelineSelection() {
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
  }

  private refresh(event) {
    if (event == "reloadTimelines") {
      console.log("Reload triggered");
      this.loadTimeLogs();
    }
  }

  private rangeFromSet(event) {
    this.rangeDateFrom = event;
  }
  private rangeTullSet(event) {
    this.rangeDateTill = event;
  }


}
