import { TestBed, inject } from '@angular/core/testing';

import { TimeLogService } from './time-log.service';

describe('TimeLogServiceService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TimeLogService]
    });
  });

  it('should ...', inject([TimeLogService], (service: TimeLogService) => {
    expect(service).toBeTruthy();
  }));
});
