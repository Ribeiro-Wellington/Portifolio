import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';

import { Evento } from '../../models/evento.model';
import { EventoService } from '../../services/evento.service';

@Component({
  selector: 'app-evento-lista',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatChipsModule
  ],
  templateUrl: './evento-lista.component.html',
  styleUrl: './evento-lista.component.scss'
})
export class EventoListaComponent implements OnInit {
  eventos: Evento[] = [];
  displayedColumns: string[] = ['timestamp', 'eventType', 'descricao', 'aggregateId', 'version', 'acoes'];
  loading = false;
  filtroForm: FormGroup;

  tiposEventos = [
    'ClienteCriadoEvent',
    'ClienteAtualizadoEvent',
    'ClienteRemovidoEvent'
  ];

  constructor(
    private eventoService: EventoService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.filtroForm = this.criarFormularioFiltro();
  }

  ngOnInit(): void {
    this.carregarEventos();
  }

  private criarFormularioFiltro(): FormGroup {
    return this.fb.group({
      aggregateId: [''],
      eventType: [''],
      dataInicio: [''],
      dataFim: [''],
      limite: [100]
    });
  }

  carregarEventos(): void {
    this.loading = true;
    const filtros = this.filtroForm.value;

    this.eventoService.obterEventos(
      filtros.aggregateId || undefined,
      filtros.eventType || undefined,
      filtros.dataInicio || undefined,
      filtros.dataFim || undefined,
      filtros.limite || 100
    ).subscribe({
      next: (eventos) => {
        this.eventos = eventos;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar eventos:', error);
        this.snackBar.open('Erro ao carregar eventos', 'Fechar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  limparFiltros(): void {
    this.filtroForm.reset({
      aggregateId: '',
      eventType: '',
      dataInicio: '',
      dataFim: '',
      limite: 100
    });
    this.carregarEventos();
  }

  formatarDataHora(data: Date): string {
    return new Date(data).toLocaleString('pt-BR');
  }

  getEventIcon(eventType: string): string {
    switch (eventType) {
      case 'ClienteCriadoEvent':
        return 'person_add';
      case 'ClienteAtualizadoEvent':
        return 'edit';
      case 'ClienteRemovidoEvent':
        return 'delete';
      default:
        return 'event';
    }
  }

  getEventColor(eventType: string): string {
    switch (eventType) {
      case 'ClienteCriadoEvent':
        return 'primary';
      case 'ClienteAtualizadoEvent':
        return 'accent';
      case 'ClienteRemovidoEvent':
        return 'warn';
      default:
        return 'primary';
    }
  }

  getEventLabel(eventType: string): string {
    switch (eventType) {
      case 'ClienteCriadoEvent':
        return 'Criação';
      case 'ClienteAtualizadoEvent':
        return 'Atualização';
      case 'ClienteRemovidoEvent':
        return 'Remoção';
      default:
        return eventType;
    }
  }

  visualizarDetalhes(evento: Evento): void {
    console.log('Detalhes do evento:', evento);
    this.snackBar.open('Funcionalidade de detalhes em desenvolvimento', 'Fechar', { duration: 3000 });
  }
} 