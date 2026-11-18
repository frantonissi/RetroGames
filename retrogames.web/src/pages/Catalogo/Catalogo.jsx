import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { generoApi, jogoApi, plataformaApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import CardJogo from '../../components/CardJogo.jsx'
import Paginacao from '../../components/Paginacao.jsx'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx'

const tamanhoPagina = 12

export default function Catalogo() {
  const [parametros, definirParametros] = useSearchParams()
  const busca = parametros.get('busca') ?? ''
  const idPlataforma = parametros.get('idPlataforma') ?? ''
  const idGenero = parametros.get('idGenero') ?? ''
  const paginaInformada = Number(parametros.get('pagina'))
  const pagina = Number.isInteger(paginaInformada) && paginaInformada > 0 && paginaInformada <= 2147483647
    ? paginaInformada : 1

  const [opcoes, definirOpcoes] = useState(null)
  const [resultado, definirResultado] = useState(null)
  const [tentativaOpcoes, definirTentativaOpcoes] = useState(0)
  const [tentativaJogos, definirTentativaJogos] = useState(0)
  const chaveConsulta = JSON.stringify([busca, idPlataforma, idGenero, pagina, tentativaJogos])

  useEffect(() => {
    const controlador = new AbortController()
    const consultar = async () => {
      try {
        const [plataformas, generos] = await Promise.all([
          plataformaApi.listar(controlador.signal),
          generoApi.listar(controlador.signal),
        ])
        if (!controlador.signal.aborted) definirOpcoes({ plataformas, generos })
      } catch (erro) {
        if (!controlador.signal.aborted) definirOpcoes({ erro: obterMensagemErro(erro) })
      }
    }
    consultar()
    return () => controlador.abort()
  }, [tentativaOpcoes])

  useEffect(() => {
    const controlador = new AbortController()
    const consultar = async () => {
      try {
        const dados = await jogoApi.listar({
          busca: busca || undefined,
          idPlataforma: idPlataforma || undefined,
          idGenero: idGenero || undefined,
          pagina,
          tamanhoPagina,
        }, controlador.signal)
        if (!controlador.signal.aborted) definirResultado({ chave: chaveConsulta, dados })
      } catch (erro) {
        if (!controlador.signal.aborted) definirResultado({ chave: chaveConsulta, erro: obterMensagemErro(erro) })
      }
    }
    consultar()
    return () => controlador.abort()
  }, [busca, idPlataforma, idGenero, pagina, chaveConsulta])

  function alterarParametros(nome, valor) {
    const novos = new URLSearchParams(parametros)
    if (valor) novos.set(nome, valor)
    else novos.delete(nome)
    if (nome !== 'pagina') novos.delete('pagina')
    if (novos.toString() !== parametros.toString()) definirParametros(novos)
  }

  function pesquisar(evento) {
    evento.preventDefault()
    const formulario = new FormData(evento.currentTarget)
    alterarParametros('busca', formulario.get('busca').trim())
  }

  const atual = resultado?.chave === chaveConsulta ? resultado : null

  return (
    <>
      <h1 className="display-5 fw-bold mb-4">Catálogo de jogos</h1>
      <form className="row g-3 align-items-end mx-0 mb-4 px-2 pb-3 bg-body border rounded-3"
        role="search" onSubmit={pesquisar}>
        <div className="col-12 col-lg">
          <label className="form-label" htmlFor="busca">Nome do jogo</label>
          <div className="input-group">
            <input className="form-control" id="busca" name="busca" type="search"
              key={busca} defaultValue={busca} />
            <button className="btn btn-primario" type="submit">Buscar</button>
          </div>
        </div>
        <div className="col-12 col-sm-6 col-lg-3">
          <label className="form-label" htmlFor="plataforma">Plataforma</label>
          <select className="form-select" id="plataforma" value={idPlataforma}
            disabled={!opcoes?.plataformas}
            onChange={(evento) => alterarParametros('idPlataforma', evento.target.value)}>
            <option value="">Todas</option>
            {opcoes?.plataformas?.map(({ id, nome }) => <option key={id} value={id}>{nome}</option>)}
          </select>
        </div>
        <div className="col-12 col-sm-6 col-lg-3">
          <label className="form-label" htmlFor="genero">Gênero</label>
          <select className="form-select" id="genero" value={idGenero}
            disabled={!opcoes?.generos}
            onChange={(evento) => alterarParametros('idGenero', evento.target.value)}>
            <option value="">Todos</option>
            {opcoes?.generos?.map(({ id, nome }) => <option key={id} value={id}>{nome}</option>)}
          </select>
        </div>
      </form>
      {!opcoes && <Carregando texto="Carregando filtros…" />}
      {opcoes?.erro && !atual?.erro && (
        <MensagemErro mensagem={opcoes.erro} aoTentarNovamente={() => {
          definirOpcoes(null)
          definirTentativaOpcoes((valor) => valor + 1)
        }} />
      )}
      {!atual && <Carregando texto="Carregando jogos…" />}
      {atual?.erro && (
        <MensagemErro mensagem={atual.erro} aoTentarNovamente={() => {
          definirTentativaJogos((valor) => valor + 1)
          if (opcoes?.erro) {
            definirOpcoes(null)
            definirTentativaOpcoes((valor) => valor + 1)
          }
        }} />
      )}
      {atual?.dados && (
        <>
          {atual.dados.itens.length === 0 ? (
            <EstadoVazio titulo="Nenhum jogo encontrado">
              <p className="mb-0">Tente outra busca ou ajuste os filtros.</p>
            </EstadoVazio>
          ) : (
            <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 g-3">
              {atual.dados.itens.map((jogo) => (
                <div className="col" key={jogo.id}>
                  <CardJogo jogo={jogo} consultaCatalogo={parametros.toString()} />
                </div>
              ))}
            </div>
          )}
          <Paginacao pagina={atual.dados.pagina} totalPaginas={atual.dados.totalPaginas}
            aoMudar={(numero) => alterarParametros('pagina', numero === 1 ? '' : String(numero))} />
        </>
      )}
    </>
  )
}
