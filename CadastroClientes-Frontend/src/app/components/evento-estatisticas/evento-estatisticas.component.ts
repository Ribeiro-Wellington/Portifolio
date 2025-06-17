import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';

import { EstatisticasEventos, TipoEvento } from '../../models/evento.model';
import { EventoService } from '../../services/evento.service';

@Component({
  selector: 'app-evento-estatisticas',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatIconModule,
    MatChipsModule
  ],
  templateUrl: './evento-estatisticas.component.html',
  styleUrl: './evento-estatisticas.component.scss'
})
export class EventoEstatisticasComponent implements OnInit {
  estatisticas: EstatisticasEventos | null = null;
  loading = false;

  constructor(
    private eventoService: EventoService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.carregarEstatisticas();
  }

  carregarEstatisticas(): void {
    this.loading = true;
    this.eventoService.obterEstatisticas().subscribe({
      next: (estatisticas) => {
        this.estatisticas = estatisticas;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
        this.snackBar.open('Erro ao carregar estatísticas', 'Fechar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  formatarDataHora(data: string): string {
    if (!data) return '';
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
        return 'Criações';
      case 'ClienteAtualizadoEvent':
        return 'Atualizações';
      case 'ClienteRemovidoEvent':
        return 'Remoções';
      default:
        return eventType;
    }
  }

  getBarColor(eventType: string): string {
    switch (eventType) {
      case 'ClienteCriadoEvent':
        return '#1976d2';
      case 'ClienteAtualizadoEvent':
        return '#ff9800';
      case 'ClienteRemovidoEvent':
        return '#f44336';
      default:
        return '#1976d2';
    }
  }

  calcularPercentual(quantidade: number): number {
    if (!this.estatisticas || this.estatisticas.totalEventos === 0) return 0;
    return Math.round((quantidade / this.estatisticas.totalEventos) * 100);
  }
} 