export interface TicketFilter {
  fromDate?: Date;
  toDate?: Date;
  requesters?: string[];
  searchText?: string;
  criticalOnly?: boolean;
}
