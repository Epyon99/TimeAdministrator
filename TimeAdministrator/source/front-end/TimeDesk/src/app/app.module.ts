import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule, Http } from '@angular/http';
import { NgbModule, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { Ng2AutoCompleteModule } from 'ng2-auto-complete';

import { AppComponent } from './app.component';
import { TimelineComponent } from './timeline/timeline.component';
import { TimeformComponent } from './timeform/timeform.component';
import { CustomDateParserFormatter } from './custom-date-parser-formatter';
import { Timeform } from './timeform/timeform';
import { RangeDatePickerComponent } from './range-date-picker/range-date-picker.component';

@NgModule({
  declarations: [
    AppComponent,
    TimelineComponent,
    TimeformComponent,
    RangeDatePickerComponent,
  ],
  imports: [
    NgbModule.forRoot(),
    BrowserModule,
    FormsModule,
    HttpModule,
    Ng2AutoCompleteModule
  ],
  bootstrap: [AppComponent],
  providers: [{provide: NgbDateParserFormatter, useFactory: CustomDateParser}]
})
export class AppModule { }
export function CustomDateParser():CustomDateParserFormatter{
  return new CustomDateParserFormatter('longDate');
}
