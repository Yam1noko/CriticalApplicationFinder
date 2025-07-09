import { Component, OnInit } from '@angular/core';
import { Rule } from '../../models/rule';
import { RuleService } from '../../services/rule-service';
import { Alert } from '../../../models/alert.model';


@Component({
  selector: 'app-rule-management',
  standalone: false,
  templateUrl: './rule-management.component.html',
  styleUrls: ['./rule-management.component.css']
})
export class RuleManagementComponent implements OnInit {
  rules: Rule[] = [];
  loading = false;

  newRule: Rule = this.createEmptyRule();
  newRuleFullNames = '';
  newRuleSubstrings = '';

  alerts: Alert[] = [];

  constructor(private ruleService: RuleService) { }

  ngOnInit(): void {
    this.loadRules();
  }

  loadRules(): void {
    this.loading = true;
    this.ruleService.getRules().subscribe({
      next: (data) => {
        this.rules = data;
        this.loading = false;
        this.showAlert('Правила успешно загружены', 'success');
      },
      error: (err) => {
        console.error('Ошибка при загрузке правил:', err);
        this.loading = false;
        this.showAlert('Ошибка при загрузке правил', 'error');
      }
    });
  }

  addRule(): void {
    if (!this.newRule.name.trim()) {
      this.showAlert('Введите имя правила', 'error');
      return;
    }

    this.newRule.ruleFullNamesStr = this.newRuleFullNames;
    this.newRule.ruleSubstringsStr = this.newRuleSubstrings;

    this.ruleService.createRule(this.newRule).subscribe({
      next: (createdRule) => {
        if (createdRule) {
          this.showAlert('Правило успешно добавлено', 'success');
        } else {
          this.showAlert('Правило успешно добавлено', 'success');
          //this.showAlert('Правило добавлено, но сервер не вернул данные', 'success');
        }
        this.resetNewRule();
        this.loadRules();
      },
      error: (err) => {
        console.error('Ошибка при добавлении правила:', err);
        this.showAlert('Ошибка при добавлении правила', 'error');
      }
    });
  }

  updateRule(rule: Rule): void {
    if (!rule.id) {
      this.showAlert('Невозможно обновить правило без id', 'error');
      return;
    }

    this.ruleService.updateRule(rule).subscribe({
      next: (updatedRule) => {
        if (updatedRule) {
          this.showAlert('Правило успешно обновлено', 'success');
        } else {
          this.showAlert('Правило успешно обновлено', 'success');
          //this.showAlert('Правило обновлено, но сервер не вернул данные', 'success');
        }
        this.loadRules();
      },
      error: (err) => {
        console.error('Ошибка при обновлении правила:', err);
        this.showAlert('Ошибка при обновлении правила', 'error');
      }
    });
  }

  deleteRule(id: number): void {
    this.ruleService.deleteRule(id).subscribe({
      next: () => {
        this.showAlert('Правило удалено', 'success');
        this.loadRules();
      },
      error: (err) => {
        console.error('Ошибка при удалении правила:', err);
        this.showAlert('Ошибка при удалении правила', 'error');
      }
    });
  }

  private createEmptyRule(): Rule {
    return {
      id: 0,
      name: '',
      isActive: true,
      useAnd: true,
      ruleFullNames: [],
      ruleSubstrings: [],
      ruleFullNamesStr: '',
      ruleSubstringsStr: ''
    };
  }

  private resetNewRule(): void {
    this.newRule = this.createEmptyRule();
    this.newRuleFullNames = '';
    this.newRuleSubstrings = '';
  }

  showAlert(message: string, type: 'success' | 'error'): void {
    const alert: Alert = { message, type };
    this.alerts.push(alert);

    setTimeout(() => {
      this.alerts = this.alerts.filter(a => a !== alert);
    }, 4000);
  }
}
