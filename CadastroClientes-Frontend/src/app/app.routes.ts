import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/clientes', pathMatch: 'full' },
  { path: 'clientes', loadComponent: () => import('./components/cliente-lista/cliente-lista.component').then(m => m.ClienteListaComponent) },
  { path: 'clientes/novo', loadComponent: () => import('./components/cliente-form/cliente-form.component').then(m => m.ClienteFormComponent) },
  { path: 'clientes/:id', loadComponent: () => import('./components/cliente-detalhes/cliente-detalhes.component').then(m => m.ClienteDetalhesComponent) },
  { path: 'clientes/:id/editar', loadComponent: () => import('./components/cliente-form/cliente-form.component').then(m => m.ClienteFormComponent) },
  { path: 'eventos', loadComponent: () => import('./components/evento-lista/evento-lista.component').then(m => m.EventoListaComponent) },
  { path: 'eventos/estatisticas', loadComponent: () => import('./components/evento-estatisticas/evento-estatisticas.component').then(m => m.EventoEstatisticasComponent) },
  { path: '**', redirectTo: '/clientes' }
];
