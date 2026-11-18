import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato.js'

export default function CardJogo({ jogo, consultaCatalogo }) {
  return (
    <article className="card cartao-jogo h-100 shadow-sm">
      <div className="card-body d-flex flex-column p-4">
        <p className="plataforma-jogo small fw-semibold mb-2">{jogo.nomePlataforma}</p>
        <h2 className="h5 mb-2">{jogo.nome}</h2>
        <p className="small text-body-secondary mb-2">
          {[jogo.anoLancamento, jogo.descricaoCondicao].filter(Boolean).join(' · ')}
        </p>
        <ul className="list-unstyled d-flex flex-wrap gap-1 mb-4" aria-label="Gêneros">
          {jogo.generos.map((genero) => (
            <li className="badge rounded-pill text-bg-light border" key={genero}>{genero}</li>
          ))}
        </ul>
        <div className="mt-auto d-flex flex-wrap align-items-center gap-2">
          <strong className="fs-5">{formatarPreco(jogo.preco)}</strong>
          {jogo.estoque === 0 && <span className="badge text-bg-secondary">Esgotado</span>}
        </div>
        <Link className="btn btn-primario align-self-start mt-3" to={`/jogos/${jogo.id}`}
          state={{ consultaCatalogo }} aria-label={`Ver detalhes de ${jogo.nome}`}>
          Ver detalhes
        </Link>
      </div>
    </article>
  )
}
