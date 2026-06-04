import { CdkMenuBar } from '@angular/cdk/menu';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CdkMenuBar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  items = [
    { label: 'Games', icon: 'pi pi-box', routerLink: '/games' },
    { label: 'Genres', icon: 'pi pi-tag', routerLink: '/genres' },
    { label: 'Platforms', icon: 'pi pi-desktop', routerLink: '/platforms' },
    { label: 'Publishers', icon: 'pi pi-building', routerLink: '/publishers' }
  ];
}
