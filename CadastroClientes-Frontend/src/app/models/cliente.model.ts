export interface Cliente {
  id: string;
  nome: string;
  documento: string;
  isPessoaJuridica: boolean;
  inscricaoEstadual?: string;
  isento: boolean;
  dataNascimento: string;
  telefone: string;
  email: string;
  cep: string;
  endereco: string;
  numero: string;
  bairro: string;
  cidade: string;
  estado: string;
  dataCadastro: string;
  dataAtualizacao?: string;
}

export interface CriarClienteRequest {
  nome: string;
  documento: string;
  isPessoaJuridica: boolean;
  inscricaoEstadual?: string;
  isento: boolean;
  dataNascimento: string;
  telefone: string;
  email: string;
  cep: string;
  endereco: string;
  numero: string;
  bairro: string;
  cidade: string;
  estado: string;
}

export interface AtualizarClienteRequest {
  id: string;
  nome: string;
  telefone: string;
  email: string;
  cep: string;
  endereco: string;
  numero: string;
  bairro: string;
  cidade: string;
  estado: string;
  inscricaoEstadual?: string;
  isento: boolean;
} 