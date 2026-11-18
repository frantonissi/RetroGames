import { useEffect, useState } from 'react'
import { Link, useLocation, useParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros.js'
import { jogoApi } from '../../api/servicos.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'
import { formatarPreco } from '../../lib/formato.js'

export default function DetalheJogo() {
  const { id } = useParams()
  const { state } = useLocation()

  return <ConteudoDetalhe key={id} id={id} consultaCatalogo={state?.consultaCatalogo} />
}

function ConteudoDetalhe({ id, consultaCatalogo }) {
  const [resultado, definirResultado] = useState(null)
  const [tentativa, definirTentativa] = useState(0)

  useEffect(() => {
    const controlador = new AbortController()
    jogoApi.buscarPorId(id, controlador.signal).then((jogo) => {
      if (!controlador.signal.aborted) definirResultado({ jogo })
    }).catch((erro) => {
      if (!controlador.signal.aborted) {
        definirResultado({ erro: obterMensagemErro(erro), inexistente: erro.response?.status === 404 })
      }
    })
    return () => controlador.abort()
  }, [id, tentativa])

  const jogo = resultado?.jogo

  return (
    <>
      <Link className="d-inline-block mb-4" to={{
        pathname: '/',
        search: consultaCatalogo ? `?${consultaCatalogo}` : '',
      }}>Voltar ao catálogo</Link>
      {!resultado && <Carregando texto="Carregando jogo…" />}
      {resultado?.erro && (
        <MensagemErro mensagem={resultado.erro}
          aoTentarNovamente={resultado.inexistente ? undefined : () => {
            definirResultado(null)
            definirTentativa((valor) => valor + 1)
          }} />
      )}
      {jogo && (
        <article className="card cartao-jogo shadow-sm">
          <div className="card-body p-4 p-md-5">
            <p className="plataforma-jogo fw-semibold mb-2">{jogo.nomePlataforma}</p>
            <h1 className="display-5 fw-bold mb-4">{jogo.nome}</h1>
            <dl className="row gy-3 mb-4">
              <div className="col-6 col-md-3">
                <dt>Ano de lançamento</dt>
                <dd>{jogo.anoLancamento}</dd>
              </div>
              <div className="col-6 col-md-3">
                <dt>Condição</dt>
                <dd>{jogo.descricaoCondicao}</dd>
              </div>
              <div className="col-6 col-md-3">
                <dt>Preço</dt>
                <dd>{formatarPreco(jogo.preco)}</dd>
              </div>
              <div className="col-6 col-md-3">
                <dt>Estoque</dt>
                <dd>{jogo.estoque === 0 ? 'Esgotado' : jogo.estoque}</dd>
              </div>
            </dl>
            {jogo.generos.length > 0 && (
              <section className="mb-4">
                <h2 className="h6">Gêneros</h2>
                <ul className="list-unstyled d-flex flex-wrap gap-2 mb-0">
                  {jogo.generos.map(({ id, nome }) => (
                    <li className="badge rounded-pill text-bg-light border" key={id}>{nome}</li>
                  ))}
                </ul>
              </section>
            )}
            {jogo.descricao?.trim() && (
              <section>
                <h2 className="h6">Descrição</h2>
                <p className="mb-0 text-break" style={{ whiteSpace: 'pre-wrap' }}>{jogo.descricao}</p>
              </section>
            )}
          </div>
        </article>
      )}
    </>
  )
}
