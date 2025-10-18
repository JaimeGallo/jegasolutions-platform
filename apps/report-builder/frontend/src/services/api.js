import axios from 'axios';
import { getApiBaseUrl } from './apiConfig';

const API_BASE_URL = getApiBaseUrl();

// Create axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
api.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      // Solo limpiar token si NO es un token SSO válido
      const isSSOValidated = localStorage.getItem('ssoValidated') === 'true';
      
      if (!isSSOValidated) {
        // Token normal expirado o inválido
        localStorage.removeItem('token');
        window.location.href = '/login';
      } else {
        // Token SSO válido pero backend no lo reconoce
        // No limpiar el token, solo loggear el error
        console.warn('⚠️ SSO token not recognized by backend, but keeping session');
      }
    }
    return Promise.reject(error);
  }
);

export default api;
