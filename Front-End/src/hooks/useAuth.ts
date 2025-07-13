import { useState, useEffect } from "react";
import { useDispatch } from "react-redux";
import MathEditorApiService from "@/services/api";
import { LoginRequest, RegisterRequest } from "@/types/api";
import { loadSession } from "@/store/app";

export const useAuth = () => {
  const dispatch = useDispatch();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const apiService = MathEditorApiService.getInstance();

  const login = async (credentials: LoginRequest) => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await apiService.login(credentials);

      if (response.success) {
        // Reload session to update the Redux store
        dispatch(loadSession() as any);
        return response.data;
      } else {
        throw new Error(response.message || "Login failed");
      }
    } catch (err: any) {
      setError(err.message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (userData: RegisterRequest) => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await apiService.register(userData);

      if (response.success) {
        // Reload session to update the Redux store
        dispatch(loadSession() as any);
        return response.data;
      } else {
        throw new Error(response.message || "Registration failed");
      }
    } catch (err: any) {
      setError(err.message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    apiService.logout();
    // Reload session to update the Redux store
    dispatch(loadSession() as any);
  };

  const isAuthenticated = () => {
    return !!apiService.getToken();
  };

  return {
    login,
    register,
    logout,
    isAuthenticated,
    isLoading,
    error,
  };
};
