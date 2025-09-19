import { useEffect, useState, useMemo, useCallback } from "react";
import {
  Table,
  Button,
  Modal,
  Typography,
  Space,
  Badge,
  Spin,
  Input,
  DatePicker,
  Row,
  Col,
  message,
} from "antd";
import {
  CheckCircleOutlined,
  CloseOutlined,
  ReloadOutlined,
  FilterOutlined,
  EditOutlined,
  DeleteOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons";
import {
  getCompensationRequestsByManager,
  updateCompensationRequestStatus,
  getAllCompensationRequests,
  deleteCompensationRequest,
  editCompensationRequest,
} from "../../../services/compensationRequestService";
import { useAuth } from "../../../utils/useAuth";
import dayjs from "dayjs";
import "./GestionSolicitudesCompensacion.scss";

const { Text } = Typography;
const { confirm } = Modal;
const { RangePicker } = DatePicker;

const STATUS = {
  APPROVED: "Approved",
  REJECTED: "Rejected",
  PENDING: "Pending",
};

export default function GestionSolicitudesCompensacion() {
  const { userRole } = useAuth();
  console.log("Hook useAuth - userRole:", userRole, typeof userRole);
  const [solicitudes, setSolicitudes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [decisionModal, setDecisionModal] = useState({
    open: false,
    record: null,
    action: null,
  });
  const [justification, setJustification] = useState("");
  const [dateRange, setDateRange] = useState(null);
  const [editModal, setEditModal] = useState({ open: false, record: null });
  const [editFields, setEditFields] = useState({
    workDate: null,
    requestedCompensationDate: null,
    justification: "",
  });
  const isSuperuser = userRole === "superusuario";

  const fetchSolicitudes = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      let startDateStr = null;
      let endDateStr = null;

      if (dateRange && dateRange[0] && dateRange[1]) {
        startDateStr = dateRange[0].format("YYYY-MM-DD");
        endDateStr = dateRange[1].format("YYYY-MM-DD");
      }

      let data;
      if (isSuperuser) {
        data = await getAllCompensationRequests(startDateStr, endDateStr);
      } else {
        data = await getCompensationRequestsByManager(startDateStr, endDateStr);
      }
      setSolicitudes(data);
    } catch (err) {
      console.error("Error completo:", err);
      setError(
        "Error al cargar las solicitudes: " +
          (err.message || "Error desconocido")
      );
    } finally {
      setLoading(false);
    }
  }, [dateRange, isSuperuser]);

  useEffect(() => {
    fetchSolicitudes();
  }, [fetchSolicitudes]);

  const handleDateRangeChange = (dates) => {
    setDateRange(dates);
  };

  const handleFilter = () => {
    fetchSolicitudes();
  };

  const handleRefresh = () => {
    fetchSolicitudes();
  };

  const handleDecision = useCallback((record, action) => {
    setDecisionModal({ open: true, record, action });
    setJustification("");
  }, []);

  const handleDecisionConfirm = async () => {
    const { record, action } = decisionModal;

    // Validar justificación para rechazo
    if (action === "reject" && !justification.trim()) {
      message.error("Debe ingresar un motivo de rechazo.");
      return;
    }

    setError("");
    setSuccess("");
    try {
      await updateCompensationRequestStatus(
        record.id,
        action === "approve" ? STATUS.APPROVED : STATUS.REJECTED,
        action === "reject" ? justification : null
      );

      message.success(
        action === "approve"
          ? "Solicitud aprobada correctamente."
          : "Solicitud rechazada correctamente."
      );

      // Pequeño delay antes de refrescar
      setTimeout(() => {
        fetchSolicitudes();
      }, 100);
    } catch (err) {
      setError(
        "Error al actualizar la solicitud: " +
          (err.message || "Error desconocido")
      );
      message.error("Error al actualizar la solicitud");
    } finally {
      setDecisionModal({ open: false, record: null, action: null });
      setJustification("");
    }
  };

  const handleEdit = useCallback((record) => {
    setEditFields({
      workDate: dayjs(record.workDate),
      requestedCompensationDate: dayjs(record.requestedCompensationDate),
      justification: record.justification || "",
    });
    setEditModal({ open: true, record });
  }, []);

  const handleEditFieldChange = (field, value) => {
    setEditFields((prev) => ({ ...prev, [field]: value }));
  };

  const handleEditConfirm = async () => {
    setError("");
    setSuccess("");
    const { record } = editModal;
    try {
      await editCompensationRequest(record.id, {
        employeeId: record.employeeId || record.EmployeeId,
        workDate: editFields.workDate.format("YYYY-MM-DD"),
        requestedCompensationDate:
          editFields.requestedCompensationDate.format("YYYY-MM-DD"),
        justification: editFields.justification,
      });

      message.success("Solicitud editada correctamente.");
      setEditModal({ open: false, record: null });
      fetchSolicitudes();
    } catch (err) {
      setError(
        "Error al editar la solicitud: " + (err.message || "Error desconocido")
      );
      message.error("Error al editar la solicitud");
    }
  };

  const handleDelete = useCallback(
    (record) => {
      confirm({
        title: "¿Eliminar solicitud?",
        icon: <ExclamationCircleOutlined />,
        content: `Se eliminará la solicitud de compensación de ${record.employeeName}. Esta acción no se puede deshacer.`,
        okText: "Eliminar",
        okType: "danger",
        cancelText: "Cancelar",
        onOk: async () => {
          setError("");
          setSuccess("");
          try {
            await deleteCompensationRequest(record.id);
            message.success("Solicitud eliminada correctamente.");
            fetchSolicitudes();
          } catch (err) {
            setError(
              "Error al eliminar la solicitud: " +
                (err.message || "Error desconocido")
            );
            message.error("Error al eliminar la solicitud");
          }
        },
      });
    },
    [fetchSolicitudes]
  );

  const columns = useMemo(
    () => [
      {
        title: "Empleado",
        dataIndex: "employeeName",
        key: "employeeName",
        align: "center",
        sorter: (a, b) =>
          (a.employeeName || "").localeCompare(b.employeeName || ""),
      },
      {
        title: "Fecha trabajada",
        dataIndex: "workDate",
        key: "workDate",
        align: "center",
        render: (date) => dayjs(date).format("YYYY-MM-DD"),
        sorter: (a, b) => dayjs(a.workDate).unix() - dayjs(b.workDate).unix(),
      },
      {
        title: "Día solicitado",
        dataIndex: "requestedCompensationDate",
        key: "requestedCompensationDate",
        align: "center",
        render: (date) => dayjs(date).format("YYYY-MM-DD"),
        sorter: (a, b) =>
          dayjs(a.requestedCompensationDate).unix() -
          dayjs(b.requestedCompensationDate).unix(),
      },
      {
        title: "Estado",
        dataIndex: "Status",
        key: "Status",
        align: "center",
        render: (status) => {
          let color = "processing";
          let text = "Pendiente";
          if (status === STATUS.APPROVED) {
            color = "success";
            text = "Aprobado";
          } else if (status === STATUS.REJECTED) {
            color = "error";
            text = "Rechazado";
          }
          return (
            <span>
              <Badge status={color} /> {text}
            </span>
          );
        },
        filters: [
          { text: "Pendiente", value: STATUS.PENDING },
          { text: "Aprobado", value: STATUS.APPROVED },
          { text: "Rechazado", value: STATUS.REJECTED },
        ],
        onFilter: (value, record) => record.Status === value,
      },
      {
        title: "Justificación",
        dataIndex: "Justification",
        key: "Justification",
        align: "center",
        render: (text) => (
          <div
            style={{
              maxWidth: "250px",
              wordWrap: "break-word",
              color: text ? "#444" : "#bbb",
              fontStyle: text ? "normal" : "italic",
            }}
          >
            {text && text.trim() ? text : "Sin justificación"}
          </div>
        ),
      },
      {
        title: "Aprobado por",
        dataIndex: "approvedByName",
        key: "approvedByName",
        align: "center",
        render: (text, record) =>
          record.Status === STATUS.APPROVED ? text || "-" : "-",
      },
      {
        title: "Fecha solicitud",
        dataIndex: "requestedAt",
        key: "requestedAt",
        align: "center",
        render: (date) => dayjs(date).format("YYYY-MM-DD HH:mm"),
        sorter: (a, b) =>
          dayjs(a.requestedAt).unix() - dayjs(b.requestedAt).unix(),
      },
      {
        title: "Acciones",
        key: "actions",
        align: "center",
        fixed: "right",
        width: 180,
        render: (_, record) => {
          console.log("=== DEBUG ACCIONES ===");
          console.log("Record completo:", record);
          console.log("userRole desde useAuth:", userRole);
          console.log("record.Status:", record.Status);
          console.log("STATUS.PENDING:", STATUS.PENDING);
          console.log("isPending:", record.Status === STATUS.PENDING);
          console.log("Record keys:", Object.keys(record));
          console.log("Record completo:", JSON.stringify(record, null, 2));

          const isPending = record.Status === STATUS.PENDING;
          const canManage =
            userRole === "manager" ||
            userRole === "superusuario" ||
            userRole === "Supervisor";

          return (
            <Space size="small">
              <Button
                type="primary"
                icon={<CheckCircleOutlined />}
                onClick={() => handleDecision(record, "approve")}
                className="approve-button"
                size="small"
                title="Aprobar"
                disabled={!(isPending && canManage)}
              />
              <Button
                danger
                icon={<CloseOutlined />}
                onClick={() => handleDecision(record, "reject")}
                className="reject-button"
                size="small"
                title="Rechazar"
                disabled={!(isPending && canManage)}
              />
              <Button
                icon={<EditOutlined />}
                onClick={() => handleEdit(record)}
                className="edit-button"
                size="small"
                title="Editar"
                disabled={!(isPending && canManage)}
              />
              <Button
                danger
                icon={<DeleteOutlined />}
                onClick={() => handleDelete(record)}
                className="delete-button"
                size="small"
                title="Eliminar"
                disabled={!(isPending && canManage)}
              />
            </Space>
          );
        },
      },
    ],
    [userRole, handleDecision, handleDelete, handleEdit]
  );

  return (
    <div className="gestion-solicitudes-container gestion-component">
      <div className="component-header">
        <h1 className="component-title">
          Gestión de Solicitudes de Compensación
        </h1>
        <Text type="secondary">
          {isSuperuser
            ? "Panel para aprobar o rechazar solicitudes de todos los empleados"
            : "Panel para aprobar o rechazar solicitudes de compensación de los empleados a su cargo"}
        </Text>
      </div>

      <div className="filter-section">
        <Row gutter={16} align="middle">
          <Col xs={24} sm={12} md={8} lg={6}>
            <Text strong>Filtrar por rango de fechas:</Text>
            <RangePicker
              style={{ width: "100%", marginTop: "8px" }}
              value={dateRange}
              onChange={handleDateRangeChange}
              placeholder={["Fecha inicio", "Fecha fin"]}
            />
          </Col>
          <Col xs={24} sm={12} md={4} lg={3} style={{ marginTop: "8px" }}>
            <Button
              type="primary"
              icon={<FilterOutlined />}
              onClick={handleFilter}
              style={{ width: "100%" }}
              id="gestion-compensacion-filter-button"
              className="filter-button"
            >
              Filtrar
            </Button>
          </Col>
        </Row>
      </div>

      <div className="actions-bar">
        <Button
          type="primary"
          icon={<ReloadOutlined />}
          onClick={handleRefresh}
          className="refresh-button"
          id="gestion-compensacion-refresh-button"
        >
          Actualizar Datos
        </Button>
        <Badge
          count={solicitudes.filter((s) => s.Status === STATUS.PENDING).length}
          offset={[0, 0]}
        >
          <Text>Pendientes de Aprobación</Text>
        </Badge>
      </div>

      {error && <div className="error-message">{error}</div>}
      {success && <div className="success-msg">{success}</div>}

      {loading ? (
        <div className="loading-container">
          <Spin size="large" />
          <p>Cargando solicitudes...</p>
        </div>
      ) : (
        <>
          {solicitudes.length > 0 ? (
            <Table
              columns={columns}
              dataSource={solicitudes}
              rowKey="id"
              pagination={{
                pageSize: 10,
                showSizeChanger: true,
                pageSizeOptions: ["10", "20", "50", "100"],
                showTotal: (total, range) =>
                  `${range[0]}-${range[1]} de ${total} solicitudes`,
              }}
              scroll={{ x: 1500, y: 500 }}
              rowClassName={(record) =>
                record.Status === STATUS.APPROVED
                  ? "table-row-approved"
                  : record.Status === STATUS.REJECTED
                  ? "table-row-warning"
                  : "table-row-normal"
              }
            />
          ) : (
            <div className="empty-data">
              <Text>
                {isSuperuser
                  ? "No hay solicitudes de compensación en el sistema."
                  : "No hay solicitudes de compensación para los empleados a su cargo."}
              </Text>
            </div>
          )}
        </>
      )}

      {/* Modal de Decisión (Aprobar/Rechazar) */}
      <Modal
        title={
          <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
            {decisionModal.action === "approve" ? (
              <CheckCircleOutlined style={{ color: "#52c41a" }} />
            ) : (
              <CloseOutlined style={{ color: "#ff4d4f" }} />
            )}
            <span>
              {decisionModal.action === "approve"
                ? "Aprobar Solicitud"
                : "Rechazar Solicitud"}
            </span>
          </div>
        }
        open={decisionModal.open}
        onCancel={() =>
          setDecisionModal({ open: false, record: null, action: null })
        }
        onOk={handleDecisionConfirm}
        okText={decisionModal.action === "approve" ? "Aprobar" : "Rechazar"}
        okButtonProps={{
          type: decisionModal.action === "approve" ? "primary" : "default",
          danger: decisionModal.action === "reject",
          disabled: decisionModal.action === "reject" && !justification.trim(),
        }}
        cancelText="Cancelar"
        destroyOnClose
        centered
      >
        <div style={{ marginBottom: "16px" }}>
          <Text strong>Empleado: </Text>
          <Text>{decisionModal.record?.employeeName}</Text>
        </div>
        <div style={{ marginBottom: "16px" }}>
          <Text strong>Fecha trabajada: </Text>
          <Text>
            {decisionModal.record
              ? dayjs(decisionModal.record.workDate).format("YYYY-MM-DD")
              : ""}
          </Text>
        </div>
        <div style={{ marginBottom: "16px" }}>
          <Text strong>Día solicitado: </Text>
          <Text>
            {decisionModal.record
              ? dayjs(decisionModal.record.requestedCompensationDate).format(
                  "YYYY-MM-DD"
                )
              : ""}
          </Text>
        </div>

        {decisionModal.action === "reject" && (
          <>
            <Text strong>Motivo de rechazo *</Text>
            <Input.TextArea
              rows={3}
              value={justification}
              onChange={(e) => setJustification(e.target.value)}
              placeholder="Ingrese el motivo de rechazo (requerido)"
              style={{ marginTop: "8px" }}
              showCount
              maxLength={500}
            />
            {!justification.trim() && (
              <Text type="danger" style={{ fontSize: "12px" }}>
                El motivo de rechazo es requerido.
              </Text>
            )}
          </>
        )}

        {decisionModal.action === "approve" && (
          <Text>¿Está seguro de que desea aprobar esta solicitud?</Text>
        )}
      </Modal>

      {/* Modal de Edición */}
      <Modal
        title={
          <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
            <EditOutlined />
            <span>Editar Solicitud de Compensación</span>
          </div>
        }
        open={editModal.open}
        onCancel={() => setEditModal({ open: false, record: null })}
        onOk={handleEditConfirm}
        okText="Guardar"
        cancelText="Cancelar"
        destroyOnClose
        centered
        width={600}
      >
        <div style={{ marginBottom: "16px" }}>
          <Text strong>Empleado: </Text>
          <Text>{editModal.record?.employeeName}</Text>
        </div>

        <div style={{ marginBottom: 16 }}>
          <Text strong>Fecha trabajada *</Text>
          <DatePicker
            value={editFields.workDate}
            onChange={(date) => handleEditFieldChange("workDate", date)}
            style={{ width: "100%", marginTop: "8px" }}
            format="YYYY-MM-DD"
            placeholder="Seleccionar fecha de trabajo"
          />
        </div>

        <div style={{ marginBottom: 16 }}>
          <Text strong>Día solicitado *</Text>
          <DatePicker
            value={editFields.requestedCompensationDate}
            onChange={(date) =>
              handleEditFieldChange("requestedCompensationDate", date)
            }
            style={{ width: "100%", marginTop: "8px" }}
            format="YYYY-MM-DD"
            placeholder="Seleccionar día de compensación"
          />
        </div>

        <div style={{ marginBottom: 16 }}>
          <Text strong>Justificación</Text>
          <Input.TextArea
            rows={3}
            value={editFields.justification}
            onChange={(e) =>
              handleEditFieldChange("justification", e.target.value)
            }
            placeholder="Justificación (opcional)"
            style={{ marginTop: "8px" }}
            showCount
            maxLength={500}
          />
        </div>
      </Modal>
    </div>
  );
}
