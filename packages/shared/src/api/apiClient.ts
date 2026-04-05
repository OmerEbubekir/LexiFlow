import axios from 'axios';
import { getAuth } from 'firebase/auth';

const apiClient = axios.create({
  // Base URL should ideally be injected at the app level,
  // but we can provide a fallback or expect it to be defined in process.env
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use(
  async (config) => {
    try {
      // getAuth() relies on Firebase being initialized in the consuming app
      const auth = getAuth();
      const user = auth.currentUser;
      
      if (user) {
        const token = await user.getIdToken();
        config.headers.Authorization = `Bearer ${token}`;
      }
    } catch (error) {
      // Ignore errors if Firebase is not initialized yet or similar issues
      console.warn('Firebase Auth could not provide a token:', error);
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export { apiClient };
