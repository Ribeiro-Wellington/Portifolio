export interface Evento {
  id: string;
  aggregateId: string;
  eventType: string;
  eventData: string;
  version: number;
  timestamp: string;
  aggregateType: string;
  descricao: string;
  mudancas: { [key: string]: any };
  resumo: string;
}

export interface HistoricoCliente {
  clienteId: string;
  totalEventos: number;
  primeiroEvento: Evento;
  ultimoEvento: Evento;
  timeline: TimelineEvent[];
}

export interface TimelineEvent {
  ordem: number;
  versao: number;
  dataHora: string;
  tipo: string;
  descricao: string;
  resumo: string;
  mudancas: { [key: string]: any };
  dadosCompletos: string;
}

export interface EstatisticasEventos {
  totalEventos: number;
  tiposEventos: TipoEvento[];
  clientesUnicos: number;
  eventoMaisRecente?: Evento;
  eventoMaisAntigo?: Evento;
}

export interface TipoEvento {
  tipo: string;
  quantidade: number;
} 