import axios from "axios";

const API_URL = "https://localhost:7259/api/auth"; 
export const register = async (username, password) => {
  return axios.post(`${API_URL}/register`, { username, password });
};

export const login = async (username, password) => {
  return axios.post(`${API_URL}/login`, { username, password });
};
