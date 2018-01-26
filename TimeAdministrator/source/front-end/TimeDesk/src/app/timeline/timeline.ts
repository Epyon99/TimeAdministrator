export class Timeline {
    public id: string;

    public timeFormId: string;

    public startTime: Date;

    public endTime: Date;

    public type: string;

    ///
    /// The color of the timeline as a string
    /// this is inserted in the CSS. Can be the hex value.
    public color: string;

    public TipoLogTiempoName : string;

    ///
    /// Length is a percentage that indicates the css width atribute of
    /// the progress bar that represents a timeline. Must finish with %
    public length: string;

    public startTimeReason: string;

    public endTimeReason: string;

    public startTimeModel : any;

    public endTimeModel : any;

    public isDateIncomplete : boolean;

    ///
    /// Used as a accesor to the numeric part of Length.
    ///
    getSize(): number {
        if (this.length == "" || this.length == undefined) {
            return 2;
        } else {
            var t = this.length.substring(0, this.length.length - 1);
            return parseInt(t);
        }
    }

    isEditable(): boolean {
        if (this.type != "fill"){
            return false;
        }
        else {
            return true;
        }
    }
}