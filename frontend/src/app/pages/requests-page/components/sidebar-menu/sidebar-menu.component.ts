import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-sidebar-menu',
  standalone: false,
  templateUrl: './sidebar-menu.component.html',
  styleUrls: ['./sidebar-menu.component.css']
})
export class SidebarMenuComponent {
  @Output() itemSelected = new EventEmitter<string>();

  select(item: string): void {
    this.itemSelected.emit(item);
  }
}
