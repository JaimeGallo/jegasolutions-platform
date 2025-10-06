import React from "react";
import BulkUserUpload from "../../components/management/BulkUserUpload/BulkUserUpload";
import "./BulkUploadPage.scss";

const BulkUploadPage = () => {
  return (
    <div className="bulk-upload-page">
      <div className="page-header">
        <h2>Carga Masiva de Usuarios</h2>
        <p className="page-description">
          Importe múltiples empleados al sistema de forma simultánea usando archivos CSV o Excel
        </p>
      </div>

      <div className="bulk-upload-content">
        <section className="info-section">
          <article>
            <h3>📝 Instrucciones de Uso</h3>
            <p>
              Este módulo permite cargar múltiples empleados simultáneamente al sistema 
              mediante archivos CSV o Excel, agilizando el proceso de onboarding.
            </p>
            
            <h4>Formato del Archivo:</h4>
            <ul>
              <li><strong>Columnas requeridas:</strong> Id, Nombre, Email, Cargo</li>
              <li><strong>Columnas opcionales:</strong> Salario, Rol, Username, Password, ManagerId</li>
              <li><strong>Formatos aceptados:</strong> .csv, .xlsx, .xls</li>
              <li><strong>Tamaño máximo:</strong> 10 MB</li>
            </ul>

            <h4>Validaciones Automáticas:</h4>
            <ul>
              <li>✅ Verificación de IDs únicos</li>
              <li>✅ Validación de formato de email</li>
              <li>✅ Comprobación de existencia de managers</li>
              <li>✅ Detección de registros duplicados</li>
            </ul>

            <h4>Contraseñas por Defecto:</h4>
            <p>
              Si no se especifica una contraseña en el archivo, el sistema asignará 
              automáticamente <code>password123</code> como contraseña temporal.
              Se recomienda que los usuarios cambien esta contraseña en su primer acceso.
            </p>

            <p className="warning-note">
              ⚠️ <strong>Importante:</strong> Los cambios realizados mediante carga masiva 
              afectan directamente a la base de datos. Revise cuidadosamente el archivo 
              antes de cargarlo.
            </p>
          </article>
        </section>

        <section className="upload-section">
          <BulkUserUpload />
        </section>
      </div>
    </div>
  );
};

export default BulkUploadPage;