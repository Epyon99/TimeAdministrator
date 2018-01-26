import { provideRoutes, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { environment } from './environment';
const routes: Routes = [
    { path: '', component:  AppComponent},
    { path:'auth', redirectTo:environment.authServiceUrl },
];
 
export const appRouterProviders = [
    provideRoutes(routes)
];