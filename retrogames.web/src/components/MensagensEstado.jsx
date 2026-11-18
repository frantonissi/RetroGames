export function Carregando({ texto = 'Carregando…' }) {
  return (
    <div className="d-flex align-items-center gap-2 py-4" role="status">
      <span className="spinner-border spinner-border-sm" aria-hidden="true" />
      {texto}
    </div>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger d-flex flex-wrap align-items-center gap-3" role="alert">
      <span>{mensagem}</span>
      {aoTentarNovamente && (
        <button className="btn btn-sm btn-outline-danger" type="button" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  )
}

export function EstadoVazio({ titulo, children }) {
  return (
    <section className="bg-body border rounded-3 p-4">
      <h2 className="h5">{titulo}</h2>
      {children}
    </section>
  )
}
