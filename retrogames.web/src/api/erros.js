export function obterMensagemErro(erro) {
  if (!erro.response || [502, 503, 504].includes(erro.response.status)) {
    return erro.response?.data?.errorMessage || 'Não foi possível conectar ao servidor.'
  }

  return erro.response.data?.errorMessage || 'Não foi possível concluir a consulta.'
}

export function obterErrosDosCampos(erro) {
  const envelope = erro.response?.data
  return envelope?.responseCode === 3 ? envelope.data ?? {} : {}
}
