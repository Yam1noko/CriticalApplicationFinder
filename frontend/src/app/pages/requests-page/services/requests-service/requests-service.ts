import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Ticket } from '../../models/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class RequestsService {
  private baseUrl = 'http://localhost:5000/api/requests/';  

  constructor(private http: HttpClient) { }


  getTickets(from: Date, to: Date): Observable<Ticket[]> {
    const fromStr = from.toISOString();
    const toStr = to.toISOString();
    const url = `${this.baseUrl}range?from=${fromStr}&to=${toStr}`;
    return this.http.get<any[]>(url).pipe(
      map(serverData => serverData.map(item => this.mapServerToTicket(item)))
    );
  }

  private mapServerToTicket(item: any): Ticket {
    return {
      ticketNumber: item.id,
      requester: item.clientName,
      shortDescr: item.shortDescr,
      description: item.descriptionRtf4096,
      link: this.generateLink(item.title),
      createdAt: new Date(item.creationDate),
      critical: item.isCritical,
      isExpanded: false
    };
  }

  private generateLink(id: string): string {
    const cleanId = (id || '').replace(/\s+/g, '');
    return `https://sd.fesco.com/sd/operator/#uuid:serviceCall$${cleanId}`;
  }
}
