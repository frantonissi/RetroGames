import api from './api.js'

async function consultar(rota, opcoes) {
  const resposta = await api.get(rota, opcoes)
  return resposta.data.data
}

export const jogoApi = {
  listar: (filtros, signal) => consultar('/Jogo', { params: filtros, signal }),
  buscarPorId: (id, signal) => consultar(`/Jogo/${encodeURIComponent(id)}`, { signal }),
}

export const plataformaApi = {
  listar: (signal) => consultar('/Plataforma', { signal }),
}

export const generoApi = {
  listar: (signal) => consultar('/Genero', { signal }),
}

export const condicaoJogoApi = {
  listar: (signal) => consultar('/CondicaoJogo', { signal }),
}
