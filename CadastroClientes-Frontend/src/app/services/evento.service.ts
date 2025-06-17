import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Evento, HistoricoCliente, EstatisticasEventos } from '../models/evento.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventoService {
  private readonly apiUrl = `${environment.apiUrl}/eventos`;

  constructor(private http: HttpClient) { }

  obterEventos(
    aggregateId?: string,
    eventType?: string,
    dataInicio?: Date,
    dataFim?: Date,
    limite?: number
  ): Observable<Evento[]> {
    let params = new HttpParams();
    
    if (aggregateId) {
      params = params.set('aggregateId', aggregateId);
    }
    if (eventType) {
      params = params.set('eventType', eventType);
    }
    if (dataInicio) {
      params = params.set('dataInicio', dataInicio.toISOString());
    }
    if (dataFim) {
      params = params.set('dataFim', dataFim.toISOString());
    }
    if (limite) {
      params = params.set('limite', limite.toString());
    }

    return this.http.get<Evento[]>(this.apiUrl, { params });
  }

  obterEventosPorCliente(clienteId: string): Observable<Evento[]> {
    return this.http.get<Evento[]>(`${this.apiUrl}/cliente/${clienteId}`);
  }

  obterHistoricoCliente(clienteId: string): Observable<HistoricoCliente> {
    return this.http.get<HistoricoCliente>(`${this.apiUrl}/cliente/${clienteId}/historico`);
  }

  obterEstatisticas(): Observable<EstatisticasEventos> {
    return this.http.get<EstatisticasEventos>(`${this.apiUrl}/estatisticas`);
  }
} 