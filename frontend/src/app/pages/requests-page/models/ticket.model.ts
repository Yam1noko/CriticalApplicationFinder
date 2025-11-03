export interface Ticket {
  ticketNumber: string;
  requester: string;
  shortDescr: string;
  description: string;
  link: string;
  createdAt: Date;
  critical?: boolean;
  isExpanded?: boolean;
  isTruncated?: boolean;
}
