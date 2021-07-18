import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { routes as docsRoutes } from '../tab1/tab1-routing.module';
import { TabsPage } from './tabs.page';
export const routes: Routes = [
    {
        path: 'tabs',
        component: TabsPage,
        children: [
            {
                path: 'docs',
                children: docsRoutes,
            },
            {
                path: 'tab2',
                loadChildren: () =>
                    import('../tab2/tab2.module').then((m) => m.Tab2PageModule),
            },
            {
                path: 'tab3',
                loadChildren: () =>
                    import('../tab3/tab3.module').then((m) => m.Tab3PageModule),
            },
            {
                path: '',
                redirectTo: '/tabs/docs',
                pathMatch: 'full',
            },
        ],
    },
    {
        path: '',
        redirectTo: '/tabs/docs',
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
})
export class TabsPageRoutingModule {}
