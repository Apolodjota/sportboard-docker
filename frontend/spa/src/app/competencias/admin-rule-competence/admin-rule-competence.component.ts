import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { RuleCompetenceService } from '../../services/competencies/rule-competence.service';
import { CompetenceService } from '../../services/competencies/competence.service';
import { RuleCompetition } from '../../models/competencies/rule-competence.model';
import { Competence } from '../../models/competencies/competence.model';
import { FlashMessageComponent } from '../../shared/flash-message/flash-message.component';
import { SharedModule } from '../../shared/shared.module';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BreadcrumbCompetenciasComponent } from '../breadcrumb-competencias/breadcrumb-competencias.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-rule-competence',
  standalone: true,
  imports: [
    SharedModule,
    MatDialogModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    BreadcrumbCompetenciasComponent,
  ],
  templateUrl: './admin-rule-competence.component.html',
  styleUrls: ['./admin-rule-competence.component.scss']
})
export class AdminRuleCompetenceComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  dataSource!: MatTableDataSource<RuleCompetition>;
  competences: Competence[] = [];
  selectedCompetence: Competence | null = null;
  selectedRule: RuleCompetition | null = null;
  form: FormGroup;
  isLoading = false;
  isDeleteMode = false;
  searchText = '';
  displayedColumns: string[] = ['numeration', 'rule_description', 'actor', 'action', 'type_rule'];
  currentRoute: string = '';

  constructor(
    private ruleCompetenceService: RuleCompetenceService,
    private competenceService: CompetenceService,
    private dialog: MatDialog,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.form = this.fb.group({
      competence: new FormControl({value: '', disabled: false}, [Validators.required]),
      numeration: new FormControl({value: '', disabled: false}, [
        Validators.required, 
        Validators.min(1)
      ]),
      rule_description: new FormControl({value: '', disabled: false}, [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(500)
      ]),
      actor: new FormControl({value: '', disabled: false}, [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]),
      action: new FormControl({value: '', disabled: false}, [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]),
      type_rule: new FormControl({value: '', disabled: false}, [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50)
      ])
    });
    this.currentRoute = this.router.url;
  }

  ngOnInit(): void {
    this.loadCompetences();
  }

  loadCompetences(): void {
    this.isLoading = true;
    this.competenceService.getCompetences().subscribe({
      next: (data) => {
        this.competences = data;
        this.isLoading = false;
      },
      error: (error) => {
        this.handleError(error);
        this.isLoading = false;
      }
    });
  }

  onCompetenceChange(event: any): void {
    const competenceId = event.value;
    this.competenceService.getCompetence(competenceId).subscribe({
      next: (competence) => {
        this.selectedCompetence = competence;
        this.loadRules();
      },
      error: (error) => {
        this.handleError(error);
      }
    });
  }

  loadRules(): void {
    if (this.selectedCompetence) {
      this.isLoading = true;
      this.competenceService.getCompetence(this.selectedCompetence.id).subscribe({
        next: (updatedCompetence) => {
          this.selectedCompetence = updatedCompetence;
          this.dataSource = new MatTableDataSource(updatedCompetence.rule_list || []);
          this.dataSource.sort = this.sort;
          this.configureDataSource();
          this.isLoading = false;
        },
        error: (error) => {
          this.handleError(error);
          this.isLoading = false;
        }
      });
    }
  }

  private configureDataSource(): void {
    if (this.dataSource) {
      this.dataSource.filterPredicate = (data: RuleCompetition, filter: string) => {
        return data.rule_description.toLowerCase().includes(filter.toLowerCase());
      };
      this.dataSource.sort = this.sort;
    }
  }

  create(): void {
    this.selectedRule = null;
    this.form.enable(); // Usar el método enable() del FormGroup
    this.form.reset();
    if (this.selectedCompetence) {
      this.form.patchValue({ competence: this.selectedCompetence.id });
    }
    this.isDeleteMode = false;
  }

  onSelect(rule: RuleCompetition): void {
    this.selectedRule = rule;
    this.form.patchValue({
      ...rule,
      competence: this.selectedCompetence?.id
    });
    this.isDeleteMode = false;
  }

  delete(): void {
    this.isDeleteMode = true;
    this.form.disable(); // Usar el método disable() del FormGroup
  }

  confirmDelete(): void {
    if (this.selectedRule && this.selectedCompetence) {
      this.isLoading = true;
      this.ruleCompetenceService.deleteRuleCompetence(this.selectedRule.numeration).subscribe({
        next: () => {
          if (this.selectedCompetence && this.selectedCompetence.rule_list) {
            this.selectedCompetence.rule_list = this.selectedCompetence.rule_list.filter(
              rule => rule.numeration !== this.selectedRule?.numeration
            );
            this.updateCompetenceRules();
          }
          this.openFlashMessage('Regla eliminada exitosamente', 'success');
          this.onCancel();
        },
        error: (error) => {
          this.handleError(error);
          this.isLoading = false;
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.valid && this.selectedCompetence) {
      const ruleData = {
        numeration: this.form.get('numeration')?.value,
        rule_description: this.form.get('rule_description')?.value,
        actor: this.form.get('actor')?.value,
        action: this.form.get('action')?.value,
        type_rule: this.form.get('type_rule')?.value,
        competence: this.selectedCompetence.id
      };

      this.isLoading = true;
      if (this.selectedRule) {
        this.updateRule(ruleData);
      } else {
        this.createRule(ruleData);
      }
    } else {
      this.markFormGroupTouched(this.form);
      this.openFlashMessage('Por favor, complete todos los campos requeridos correctamente', 'warning');
    }
  }

  private updateRule(ruleData: any): void {
    if (this.selectedRule) {
      this.ruleCompetenceService.updateRuleCompetence(this.selectedRule.numeration, ruleData).subscribe({
        next: (updatedRule) => {
          this.handleRuleUpdateSuccess(updatedRule);
        },
        error: (error) => {
          this.handleError(error);
          this.isLoading = false;
        }
      });
    }
  }

  private createRule(ruleData: any): void {
    this.ruleCompetenceService.createRuleCompetence(ruleData).subscribe({
      next: (newRule) => {
        if (this.selectedCompetence) {
          // Añadir la nueva regla a la lista de reglas de la competencia
          if (!this.selectedCompetence.rule_list) {
            this.selectedCompetence.rule_list = [];
          }
          this.selectedCompetence.rule_list.push(newRule);
          
          // Actualizar la competencia con la nueva regla
          this.updateCompetenceWithNewRule(newRule);
        }
      },
      error: (error) => {
        this.handleError(error);
        this.isLoading = false;
      }
    });
  }

  private updateCompetenceWithNewRule(newRule: RuleCompetition): void {
    if (this.selectedCompetence) {
      const competenceData = new FormData();
      competenceData.append('id', this.selectedCompetence.id.toString());
      competenceData.append('name', this.selectedCompetence.name);
      competenceData.append('description', this.selectedCompetence.description);
      
      // No enviar el logo si es una URL existente
      if (this.selectedCompetence.logo && !this.selectedCompetence.logo.startsWith('http')) {
        competenceData.append('logo', this.selectedCompetence.logo);
      }

      // Actualizar la lista de reglas incluyendo la nueva
      if (this.selectedCompetence.rule_list) {
        competenceData.append('rule_list', JSON.stringify(this.selectedCompetence.rule_list));
      }

      // Usar PATCH en lugar de PUT para actualización parcial
      this.competenceService.partialUpdateCompetence(this.selectedCompetence.id, competenceData).subscribe({
        next: () => {
          this.openFlashMessage('Regla creada y asignada exitosamente', 'success');
          this.loadRules();
          this.onCancel();
        },
        error: (error) => {
          this.handleError(error);
          this.isLoading = false;
        }
      });
    }
  }

  private handleRuleUpdateSuccess(updatedRule: RuleCompetition): void {
    if (this.selectedCompetence && this.selectedCompetence.rule_list) {
      const index = this.selectedCompetence.rule_list.findIndex(
        r => r.numeration === this.selectedRule?.numeration
      );
      if (index !== -1) {
        this.selectedCompetence.rule_list[index] = updatedRule;
        this.updateCompetenceRules();
      }
    }
    this.openFlashMessage('Regla actualizada exitosamente', 'success');
    this.onCancel();
  }

  private updateCompetenceRules(): void {
    if (this.selectedCompetence) {
      const competenceData = new FormData();
      competenceData.append('id', this.selectedCompetence.id.toString());
      competenceData.append('name', this.selectedCompetence.name);
      competenceData.append('description', this.selectedCompetence.description);
      
      // No enviar el logo en la actualización de reglas
      if (this.selectedCompetence.rule_list) {
        competenceData.append('rule_list', JSON.stringify(this.selectedCompetence.rule_list));
      }

      this.competenceService.partialUpdateCompetence(this.selectedCompetence.id, competenceData).subscribe({
        next: () => {
          this.loadRules();
          this.isLoading = false;
        },
        error: (error) => {
          this.handleError(error);
          this.isLoading = false;
        }
      });
    }
  }

  onCancel(): void {
    this.form.enable(); // Usar el método enable() del FormGroup
    this.form.reset();
    this.selectedRule = null;
    this.isDeleteMode = false;
    if (this.selectedCompetence) {
      this.form.patchValue({ competence: this.selectedCompetence.id });
    }
  }

  refresh(): void {
    this.searchText = '';
    if (this.dataSource) {
      this.dataSource.filter = '';
    }
    this.loadRules();
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    if (this.dataSource) {
      this.dataSource.filter = filterValue.trim().toLowerCase();
    }
  }

  sortData(sort: Sort): void {
    if (!sort.active || sort.direction === '' || !this.dataSource) {
      return;
    }

    this.dataSource.data = this.dataSource.data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'numeration': return this.compare(a.numeration, b.numeration, isAsc);
        case 'rule_description': return this.compare(a.rule_description, b.rule_description, isAsc);
        case 'actor': return this.compare(a.actor, b.actor, isAsc);
        case 'action': return this.compare(a.action, b.action, isAsc);
        case 'type_rule': return this.compare(a.type_rule, b.type_rule, isAsc);
        default: return 0;
      }
    });
  }

  private compare(a: number | string, b: number | string, isAsc: boolean): number {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  getErrorMessage(controlName: string): string {
    const control = this.form.get(controlName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) {
        return 'Este campo es requerido';
      }
      if (control.errors['minlength']) {
        return `Mínimo ${control.errors['minlength'].requiredLength} caracteres`;
      }
      if (control.errors['maxlength']) {
        return `Máximo ${control.errors['maxlength'].requiredLength} caracteres`;
      }
      if (control.errors['min']) {
        return `El valor mínimo es ${control.errors['min'].min}`;
      }
    }
    return '';
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  private openFlashMessage(message: string, type: 'success' | 'error' | 'info' | 'warning'): void {
    const dialogRef = this.dialog.open(FlashMessageComponent, {
      width: '400px',
      data: {
        message,
        type,
        duration: 3000,
        position: 'top-right'
      }
    });

    setTimeout(() => {
      dialogRef.close();
    }, 3000);
  }

  private handleError(error: any): void {
    let errorMessage = 'Ha ocurrido un error';
    if (error.error?.message) {
      errorMessage = error.error.message;
    } else if (error.status === 404) {
      errorMessage = 'Recurso no encontrado';
    } else if (error.status === 403) {
      errorMessage = 'No tiene permisos para realizar esta acción';
    }
    this.openFlashMessage(errorMessage, 'error');
  }
}
