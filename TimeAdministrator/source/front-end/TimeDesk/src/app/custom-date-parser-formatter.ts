import { NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { DatePipe } from '@angular/common';

const errorMessage = "Invalid date. Check the format.";
const emptyMessage = "Empty Date."

/// This class work over the NgbDateParserFormatter
/// So the format on the Datepickers can be customized.
export class CustomDateParserFormatter extends NgbDateParserFormatter {

    datePipe = new DatePipe('en-US');

    constructor(
        private dateFormatString: string) {
        super();
    }

    ///
    /// it formats the Datepicker selected value over the inputs.
    /// as a new DatePipe
    format(date: NgbDateStruct): string {
        if (date === null) {
            //returns empty string to the input box.
            return '';
        }
        try {
            return this.datePipe.transform(new Date(date.year, date.month - 1, date.day), 'dd.MM.yyyy');
        } catch (e) {
            //returns invalid date formta to the input box.
            return errorMessage;
        }
    }

    formatForServer(date: NgbDateStruct): string {
        if (date === null) {
            // returns empty to the datepicker input
            return '';
        }
        try {
            return this.datePipe.transform(new Date(date.year, date.month - 1, date.day), 'y-MM-dd');
        } catch (e) {
            // returns invalid date to the datepickerpicker Input
            return errorMessage;
        }
    }

    parse(value: string): NgbDateStruct {
        let returnVal: NgbDateStruct;
        if (!value) {
            returnVal = null;
        } else {
            try {
                let dateParts = this.datePipe.transform(value, 'M-d-y').split('-');
                returnVal = { year: parseInt(dateParts[2]), month: parseInt(dateParts[0]), day: parseInt(dateParts[1]) };
            } catch (e) {
                returnVal = null;
            }
        }
        return returnVal;
    }
}