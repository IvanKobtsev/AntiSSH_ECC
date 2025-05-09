import axios from "axios";
import { apiLink } from "./common/constants.ts";

const api = axios.create({
  baseURL: apiLink,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      // localStorage.removeItem("token"); // Clear token
      // window.location.href = "/login"; // Redirect to login
    }
    return Promise.reject(error);
  },
);

export default api;
