// API Response Models matching backend ApiResponse<T>

export interface ApiResponse<T = void> {
  success: boolean;
  message: string;
  data?: T;
}

export interface LoginApiResponse {
  success: boolean;
  message: string;
  token?: string;
}
