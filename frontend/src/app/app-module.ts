import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { RequestsTableComponent } from './pages/requests-page/components/requests-table/requests-table.component';
import { RequestsFilterComponent } from './pages/requests-page/components/requests-filter/requests-filter.component';
import { SidebarMenuComponent } from './pages/requests-page/components/sidebar-menu/sidebar-menu.component';
import { Modal } from './pages/modal-page/modal/modal';
import { RuleManagementComponent } from './pages/modal-page/components/rule-management/rule-management.component';
import { NotificationSettingsComponent } from './pages/modal-page/components/notification-settings/notification-settings.component';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    App,
    RequestsTableComponent,
    RequestsFilterComponent,
    SidebarMenuComponent,
    Modal,
    RuleManagementComponent,
    NotificationSettingsComponent,
    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners()
  ],
  bootstrap: [App]
})
export class AppModule { }
