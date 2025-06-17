import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { NgxMaskDirective, NgxMaskPipe } from 'ngx-mask';

import { Cliente, CriarClienteRequest, AtualizarClienteRequest } from '../../models/cliente.model';
import { ClienteService } from '../../services/cliente.service';

@Component({
  selector: 'app-cliente-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    NgxMaskDirective,
    NgxMaskPipe
  ],
  templateUrl: './cliente-form.component.html',
  styleUrl: './cliente-form.component.scss'
})
export class ClienteFormComponent implements OnInit {
  form: FormGroup;
  loading = false;
  isEditMode = false;
  clienteId: string | null = null;

  estados = [
    'AC', 'AL', 'AP', 'AM', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA',
    'MT', 'MS', 'MG', 'PA', 'PB', 'PR', 'PE', 'PI', 'RJ', 'RN',
    'RS', 'RO', 'RR', 'SC', 'SP', 'SE', 'TO'
  ];

  constructor(
    private fb: FormBuilder,
    private clienteService: ClienteService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.form = this.criarFormulario();
  }

  ngOnInit(): void {
    this.clienteId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.clienteId;

    if (this.isEditMode && this.clienteId) {
      this.carregarCliente(this.clienteId);
    }
  }

  private criarFormulario(): FormGroup {
    return this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      documento: ['', [Validators.required, Validators.minLength(11)]],
      isPessoaJuridica: [false],
      inscricaoEstadual: [''],
      isento: [false],
      dataNascimento: ['', [Validators.required, this.minAnoNascimentoValidator]],
      telefone: ['', [Validators.required, Validators.minLength(10)]],
      email: ['', [Validators.required, Validators.email]],
      cep: ['', [Validators.required, (control: AbstractControl): ValidationErrors | null => {
        if (!control.value) return null;
        
        const cepRaw = control.value.toString();
        const cepDigits = cepRaw.replace(/\D/g, '');

        if (cepDigits.length !== 8 || isNaN(Number(cepDigits))) {
          return { cepFormat: true };
        }

        return null;
      }]],
      endereco: ['', Validators.required],
      numero: ['', [Validators.required, Validators.min(0)]],
      bairro: ['', Validators.required],
      cidade: ['', Validators.required],
      estado: ['', Validators.required]
    });
  }

  private carregarCliente(id: string): void {
    this.loading = true;
    this.clienteService.obterPorId(id).subscribe({
      next: (cliente) => {
        this.form.patchValue({
          nome: cliente.nome,
          documento: cliente.documento,
          isPessoaJuridica: cliente.isPessoaJuridica,
          inscricaoEstadual: cliente.inscricaoEstadual || '',
          isento: cliente.isento,
          dataNascimento: this.formatDateFromBackendToFrontend(cliente.dataNascimento),
          telefone: cliente.telefone,
          email: cliente.email,
          cep: cliente.cep,
          endereco: cliente.endereco,
          numero: cliente.numero,
          bairro: cliente.bairro,
          cidade: cliente.cidade,
          estado: cliente.estado
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar cliente:', error);
        this.snackBar.open('Erro ao carregar cliente', 'Fechar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.loading = true;
      const formValue = this.form.value;

      if (this.isEditMode && this.clienteId) {
        const clienteAtualizado: AtualizarClienteRequest = {
          id: this.clienteId,
          nome: formValue.nome,
          telefone: this.formatTelefoneToBackend(formValue.telefone),
          email: formValue.email,
          cep: this.formatCepToBackend(formValue.cep),
          endereco: formValue.endereco,
          numero: formValue.numero,
          bairro: formValue.bairro,
          cidade: formValue.cidade,
          estado: formValue.estado,
          inscricaoEstadual: formValue.inscricaoEstadual || undefined,
          isento: formValue.isento
        };

        this.clienteService.atualizar(clienteAtualizado).subscribe({
          next: () => {
            this.snackBar.open('Cliente atualizado com sucesso', 'Fechar', { duration: 3000 });
            this.router.navigate(['/clientes', this.clienteId]);
            this.loading = false;
          },
          error: (error) => {
            console.error('Erro ao atualizar cliente:', error);
            this.snackBar.open('Erro ao atualizar cliente', 'Fechar', { duration: 3000 });
            this.loading = false;
          }
        });
      } else {
        const novoCliente: CriarClienteRequest = {
          nome: formValue.nome,
          documento: formValue.documento,
          isPessoaJuridica: formValue.isPessoaJuridica,
          inscricaoEstadual: formValue.inscricaoEstadual || undefined,
          isento: formValue.isento,
          dataNascimento: formValue.isPessoaJuridica ? '1990-01-01' : this.formatDateToBackend(formValue.dataNascimento),
          telefone: this.formatTelefoneToBackend(formValue.telefone),
          email: formValue.email,
          cep: this.formatCepToBackend(formValue.cep),
          endereco: formValue.endereco,
          numero: formValue.numero,
          bairro: formValue.bairro,
          cidade: formValue.cidade,
          estado: formValue.estado
        };

        this.clienteService.criar(novoCliente).subscribe({
          next: (id) => {
            this.snackBar.open('Cliente criado com sucesso', 'Fechar', { duration: 3000 });
            this.router.navigate(['/clientes']);
            this.loading = false;
          },
          error: (error) => {
            console.error('Erro ao criar cliente:', error);
            this.snackBar.open('Erro ao criar cliente', 'Fechar', { duration: 3000 });
            this.loading = false;
          }
        });
      }
    } else {
      Object.keys(this.form.controls).forEach(key => {
        const control = this.form.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
    }
  }

  onTipoPessoaChange(): void {
    const isPessoaJuridica = this.form.get('isPessoaJuridica')?.value;
    const documentoControl = this.form.get('documento');
    const dataNascimentoControl = this.form.get('dataNascimento');
    
    if (isPessoaJuridica) {
      documentoControl?.setValidators([Validators.required, Validators.minLength(14)]);
      dataNascimentoControl?.clearValidators();
    } else {
      documentoControl?.setValidators([Validators.required, Validators.minLength(11)]);
      dataNascimentoControl?.setValidators([Validators.required, this.minAnoNascimentoValidator]);
    }
    
    documentoControl?.updateValueAndValidity();
    dataNascimentoControl?.updateValueAndValidity();
  }

  cancelar(): void {
    this.router.navigate(['/clientes']);
  }

  private formatDateToBackend(date: Date | string): string {
    let year, month, day;

    if (typeof date === 'string') {
      const cleanDate = date.replace(/\D/g, '');

      if (cleanDate.length === 8) {
        day = parseInt(cleanDate.substring(0, 2), 10);
        month = parseInt(cleanDate.substring(2, 4), 10);
        year = parseInt(cleanDate.substring(4, 8), 10);
      } else if (date.includes('/') && date.split('/').length === 3) {
        const parts = date.split('/');
        day = parseInt(parts[0], 10);
        month = parseInt(parts[1], 10);
        year = parseInt(parts[2], 10);
      } else {
        const d = new Date(date);
        if (isNaN(d.getTime())) return '';
        year = d.getFullYear();
        month = d.getMonth() + 1;
        day = d.getDate();
      }
    } else if (date instanceof Date) {
      year = date.getFullYear();
      month = date.getMonth() + 1;
      day = date.getDate();
    } else {
      return '';
    }

    if (isNaN(year) || isNaN(month) || isNaN(day) || year < 1900 || month < 1 || month > 12 || day < 1 || day > 31) {
      return '';
    }

    const formattedMonth = (`0${month}`).slice(-2);
    const formattedDay = (`0${day}`).slice(-2);
    return `${year}-${formattedMonth}-${formattedDay}`;
  }

  private formatDateFromBackendToFrontend(dateString: string): string {
    if (!dateString) return '';
    const parts = dateString.split('-');
    const year = parts[0];
    const month = parts[1];
    const day = parts[2];
    return `${day}/${month}/${year}`;
  }

  private minAnoNascimentoValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;

    if (typeof control.value === 'string') {
      if (control.value.length !== 10) {
        return null;
      }

      const partes = control.value.split('/');
      if (partes.length !== 3) {
        return { minDate: true, reason: 'Formato inválido (Esperado: dd/MM/yyyy)' };
      }

      const dia = parseInt(partes[0], 10);
      const mes = parseInt(partes[1], 10) - 1;
      const ano = parseInt(partes[2], 10);

      if (isNaN(dia) || isNaN(mes) || isNaN(ano)) {
        return { minDate: true, reason: 'Valores numéricos inválidos.' };
      }

      if (ano < 1900) {
        return { minDate: true, reason: 'O ano deve ser maior ou igual a 1900.' };
      }

      const data = new Date(ano, mes, dia);

      if (data.getFullYear() !== ano || data.getMonth() !== mes || data.getDate() !== dia) {
        return { minDate: true, reason: 'Data inexistente ou inválida.' };
      }

      return null;
    } else if (control.value instanceof Date) {
      if (control.value.getFullYear() < 1900) {
        return { minDate: true, reason: 'O ano deve ser maior ou igual a 1900.' };
      }
      return null;
    }

    return null;
  }

  private formatCepToBackend(cep: string): string {
    if (!cep) return '';
    const digitsOnly = cep.replace(/\D/g, '');
    if (digitsOnly.length === 8) {
      return `${digitsOnly.substring(0, 5)}-${digitsOnly.substring(5, 8)}`;
    }
    return cep;
  }

  private formatTelefoneToBackend(telefone: string): string {
    if (!telefone) return '';
    const digitsOnly = telefone.replace(/\D/g, '');

    if (digitsOnly.length === 11) {
      return `(${digitsOnly.substring(0, 2)}) ${digitsOnly.substring(2, 7)}-${digitsOnly.substring(7, 11)}`;
    } else if (digitsOnly.length === 10) {
      return `(${digitsOnly.substring(0, 2)}) ${digitsOnly.substring(2, 6)}-${digitsOnly.substring(6, 10)}`;
    }
    return telefone;
  }
} 