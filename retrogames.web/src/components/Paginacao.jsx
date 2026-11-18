export default function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) return null

  const inicio = Math.max(1, Math.min(pagina - 2, totalPaginas - 4))
  const paginas = Array.from({ length: Math.min(totalPaginas, 5) }, (_, indice) => inicio + indice)

  return (
    <nav className="mt-4" aria-label="Paginação do catálogo">
      <ul className="pagination flex-wrap mb-0">
        <li className="page-item">
          <button className="page-link" type="button" disabled={pagina <= 1}
            onClick={() => aoMudar(Math.min(pagina - 1, totalPaginas))}>Anterior</button>
        </li>
        {paginas.map((numero) => (
          <li className={numero === pagina ? 'page-item active' : 'page-item'} key={numero}>
            <button className="page-link" type="button" aria-current={numero === pagina ? 'page' : undefined}
              aria-label={`Página ${numero}`} onClick={() => aoMudar(numero)}>{numero}</button>
          </li>
        ))}
        <li className="page-item">
          <button className="page-link" type="button" disabled={pagina >= totalPaginas}
            onClick={() => aoMudar(pagina + 1)}>Próxima</button>
        </li>
      </ul>
    </nav>
  )
}
