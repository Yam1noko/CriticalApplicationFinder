export interface TicketFilter {
  fromDate?: Date;
  toDate?: Date;
  requesters?: string[];     
  searchText?: string;
  subjectWords?: string[];
  containsWords?: string[];
  criticalOnly?: boolean;
}
