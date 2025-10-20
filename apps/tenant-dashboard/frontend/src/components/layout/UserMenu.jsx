import { useState } from 'react';
import { User, Settings, LogOut, Key } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import ChangePasswordModal from '../auth/ChangePasswordModal';
import './UserMenu.css';

const UserMenu = () => {
  const { user, logout } = useAuth();
  const [showMenu, setShowMenu] = useState(false);
  const [showChangePasswordModal, setShowChangePasswordModal] = useState(false);

  const handleChangePassword = () => {
    setShowMenu(false);
    setShowChangePasswordModal(true);
  };

  const handleLogout = () => {
    setShowMenu(false);
    logout();
  };

  return (
    <div className="user-menu">
      <button className="user-button" onClick={() => setShowMenu(!showMenu)}>
        <User className="h-5 w-5" />
        <span>{user?.name || user?.email}</span>
      </button>

      {showMenu && (
        <>
          <div className="menu-backdrop" onClick={() => setShowMenu(false)} />
          <div className="user-dropdown">
            <div className="user-info">
              <p className="user-name">{user?.name}</p>
              <p className="user-email">{user?.email}</p>
            </div>

            <div className="menu-divider" />

            <button onClick={handleChangePassword} className="menu-item">
              <Key className="h-4 w-4" />
              <span>Cambiar Contraseña</span>
            </button>

            <button
              onClick={() => {
                setShowMenu(false);
              }}
              className="menu-item"
            >
              <Settings className="h-4 w-4" />
              <span>Configuración</span>
            </button>

            <div className="menu-divider" />

            <button onClick={handleLogout} className="menu-item logout">
              <LogOut className="h-4 w-4" />
              <span>Cerrar Sesión</span>
            </button>
          </div>
        </>
      )}

      <ChangePasswordModal
        isOpen={showChangePasswordModal}
        onClose={() => setShowChangePasswordModal(false)}
      />
    </div>
  );
};

export default UserMenu;
