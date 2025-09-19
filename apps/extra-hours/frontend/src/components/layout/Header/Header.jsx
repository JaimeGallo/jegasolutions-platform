import { useState, useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../utils/useAuth";
import "./Header.scss";
import ChangePasswordModal from "../../auth/ChangePasswordModal/ChangePasswordModal";
import { Home, User, ChevronDown, LogOut, Key } from "lucide-react";

const Header = () => {
  const { auth, logout, getUserName } = useAuth();
  const navigate = useNavigate();
  const [showDropdown, setShowDropdown] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const dropdownRef = useRef(null);

  const toggleDropdown = () => {
    setShowDropdown(!showDropdown);
  };

  const handleLogout = async () => {
    try {
      await logout();
      setShowDropdown(false);
    } catch (error) {
      console.error("Error during logout:", error);
    }
  };

  const openModal = () => {
    setIsModalOpen(true);
    setShowDropdown(false);
  };

  const closeModal = () => {
    setIsModalOpen(false);
  };

  // Cerrar dropdown al hacer clic fuera
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setShowDropdown(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  const getRoleDisplayName = (role) => {
    switch (role) {
      case "superusuario":
        return "Administrador";
      case "manager":
        return "Supervisor";
      case "empleado":
        return "Empleado";
      default:
        return role;
    }
  };

  return (
    <>
      <header className="app-header">
        <div className="header-content">
          {/* Brand Section */}
          <div className="brand-section" onClick={() => navigate("/menu")}>
            <div className="logo-container">
              <Home className="brand-icon" />
            </div>
            <div className="brand-info">
              <h1 className="brand-title">INICIO</h1>
              <span className="brand-subtitle">Panel de control</span>
            </div>
          </div>

          {/* User Section */}
          {auth && (
            <div className="user-section" ref={dropdownRef}>
              <div className="user-profile" onClick={toggleDropdown}>
                <div className="user-avatar styled-avatar">
                  <User size={20} />
                </div>
                <div className="user-details">
                  <span className="user-name">
                    {getUserName() || auth.uniqueName || "Usuario"}
                  </span>
                  <span className="user-role">
                    {getRoleDisplayName(auth.role)}
                  </span>
                </div>
                <ChevronDown
                  className={`dropdown-arrow ${showDropdown ? "rotated" : ""}`}
                  size={16}
                />
              </div>

              {showDropdown && (
                <div className="user-dropdown">
                  <div className="dropdown-header">
                    <div className="dropdown-user-info">
                      <span className="dropdown-name">
                        {getUserName() || auth.uniqueName || "Usuario"}
                      </span>
                      <span className="dropdown-role">
                        {getRoleDisplayName(auth.role)}
                      </span>
                    </div>
                  </div>

                  <div className="dropdown-divider"></div>

                  <button className="dropdown-item" onClick={openModal}>
                    <Key size={16} />
                    <span>Cambiar Contraseña</span>
                  </button>

                  <div className="dropdown-divider"></div>

                  <button
                    className="dropdown-item logout-item"
                    onClick={handleLogout}
                  >
                    <LogOut size={16} />
                    <span>Cerrar Sesión</span>
                  </button>
                </div>
              )}
            </div>
          )}
        </div>
      </header>

      {isModalOpen && <ChangePasswordModal onClose={closeModal} />}
    </>
  );
};

export default Header;
