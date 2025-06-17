import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';

import { Cliente } from '../../models/cliente.model';
import { Evento } from '../../models/evento.model';
import { ClienteService } from '../../services/cliente.service';
import { EventoService } from '../../services/evento.service';

@Component({
  selector: 'app-cliente-detalhes',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTabsModule,
    MatChipsModule
  ],
  templateUrl: './cliente-detalhes.component.html',
  styleUrl: './cliente-detalhes.component.scss'
})
export class ClienteDetalhesComponent implements OnInit {
  cliente: Cliente | null = null;
  eventos: Evento[] = [];
  loading = false;
  loadingEventos = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private clienteService: ClienteService,
    private eventoService: EventoService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.carregarCliente(id);
      this.carregarEventos(id);
    }
  }

  private carregarCliente(id: string): void {
    this.loading = true;
    this.clienteService.obterPorId(id).subscribe({
      next: (cliente) => {
        this.cliente = cliente;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar cliente:', error);
        this.snackBar.open('Erro ao carregar cliente', 'Fechar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private carregarEventos(clienteId: string): void {
    this.loadingEventos = true;
    this.eventoService.obterEventosPorCliente(clienteId).subscribe({
      next: (eventos) => {
        this.eventos = eventos;
        this.loadingEventos = false;
      },
      error: (error) => {
        console.error('Erro ao carregar eventos:', error);
        this.snackBar.open('Erro ao carregar histórico', 'Fechar', { duration: 3000 });
        this.loadingEventos = false;
      }
    });
  }

  removerCliente(): void {
    if (this.cliente && confirm(`Tem certeza que deseja remover o cliente "${this.cliente.nome}"?`)) {
      this.clienteService.remover(this.cliente.id).subscribe({
        next: () => {
          this.snackBar.open('Cliente removido com sucesso', 'Fechar', { duration: 3000 });
          this.router.navigate(['/clientes']);
        },
        error: (error) => {
          console.error('Erro ao remover cliente:', error);
          this.snackBar.open('Erro ao remover cliente', 'Fechar', { duration: 3000 });
        }
      });
    }
  }

  formatarDocumento(documento: string, isPessoaJuridica: boolean): string {
    if (isPessoaJuridica) {
      return documento.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
    } else {
      return documento.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
    }
  }

  formatarTelefone(telefone: string): string {
    return telefone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
  }

  formatarData(data: string): string {
    if (!data) return '';
    return new Date(data).toLocaleDateString('pt-BR');
  }

  formatarDataHora(data: string): string {
    if (!data) return '';
    return new Date(data).toLocaleString('pt-BR');
  }

  getTipoCliente(): string {
    if (!this.cliente) return '';
    return this.cliente.isPessoaJuridica ? 'Pessoa Jurídica' : 'Pessoa Física';
  }

  getStatusInscricao(): string {
    if (!this.cliente) return '';
    if (this.cliente.isPessoaJuridica) {
      return this.cliente.isento ? 'Isento' : (this.cliente.inscricaoEstadual || 'Não informado');
    }
    return 'N/A';
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

  getChangesArray(mudancas: { [key: string]: any }): { key: string; value: any }[] {
    return Object.entries(mudancas).map(([key, value]) => ({ key, value }));
  }

  hasChanges(mudancas: { [key: string]: any }): boolean {
    return mudancas && Object.keys(mudancas).length > 0;
  }
} 