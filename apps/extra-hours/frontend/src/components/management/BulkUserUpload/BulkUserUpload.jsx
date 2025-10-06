import React, { useState } from "react";
import { Upload, Button, message, Alert, Table, Divider, Card } from "antd";
import { UploadOutlined, DownloadOutlined, InboxOutlined } from "@ant-design/icons";
import { API_CONFIG } from "../../../environments/api.config";
import { getAuthHeadersForFormData } from "../../../environments/http-headers";
import "./BulkUserUpload.scss";

const { Dragger } = Upload;

const BulkUserUpload = () => {
  const [fileList, setFileList] = useState([]);
  const [uploading, setUploading] = useState(false);
  const [uploadResults, setUploadResults] = useState(null);

  // Configuración del componente Upload
  const uploadProps = {
    name: "file",
    multiple: false,
    fileList,
    accept: ".csv,.xlsx,.xls",
    beforeUpload: (file) => {
      const isValidFormat =
        file.type === "text/csv" ||
        file.type === "application/vnd.ms-excel" ||
        file.type === "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

      if (!isValidFormat) {
        message.error("Solo se permiten archivos CSV o Excel (.xlsx, .xls)");
        return false;
      }

      const isLt10M = file.size / 1024 / 1024 < 10;
      if (!isLt10M) {
        message.error("El archivo debe ser menor a 10MB");
        return false;
      }

      setFileList([file]);
      return false; // Prevenir upload automático
    },
    onRemove: () => {
      setFileList([]);
    },
  };

  // Función para descargar plantilla CSV
  const downloadTemplate = () => {
    const csvContent = `Id,Nombre,Email,Cargo,Salario,Rol,Username,Password,ManagerId
123456,Juan Pérez,juan.perez@empresa.com,Analista,5000000,empleado,juan.perez,,1
123457,María García,maria.garcia@empresa.com,Desarrollador,6500000,empleado,maria.garcia,,2
123458,Carlos López,carlos.lopez@empresa.com,Gerente,8000000,manager,carlos.lopez,,
`;

    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", "plantilla_carga_usuarios.csv");
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  // Función para procesar el upload
  const handleUpload = async () => {
    if (fileList.length === 0) {
      message.warning("Por favor seleccione un archivo");
      return;
    }

    const formData = new FormData();
    formData.append("file", fileList[0]);

    setUploading(true);
    setUploadResults(null);

    try {
      const authHeaders = getAuthHeadersForFormData();
      const response = await fetch(
        `${API_CONFIG.BASE_URL}/api/employee/bulk-upload`,
        {
          method: "POST",
          headers: authHeaders,
          body: formData,
        }
      );

      let data;
      try {
        data = await response.json();
      } catch (jsonError) {
        console.error("Error parsing JSON response:", jsonError);
        throw new Error("Error al procesar la respuesta del servidor");
      }

      if (!response.ok) {
        throw new Error(data.error || "Error al procesar el archivo");
      }

      setUploadResults(data);
      
      if (data.summary.failed === 0) {
        message.success(
          `✅ Carga exitosa: ${data.summary.successful} usuarios agregados`
        );
      } else {
        message.warning(
          `⚠️ Carga parcial: ${data.summary.successful} exitosos, ${data.summary.failed} fallidos`
        );
      }

      setFileList([]);
    } catch (error) {
      message.error(error.message || "Error al cargar el archivo");
      console.error("Error:", error);
    } finally {
      setUploading(false);
    }
  };

  // Columnas para tabla de resultados exitosos
  const successColumns = [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
      width: 100,
    },
    {
      title: "Nombre",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
    },
    {
      title: "Rol",
      dataIndex: "role",
      key: "role",
      render: (role) => (
        <span className={`role-badge ${role}`}>
          {role === "manager" ? "Gestor" : role === "superusuario" ? "Admin" : "Empleado"}
        </span>
      ),
    },
  ];

  // Columnas para tabla de errores
  const errorColumns = [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
      width: 100,
    },
    {
      title: "Nombre",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Error",
      dataIndex: "error",
      key: "error",
      render: (text) => <span className="error-message">{text}</span>,
    },
  ];

  return (
    <div className="bulk-upload-container">
      <Card className="upload-card">
        <h2 className="section-title">📤 Carga Masiva de Usuarios</h2>
        <p className="section-description">
          Cargue múltiples usuarios simultáneamente usando un archivo CSV o Excel
        </p>

        <Divider />

        {/* Instrucciones */}
        <Alert
          message="Instrucciones de Uso"
          description={
            <ol className="instructions-list">
              <li>Descargue la plantilla CSV de ejemplo</li>
              <li>Complete los datos de los usuarios según el formato</li>
              <li>
                Campos requeridos: <strong>Id, Nombre, Email, Cargo</strong>
              </li>
              <li>
                Campos opcionales: <strong>Salario, Rol, Username, Password, ManagerId</strong>
              </li>
              <li>Cargue el archivo completado (CSV o Excel)</li>
            </ol>
          }
          type="info"
          showIcon
          style={{ marginBottom: 24 }}
        />

        {/* Botón para descargar plantilla */}
        <div className="template-section">
          <Button
            icon={<DownloadOutlined />}
            onClick={downloadTemplate}
            size="large"
            className="download-template-btn"
          >
            Descargar Plantilla CSV
          </Button>
        </div>

        <Divider />

        {/* Área de carga */}
        <Dragger {...uploadProps} className="upload-dragger">
          <p className="ant-upload-drag-icon">
            <InboxOutlined style={{ color: "#1890ff" }} />
          </p>
          <p className="ant-upload-text">
            Haga clic o arrastre el archivo a esta área
          </p>
          <p className="ant-upload-hint">
            Formatos soportados: CSV, Excel (.xlsx, .xls) - Máximo 10MB
          </p>
        </Dragger>

        {/* Botón de carga */}
        <div className="upload-actions">
          <Button
            type="primary"
            onClick={handleUpload}
            disabled={fileList.length === 0}
            loading={uploading}
            size="large"
            icon={<UploadOutlined />}
            className="upload-btn"
          >
            {uploading ? "Procesando..." : "Cargar Usuarios"}
          </Button>
        </div>
      </Card>

      {/* Resultados de la carga */}
      {uploadResults && (
        <Card className="results-card" style={{ marginTop: 24 }}>
          <h3 className="results-title">📊 Resultados de la Carga</h3>

          {/* Resumen */}
          <div className="summary-section">
            <Alert
              message="Resumen del Proceso"
              description={
                <div className="summary-stats">
                  <div className="stat-item">
                    <span className="stat-label">Total procesados:</span>
                    <span className="stat-value">{uploadResults.summary.total}</span>
                  </div>
                  <div className="stat-item success">
                    <span className="stat-label">✅ Exitosos:</span>
                    <span className="stat-value">{uploadResults.summary.successful}</span>
                  </div>
                  <div className="stat-item error">
                    <span className="stat-label">❌ Fallidos:</span>
                    <span className="stat-value">{uploadResults.summary.failed}</span>
                  </div>
                </div>
              }
              type={uploadResults.summary.failed === 0 ? "success" : "warning"}
              showIcon
              style={{ marginBottom: 24 }}
            />
          </div>

          {/* Tabla de usuarios exitosos */}
          {uploadResults.details.successful.length > 0 && (
            <div className="table-section">
              <h4 className="table-title">✅ Usuarios Agregados Exitosamente</h4>
              <Table
                dataSource={uploadResults.details.successful}
                columns={successColumns}
                rowKey="id"
                pagination={{ pageSize: 5 }}
                size="small"
              />
            </div>
          )}

          {/* Tabla de errores */}
          {uploadResults.details.failed.length > 0 && (
            <div className="table-section">
              <h4 className="table-title error">❌ Usuarios con Errores</h4>
              <Table
                dataSource={uploadResults.details.failed}
                columns={errorColumns}
                rowKey="id"
                pagination={{ pageSize: 5 }}
                size="small"
              />
            </div>
          )}
        </Card>
      )}
    </div>
  );
};

export default BulkUserUpload;