import { LogTiempoPage } from './app.po';

describe('time-desk App', () => {
  let page: LogTiempoPage;

  beforeEach(() => {
    page = new LogTiempoPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
