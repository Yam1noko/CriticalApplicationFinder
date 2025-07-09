import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Rule } from '../models/rule';

@Injectable({
  providedIn: 'root'
})
export class RuleService {
  private apiUrl = 'http://localhost:5000/api/rules';

  constructor(private http: HttpClient) { }
  getRules(): Observable<Rule[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => this.fromServer(item)))
    );
  }

  createRule(rule: Rule): Observable<Rule | null> {
    const body = this.toServerFormat(rule, true);
    return this.http.post<any>(this.apiUrl, body).pipe(
      map(item => {
        if (!item) {
          console.warn('Сервер вернул null при создании правила');
          return null;
        }
        return this.fromServer(item);
      })
    );
  }

  updateRule(rule: Rule): Observable<Rule | null> {
    if (!rule.id) throw new Error('Rule must have an id');
    const body = this.toServerFormat(rule, false);
    return this.http.put<any>(`${this.apiUrl}/${rule.id}`, body).pipe(
      map(item => {
        if (!item) {
          console.warn('Сервер вернул null при обновлении правила');
          return null;
        }
        return this.fromServer(item);
      })
    );
  }

  deleteRule(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  private ensureStringArray(value: any): string[] {
    if (Array.isArray(value)) {
      return value.map(v => v != null ? String(v) : '').filter(v => v.length > 0);
    }
    return [];
  }

  private fromServer(item: any): Rule {
    if (!item || typeof item !== 'object') {
      throw new Error('Invalid server response: item is null or not an object');
    }

    const fullNames = this.ensureStringArray(item?.ruleFullNames);
    const substrings = this.ensureStringArray(item?.ruleSubstrings);

    return {
      id: item.id,
      name: item.name,
      isActive: item.isActive,
      useAnd: item.useAnd,
      ruleFullNames: fullNames,
      ruleSubstrings: substrings,
      ruleFullNamesStr: fullNames.join(', '),
      ruleSubstringsStr: substrings.join(', ')
    };
  }

  private toServerFormat(rule: Rule, isNew: boolean): any {
    return {
      id: isNew ? 0 : rule.id,
      name: rule.name,
      useAnd: rule.useAnd,
      isActive: rule.isActive,
      ruleFullNames: this.splitString(rule.ruleFullNamesStr),
      ruleSubstrings: this.splitString(rule.ruleSubstringsStr)
    };
  }

  private splitString(input?: string): string[] {
    return input
      ? input.split(',').map(s => s.trim()).filter(s => s.length > 0)
      : [];
  }
}
