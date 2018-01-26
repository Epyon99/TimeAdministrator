import { Timeline } from '../timeline/timeline';
import { TipoLogTiempo } from './TipoLogTiempo';

export class Timeform {

    public id: number;

    public date: any;

    public listOfWorklines: Array<Timeline> = [];

    public listOfTimelines: Array<Timeline> = [];

    public utcDate: any;

    public baseTipoLogTiempo: TipoLogTiempo;

    /**
     *
     */
    constructor() {        
        this.listOfTimelines = [];
        this.listOfWorklines = [];
    }

}