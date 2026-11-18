import { NavLink, Outlet } from 'react-router-dom'

export default function Layout() {
  return (
    <div className="d-flex flex-column min-vh-100">
      <header className="barra-retro">
        <nav className="navbar navbar-expand-sm" data-bs-theme="dark" aria-label="Navegação principal">
          <div className="container">
            <NavLink className="navbar-brand fw-bold" to="/">Retro<span>Games</span></NavLink>
            <button className="navbar-toggler" type="button" data-bs-toggle="collapse"
              data-bs-target="#menu-principal" aria-controls="menu-principal"
              aria-expanded="false" aria-label="Alternar menu">
              <span className="navbar-toggler-icon" />
            </button>
            <div className="collapse navbar-collapse justify-content-end" id="menu-principal">
              <NavLink className="nav-link text-white py-2 px-sm-2" to="/" end>Catálogo</NavLink>
            </div>
          </div>
        </nav>
      </header>
      <main className="container flex-grow-1 py-5">
        <Outlet />
      </main>
      <footer className="border-top py-4 text-body-secondary">
        <div className="container small">RetroGames</div>
      </footer>
    </div>
  )
}
