import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App Mónaco', () => {
  it('debería crear la aplicación', async () => {
    await TestBed.configureTestingModule({ imports: [App] }).compileComponents();
    const fixture = TestBed.createComponent(App);
    expect(fixture.componentInstance).toBeTruthy();
  });
});
