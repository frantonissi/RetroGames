const moeda = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })
const dataHora = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' })

export function formatarPreco(valor) {
  return moeda.format(valor)
}

export function formatarDataHora(valor) {
  return dataHora.format(new Date(valor))
}
