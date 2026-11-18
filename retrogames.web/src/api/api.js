import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

api.interceptors.response.use(undefined, (erro) => {
  if (erro.response?.status === 401) {
    window.dispatchEvent(new Event('sessao:expirada'))
  }
  return Promise.reject(erro)
})

export default api
