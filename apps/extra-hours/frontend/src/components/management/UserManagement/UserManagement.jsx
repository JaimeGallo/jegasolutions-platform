import { useState, useEffect } from 'react';
import {
  Tabs,
  Form,
  Input,
  Button,
  Table,
  Modal,
  message,
  Select,
  InputNumber,
  Space,
  Tooltip,
  Popconfirm,
  Alert,
  Tag,
  Statistic,
  Row,
  Col,
  Card,
} from 'antd';
import {
  EditOutlined,
  DeleteOutlined,
  SearchOutlined,
  PlusOutlined,
  UserOutlined,
  TeamOutlined,
} from '@ant-design/icons';
import {
  addEmployee,
  findEmployee,
  updateEmployee,
  deleteEmployee,
} from '../../../services/employeeService';
import { UserService } from '../../../services/UserService';
import './UserManagement.scss';

const { TabPane } = Tabs;
const { Search } = Input;

const UserManagement = () => {
  const [activeTab, setActiveTab] = useState('1');
  const [managerForm] = Form.useForm();
  const [employeeForm] = Form.useForm();
  const [editForm] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [users, setUsers] = useState([]);
  const [managers, setManagers] = useState([]);
  const [selectedUser, setSelectedUser] = useState(null);
  const [isEditModalOpen, setEditModalOpen] = useState(false);
  const [searchValue, setSearchValue] = useState('');
  const [stats, setStats] = useState({
    managersCount: 0,
    employeesCount: 0,
    orphanEmployees: 0,
  });

  // Cargar managers al montar el componente
  useEffect(() => {
    loadManagers();
  }, []);

  // Cargar estadísticas
  useEffect(() => {
    loadStats();
  }, [users]);

  const loadManagers = async () => {
    try {
      // TODO: Crear endpoint GET /api/managers en el backend
      // Por ahora, simulamos con búsqueda de todos los usuarios rol manager
      const allUsers = []; // Obtener todos los usuarios
      const managersData = allUsers.filter(u => u.role === 'manager');
      setManagers(managersData);
    } catch (error) {
      console.error('Error cargando managers:', error);
    }
  };

  const loadStats = () => {
    const managersCount = users.filter(u => u.role === 'manager').length;
    const employeesCount = users.filter(u => u.role === 'empleado').length;
    const orphanEmployees = users.filter(
      u => u.role === 'empleado' && !u.manager_id
    ).length;
    setStats({ managersCount, employeesCount, orphanEmployees });
  };

  const generateEmail = name => {
    return name ? `${name.toLowerCase().replace(/ /g, '.')}@empresa.com` : '';
  };

  // ========================================
  // TAB 1: CREAR MANAGER
  // ========================================
  const handleAddManager = async values => {
    setLoading(true);
    try {
      await addEmployee({
        ...values,
        role: 'manager', // Rol fijo
        email: generateEmail(values.name),
        manager_id: null, // Los managers no tienen manager
      });

      message.success('Manager creado exitosamente');
      managerForm.resetFields();
      loadManagers(); // Recargar lista de managers
    } catch (error) {
      message.error(`Error: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };

  // ========================================
  // TAB 2: CREAR EMPLEADO
  // ========================================
  const handleAddEmployee = async values => {
    setLoading(true);
    try {
      await addEmployee({
        ...values,
        email: generateEmail(values.name),
      });

      message.success('Empleado agregado exitosamente');
      employeeForm.resetFields();
    } catch (error) {
      message.error(`Error: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };

  // ========================================
  // TAB 3: GESTIONAR USUARIOS
  // ========================================
  const handleSearch = async value => {
    setSearchValue(value);
    if (!value) {
      setUsers([]);
      return;
    }
    setLoading(true);
    try {
      const user = await findEmployee(value);
      setUsers(user ? [user] : []);
      if (!user) {
        message.info('No se encontraron resultados');
      }
    } catch (error) {
      message.error('Error al buscar usuario');
      setUsers([]);
    } finally {
      setLoading(false);
    }
  };

  const showEditModal = user => {
    setSelectedUser(user);
    editForm.setFieldsValue({
      name: user.name,
      position: user.position,
      salary: user.salary,
      manager_id: user.manager?.id,
      role: user.role,
    });
    setEditModalOpen(true);
  };

  const handleEdit = async values => {
    setLoading(true);
    try {
      await updateEmployee(selectedUser.id, values);

      if (values.newPassword) {
        await UserService.changePasswordAdmin(
          selectedUser.id,
          values.newPassword
        );
      }

      message.success('Usuario actualizado correctamente');
      setEditModalOpen(false);
      handleSearch(searchValue); // Refresh data
    } catch (error) {
      message.error('Error al actualizar el usuario');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async userId => {
    setLoading(true);
    try {
      await deleteEmployee(userId);
      message.success('Usuario eliminado correctamente');
      setUsers(prevUsers => prevUsers.filter(u => u.id !== userId));
    } catch (error) {
      message.error('Error al eliminar el usuario');
    } finally {
      setLoading(false);
    }
  };

  const columns = [
    {
      title: 'ID',
      dataIndex: 'id',
      key: 'id',
      sorter: (a, b) => a.id - b.id,
    },
    {
      title: 'Nombre',
      dataIndex: 'name',
      key: 'name',
      sorter: (a, b) => a.name.localeCompare(b.name),
    },
    {
      title: 'Cargo',
      dataIndex: 'position',
      key: 'position',
    },
    {
      title: 'Rol',
      dataIndex: 'role',
      key: 'role',
      render: role => {
        const colors = {
          superusuario: 'red',
          manager: 'blue',
          empleado: 'green',
        };
        return <Tag color={colors[role] || 'default'}>{role}</Tag>;
      },
    },
    {
      title: 'Manager',
      dataIndex: ['manager', 'name'],
      key: 'manager_name',
      render: name =>
        name ? <Tag color="blue">{name}</Tag> : <Tag>Sin manager</Tag>,
    },
    {
      title: 'Acciones',
      key: 'actions',
      render: (_, user) => (
        <Space>
          <Tooltip title="Editar">
            <Button
              icon={<EditOutlined />}
              onClick={() => showEditModal(user)}
              type="primary"
              size="small"
            />
          </Tooltip>
          <Tooltip title="Eliminar">
            <Popconfirm
              title="¿Estás seguro de eliminar este usuario?"
              description="Esta acción no se puede deshacer."
              onConfirm={() => handleDelete(user.id)}
              okText="Eliminar"
              cancelText="Cancelar"
              okButtonProps={{ danger: true }}
            >
              <Button icon={<DeleteOutlined />} danger size="small" />
            </Popconfirm>
          </Tooltip>
        </Space>
      ),
    },
  ];

  return (
    <div className="user-management">
      {/* Dashboard de Estadísticas */}
      <Row gutter={16} style={{ marginBottom: 24 }}>
        <Col span={8}>
          <Card>
            <Statistic
              title="Managers en el Sistema"
              value={stats.managersCount}
              prefix={<UserOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>
        <Col span={8}>
          <Card>
            <Statistic
              title="Empleados Totales"
              value={stats.employeesCount}
              prefix={<TeamOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
        <Col span={8}>
          <Card>
            <Statistic
              title="Empleados Sin Manager"
              value={stats.orphanEmployees}
              valueStyle={{
                color: stats.orphanEmployees > 0 ? '#cf1322' : '#3f8600',
              }}
            />
          </Card>
        </Col>
      </Row>

      <Tabs activeKey={activeTab} onChange={setActiveTab}>
        {/* ========================================
            TAB 1: CREAR MANAGER
            ======================================== */}
        <TabPane tab="📋 Crear Manager" key="1">
          <Card title="Nuevo Manager / Gerente">
            <Alert
              message="Los managers supervisan empleados y aprueban horas extra"
              type="info"
              showIcon
              style={{ marginBottom: 16 }}
            />
            <Form
              form={managerForm}
              layout="vertical"
              onFinish={handleAddManager}
            >
              <Row gutter={16}>
                <Col span={12}>
                  <Form.Item
                    label="ID"
                    name="id"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor ingrese el ID',
                      },
                    ]}
                  >
                    <InputNumber min={1} style={{ width: '100%' }} />
                  </Form.Item>

                  <Form.Item
                    label="Nombre Completo"
                    name="name"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor ingrese el nombre',
                      },
                    ]}
                  >
                    <Input placeholder="Ej: Juan Pérez" />
                  </Form.Item>
                </Col>

                <Col span={12}>
                  <Form.Item
                    label="Cargo/Departamento"
                    name="position"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor ingrese el cargo',
                      },
                    ]}
                  >
                    <Input placeholder="Ej: Gerente de Ventas" />
                  </Form.Item>

                  <Form.Item
                    label="Salario"
                    name="salary"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor ingrese el salario',
                      },
                    ]}
                  >
                    <InputNumber
                      formatter={value =>
                        `$ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
                      }
                      parser={value => value.replace(/\$\s?|(,*)/g, '')}
                      style={{ width: '100%' }}
                      min={1}
                      placeholder="Ej: 5000000"
                    />
                  </Form.Item>
                </Col>
              </Row>

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  loading={loading}
                  icon={<PlusOutlined />}
                  size="large"
                >
                  Crear Manager
                </Button>
              </Form.Item>
            </Form>
          </Card>
        </TabPane>

        {/* ========================================
            TAB 2: CREAR EMPLEADO
            ======================================== */}
        <TabPane tab="👤 Crear Empleado" key="2">
          <Card title="Nuevo Empleado">
            {managers.length === 0 && (
              <Alert
                type="warning"
                message="No hay managers disponibles"
                description="Debes crear al menos un manager primero en la pestaña 'Crear Manager'"
                action={
                  <Button onClick={() => setActiveTab('1')} type="primary">
                    Ir a Crear Manager
                  </Button>
                }
                style={{ marginBottom: 16 }}
                showIcon
              />
            )}

            <Form
              form={employeeForm}
              layout="vertical"
              onFinish={handleAddEmployee}
            >
              <Row gutter={16}>
                <Col span={12}>
                  <Form.Item
                    label="ID"
                    name="id"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor ingrese el ID',
                      },
                    ]}
                  >
                    <InputNumber min={1} style={{ width: '100%' }} />
                  </Form.Item>

                  <Form.Item
                    label="Nombre Completo"
                    name="name"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor ingrese el nombre',
                      },
                    ]}
                  >
                    <Input placeholder="Ej: María González" />
                  </Form.Item>

                  <Form.Item
                    label="Cargo/Posición"
                    name="position"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor ingrese el cargo',
                      },
                    ]}
                  >
                    <Input placeholder="Ej: Asistente de Ventas" />
                  </Form.Item>
                </Col>

                <Col span={12}>
                  <Form.Item
                    label="Salario"
                    name="salary"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor ingrese el salario',
                      },
                    ]}
                  >
                    <InputNumber
                      formatter={value =>
                        `$ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
                      }
                      parser={value => value.replace(/\$\s?|(,*)/g, '')}
                      style={{ width: '100%' }}
                      min={1}
                      placeholder="Ej: 2500000"
                    />
                  </Form.Item>

                  <Form.Item
                    label="Manager Asignado"
                    name="manager_id"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor seleccione el manager',
                      },
                    ]}
                  >
                    <Select
                      placeholder="Selecciona el manager"
                      showSearch
                      filterOption={(input, option) =>
                        option.children
                          .toLowerCase()
                          .includes(input.toLowerCase())
                      }
                      disabled={managers.length === 0}
                    >
                      {managers.map(m => (
                        <Select.Option key={m.id} value={m.id}>
                          {m.name} - {m.position} (ID: {m.id})
                        </Select.Option>
                      ))}
                    </Select>
                  </Form.Item>

                  <Form.Item
                    label="Rol"
                    name="role"
                    initialValue="empleado"
                    rules={[
                      {
                        required: true,
                        message: 'Por favor seleccione el rol',
                      },
                    ]}
                  >
                    <Select>
                      <Select.Option value="empleado">Empleado</Select.Option>
                      <Select.Option value="superusuario">
                        Superusuario
                      </Select.Option>
                    </Select>
                  </Form.Item>
                </Col>
              </Row>

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  loading={loading}
                  icon={<PlusOutlined />}
                  size="large"
                  disabled={managers.length === 0}
                >
                  Crear Empleado
                </Button>
              </Form.Item>
            </Form>
          </Card>
        </TabPane>

        {/* ========================================
            TAB 3: GESTIONAR USUARIOS
            ======================================== */}
        <TabPane tab="📊 Gestionar Usuarios" key="3">
          <div className="manage-users">
            <div className="search-container">
              <Search
                placeholder="Buscar por ID de usuario"
                onSearch={handleSearch}
                enterButton={<SearchOutlined />}
                loading={loading}
                allowClear
                size="large"
              />
            </div>

            <div className="user-table">
              <Table
                columns={columns}
                dataSource={users}
                rowKey="id"
                loading={loading}
                pagination={{ pageSize: 10 }}
                locale={{
                  emptyText: searchValue
                    ? 'No se encontraron resultados'
                    : 'Ingrese un ID para buscar',
                }}
              />
            </div>
          </div>
        </TabPane>
      </Tabs>

      {/* Edit Modal */}
      <Modal
        title="Editar Usuario"
        open={isEditModalOpen}
        onCancel={() => setEditModalOpen(false)}
        footer={null}
        width={600}
      >
        <Form form={editForm} onFinish={handleEdit} layout="vertical">
          <Form.Item
            name="name"
            label="Nombre"
            rules={[{ required: true, message: 'Este campo es requerido' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="position"
            label="Posición"
            rules={[{ required: true, message: 'Este campo es requerido' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="salary"
            label="Salario"
            rules={[{ required: true, message: 'Este campo es requerido' }]}
          >
            <InputNumber
              formatter={value =>
                `$ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
              }
              parser={value => value.replace(/\$\s?|(,*)/g, '')}
              style={{ width: '100%' }}
              min={1}
            />
          </Form.Item>

          {selectedUser?.role !== 'manager' && (
            <Form.Item name="manager_id" label="Manager">
              <Select
                placeholder="Selecciona el manager"
                showSearch
                filterOption={(input, option) =>
                  option.children.toLowerCase().includes(input.toLowerCase())
                }
              >
                {managers.map(m => (
                  <Select.Option key={m.id} value={m.id}>
                    {m.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          )}

          <Form.Item
            name="role"
            label="Rol"
            rules={[{ required: true, message: 'Este campo es requerido' }]}
          >
            <Select>
              <Select.Option value="manager">Manager</Select.Option>
              <Select.Option value="empleado">Empleado</Select.Option>
              <Select.Option value="superusuario">Superusuario</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item name="newPassword" label="Nueva Contraseña (opcional)">
            <Input.Password placeholder="Dejar en blanco para no cambiar" />
          </Form.Item>

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit" loading={loading}>
                Guardar Cambios
              </Button>
              <Button onClick={() => setEditModalOpen(false)}>Cancelar</Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default UserManagement;
