import { browser, element, by } from 'protractor';

export class LogTiempoPage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('Epy-LogTiempo-root h1')).getText();
  }
}
