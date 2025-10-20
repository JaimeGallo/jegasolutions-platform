import { useState } from 'react';
import { X, Lock, Eye, EyeOff, AlertCircle, CheckCircle } from 'lucide-react';
import authService from '../../services/authService';
import './ChangePasswordModal.css';

const ChangePasswordModal = ({ isOpen, onClose }) => {
  const [formData, setFormData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  const [showPasswords, setShowPasswords] = useState({
    current: false,
    new: false,
    confirm: false,
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleChange = e => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
    setError(''); // Limpiar errores al escribir
  };

  const togglePasswordVisibility = field => {
    setShowPasswords({
      ...showPasswords,
      [field]: !showPasswords[field],
    });
  };

  const validateForm = () => {
    // Validar que todos los campos estén llenos
    if (
      !formData.currentPassword ||
      !formData.newPassword ||
      !formData.confirmPassword
    ) {
      setError('Todos los campos son obligatorios');
      return false;
    }

    // Validar longitud mínima
    if (formData.newPassword.length < 8) {
      setError('La nueva contraseña debe tener al menos 8 caracteres');
      return false;
    }

    // Validar que las contraseñas coincidan
    if (formData.newPassword !== formData.confirmPassword) {
      setError('Las contraseñas nuevas no coinciden');
      return false;
    }

    // Validar que la nueva contraseña sea diferente a la actual
    if (formData.currentPassword === formData.newPassword) {
      setError('La nueva contraseña debe ser diferente a la actual');
      return false;
    }

    return true;
  };

  const handleSubmit = async e => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setLoading(true);
    setError('');
    setSuccess('');

    try {
      await authService.changePassword(
        formData.currentPassword,
        formData.newPassword
      );

      setSuccess('Contraseña actualizada correctamente');

      // Cerrar modal después de 2 segundos
      setTimeout(() => {
        onClose();
        setFormData({
          currentPassword: '',
          newPassword: '',
          confirmPassword: '',
        });
        setSuccess('');
      }, 2000);
    } catch (err) {
      setError(err.message || 'Error al cambiar la contraseña');
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    setFormData({
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    });
    setError('');
    setSuccess('');
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={handleClose}>
      <div className="modal-content" onClick={e => e.stopPropagation()}>
        {/* Header */}
        <div className="modal-header">
          <div className="modal-title">
            <Lock className="h-6 w-6 text-jega-blue-600" />
            <h2>Cambiar Contraseña</h2>
          </div>
          <button
            className="close-button"
            onClick={handleClose}
            disabled={loading}
            aria-label="Cerrar modal"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* Body */}
        <form onSubmit={handleSubmit} className="modal-body">
          {/* Contraseña Actual */}
          <div className="form-group">
            <label htmlFor="currentPassword">Contraseña Actual</label>
            <div className="password-input-wrapper">
              <Lock className="input-icon" />
              <input
                type={showPasswords.current ? 'text' : 'password'}
                id="currentPassword"
                name="currentPassword"
                value={formData.currentPassword}
                onChange={handleChange}
                placeholder="Ingrese su contraseña actual"
                disabled={loading}
                required
                autoComplete="current-password"
              />
              <button
                type="button"
                className="password-toggle"
                onClick={() => togglePasswordVisibility('current')}
                disabled={loading}
                aria-label="Mostrar/ocultar contraseña actual"
              >
                {showPasswords.current ? (
                  <EyeOff className="h-5 w-5" />
                ) : (
                  <Eye className="h-5 w-5" />
                )}
              </button>
            </div>
          </div>

          {/* Nueva Contraseña */}
          <div className="form-group">
            <label htmlFor="newPassword">Nueva Contraseña</label>
            <div className="password-input-wrapper">
              <Lock className="input-icon" />
              <input
                type={showPasswords.new ? 'text' : 'password'}
                id="newPassword"
                name="newPassword"
                value={formData.newPassword}
                onChange={handleChange}
                placeholder="Mínimo 8 caracteres"
                disabled={loading}
                required
                autoComplete="new-password"
              />
              <button
                type="button"
                className="password-toggle"
                onClick={() => togglePasswordVisibility('new')}
                disabled={loading}
                aria-label="Mostrar/ocultar nueva contraseña"
              >
                {showPasswords.new ? (
                  <EyeOff className="h-5 w-5" />
                ) : (
                  <Eye className="h-5 w-5" />
                )}
              </button>
            </div>
          </div>

          {/* Confirmar Nueva Contraseña */}
          <div className="form-group">
            <label htmlFor="confirmPassword">Confirmar Nueva Contraseña</label>
            <div className="password-input-wrapper">
              <Lock className="input-icon" />
              <input
                type={showPasswords.confirm ? 'text' : 'password'}
                id="confirmPassword"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
                placeholder="Repita la nueva contraseña"
                disabled={loading}
                required
                autoComplete="new-password"
              />
              <button
                type="button"
                className="password-toggle"
                onClick={() => togglePasswordVisibility('confirm')}
                disabled={loading}
                aria-label="Mostrar/ocultar confirmación"
              >
                {showPasswords.confirm ? (
                  <EyeOff className="h-5 w-5" />
                ) : (
                  <Eye className="h-5 w-5" />
                )}
              </button>
            </div>
          </div>

          {/* Mensajes de error/éxito */}
          {error && (
            <div className="alert alert-error">
              <AlertCircle className="h-5 w-5" />
              <span>{error}</span>
            </div>
          )}

          {success && (
            <div className="alert alert-success">
              <CheckCircle className="h-5 w-5" />
              <span>{success}</span>
            </div>
          )}

          {/* Botones */}
          <div className="modal-footer">
            <button
              type="button"
              className="btn-secondary"
              onClick={handleClose}
              disabled={loading}
            >
              Cancelar
            </button>
            <button type="submit" className="btn-primary" disabled={loading}>
              {loading ? 'Cambiando...' : 'Cambiar Contraseña'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ChangePasswordModal;
