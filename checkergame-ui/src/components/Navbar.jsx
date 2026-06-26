import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <nav style={{ padding: "1rem", background: "#eee" }}>
      <Link to="/" style={{ marginRight: "1rem" }}>Home</Link>
      <Link to="/register" style={{ marginRight: "1rem" }}>Register</Link>
      <Link to="/login" style={{ marginRight: "1rem" }}>Login</Link>
      <Link to="/lobby" style={{ marginRight: "1rem" }}>Game Lobby</Link>
    </nav>
  );
}
