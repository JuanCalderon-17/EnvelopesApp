export interface Envelope {
  id: number;
  code: string;
  amount: number;
  recordedAt: string;
  workerId: number;
  workerName: string;
  kiosko: string;
}

export interface LoginResponse {
  token: string;
  expiration: string;
  workerName: string;
  workerId: number;
}

export interface CreateEnvelopeDto {
  code: string;
  amount: number;
  workerId: number;
}

export interface UpdateEnvelopeDto {
  code: string;
  amount: number;
}
