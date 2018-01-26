import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';

import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import { Timeform } from './timeform/timeform';
import { Timeline } from './timeline/timeline';
import { environment } from './environment';

@Injectable()
export class TimeLogService {
  private serviceUrl = environment.servicesUrl + "v1/logs";
  private bookingUrl = environment.servicesUrl + "v1/bookings";
  private timelogServiceUrl = environment.TipoLogTiempoServiceUrl + "v1/types";
  private searchServiceUrl = environment.servicesUrl;
  private headersOpt = new Headers({});
  private config = new RequestOptions({ headers: this.headersOpt });
  public temporalToken: any;



  constructor(private http: Http) { }

  ///
  /// Get the Time from the server base on the user.
  ///
  openTimeLog(date: any, type: string) {
    return this.http.put(this.serviceUrl, { "DateTime": date, "LogType": type }, this.config)
      .map(this.extractData)
      .catch(this.handleError);
  }

  ///
  /// Get all the timelogs on the server  
  ///
  getRangeLog(from: string, till: string) {
    let params: URLSearchParams = new URLSearchParams();
    params.set('from', from);
    params.set('till', till);
    var config = new RequestOptions({ headers: this.headersOpt, search: params });
    config.search = params;
    var result = this.http.get(this.serviceUrl, config).map(this.extractData).catch(this.handleError);
    return result;
  }

  ///
  /// Get all TipoLogTiempos
  ///
  getTipoLogTiempos() {
    debugger;
    var result = this.http.get(this.timelogServiceUrl, this.config).map(this.extractData).catch(this.handleError);
    return result;
  }

  /// 
  /// Front-end test data only.
  ///
  getTimeLogs(from: string, till: string): any[] {
    let logs = [];
    var num: number;
    for (num = 0; num <= 0; num++) {
      var time = new Timeline();
      time.startTime = new Date();
      time.endTime = new Date();
      time.id = num.toString();
      logs.push(time);
      time.length = "100%";
      time.color = "white";
      switch (num) {
        case 1:
          time.color = "red";
          time.length = "15%"
          break;
        case 2:
          time.color = "blue";
          time.length = "12%"
          break;
        case 3:
          time.color = "green";
          time.length = "20%";
          break;
        case 4:
          time.color = "black";
          time.length = "30%"
          break;
      }
    }

    return logs;
  }

  ///
  /// Updates an existing timelog
  ///
  modifyTimelog(timeline: any) {
    return this.http.post(this.serviceUrl, { "Id": timeline.Id, "TimeStart": timeline.TimeStart, "TimeEnd": timeline.TimeEnd, "TimeStartReason": timeline.TimeStartReason, "TimeEndReason": timeline.TimeEndReason }, this.config)
      .map(this.extractData)
      .catch(this.handleError);
  }

  ///
  /// Deletes an existing timelog
  ///
  deleteTimelog(id: string) {
    return this.http.delete(this.serviceUrl + "/" + id, this.config)
      .map(this.extractData)
      .catch(this.handleError);
  }

  ///
  /// Returns a list of Reasons base on they keyword order by it's weight.
  ///
  getListofReasonsForUser(keyword: string) {
    let params: URLSearchParams = new URLSearchParams();
    params.set('keywords', keyword);
    var config = new RequestOptions({ headers: this.headersOpt, search: params });
    config.search = params;
    return this.http.get(this.searchServiceUrl + "v1/search/keyword", config)
      .map(this.extractData)
      .catch(this.handleError);
  }

  ///
  /// Creates an array of timelogs
  ///
  createRangeLogs(dates: any[], typeid: string) {
    return this.http.put(this.bookingUrl, { "Dates": dates, "Typeid": typeid }, this.config)
      .map(this.extractData)
      .catch(this.handleError);
  }

  ///
  /// Extracts JSON data from server responses.
  ///
  private extractData(res: Response) {
    if (res.text().length == 0) {
      return {};
    }
    let body = res.json();
    return body || {};
  }

  ///
  /// Extract Auth Token
  ///
  public addToken(token) {
    this.temporalToken = token;
    this.headersOpt = new Headers({
      'Authorization': 'Bearer ' + this.temporalToken,
      'Accept': 'application/json;',
      "X-Testing": "testing"
    });
    this.config = new RequestOptions({ headers: this.headersOpt });

  }

  ///
  /// Callback for handling server responses.
  ///
  private handleError(error: Response | any) {

    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || '';
      const err = body.error || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.error(errMsg);
    return Observable.throw(errMsg);
  }
}
