import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Form,
  Input,
  Button,
  message,
  Typography,
  Divider,
  Alert,
  Spin,
} from 'antd';
import { useAuth } from '../utils/useAuth';
import { UserService } from '../services/UserService';
import { jwtDecode } from 'jwt-decode';
import {
  LockOutlined,
  MailOutlined,
  EyeInvisibleOutlined,
  EyeTwoTone,
  HomeOutlined,
  ArrowLeftOutlined,
} from '@ant-design/icons';
import './LoginPage.scss';

const { Title, Text } = Typography;

const Login = () => {
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { login } = useAuth();
  const [errorMsg, setErrorMsg] = useState('');
  const [isFromSSO, setIsFromSSO] = useState(false);
  const [isCheckingSSO, setIsCheckingSSO] = useState(true);

  // ✅ SSO: Detectar si el usuario viene del tenant-dashboard
  useEffect(() => {
    const checkSSOStatus = () => {
      const urlParams = new URLSearchParams(window.location.search);
      const hasToken = urlParams.get('token');
      const referrer = document.referrer;

      // Detectar si viene del SSO
      const fromSSO =
        hasToken ||
        referrer.includes('jegasolutions.co') ||
        referrer.includes('tenant-dashboard');

      setIsFromSSO(fromSSO);
      setIsCheckingSSO(false);

      if (fromSSO) {
        console.log('🔍 SSO: Usuario detectado desde tenant-dashboard');
      }
    };

    // Pequeño delay para permitir que el AuthProvider procese el token
    const timer = setTimeout(checkSSOStatus, 1000);
    return () => clearTimeout(timer);
  }, []);

  const handleLogin = async values => {
    setLoading(true);
    setErrorMsg('');
    try {
      const data = await UserService.login(values.email, values.password);
      const { token } = data;
      const decodedToken = jwtDecode(token);

      if (decodedToken.role) {
        login({ token, role: decodedToken.role });
        navigate('/menu');
        message.success(`Bienvenid@ ${decodedToken.unique_name}`);
      } else {
        setErrorMsg('No se pudo determinar el rol del usuario');
      }
    } catch (error) {
      setErrorMsg('Usuario o contraseña incorrectos');
    } finally {
      setLoading(false);
    }
  };

  const handleGoToDashboard = () => {
    // Detectar el subdomain del tenant
    const hostname = window.location.hostname;
    if (hostname.includes('jegasolutions.co')) {
      const parts = hostname.split('.');
      if (parts.length >= 3 && parts[0] !== 'www') {
        const subdomain = parts[0];
        window.location.href = `https://${subdomain}.jegasolutions.co`;
      } else {
        window.location.href = 'https://jegasolutions.co';
      }
    } else {
      window.location.href = 'https://jegasolutions.co';
    }
  };

  // Mostrar loading mientras se verifica el SSO
  if (isCheckingSSO) {
    return (
      <div className="login-container">
        <div className="login-card">
          <div className="login-header">
            <Spin size="large" />
            <Title
              level={2}
              className="welcome-title"
              style={{ marginTop: 16 }}
            >
              Verificando acceso...
            </Title>
            <Text type="secondary" className="welcome-subtitle">
              Procesando tu sesión desde el dashboard principal
            </Text>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          {/* <img src={Logo} alt="Logo Amadeus" className="logo" /> */}
          <Title level={2} className="welcome-title">
            {isFromSSO ? 'Acceso al Módulo' : 'Bienvenid@'}
          </Title>
          <Text type="secondary" className="welcome-subtitle">
            {isFromSSO
              ? 'Accede directamente al módulo de Gestión de Horas Extra'
              : 'Accede a tu cuenta corporativa para gestionar tus horas extra'}
          </Text>
        </div>

        {/* Alert para usuarios que vienen del SSO */}
        {isFromSSO && (
          <Alert
            message="Acceso desde Dashboard Principal"
            description="Si tienes problemas para acceder automáticamente, puedes iniciar sesión manualmente aquí."
            type="info"
            showIcon
            style={{ marginBottom: 16 }}
            action={
              <Button
                size="small"
                onClick={handleGoToDashboard}
                icon={<HomeOutlined />}
              >
                Volver al Dashboard
              </Button>
            }
          />
        )}

        <Divider className="divider" />

        <Form
          form={form}
          name="login-form"
          onFinish={handleLogin}
          layout="vertical"
          className="login-form"
          initialValues={{ remember: true }}
        >
          <Form.Item
            name="email"
            rules={[
              {
                required: true,
                message: 'Por favor ingrese su correo electrónico',
              },
              {
                type: 'email',
                message: 'Ingrese un correo electrónico válido',
              },
            ]}
          >
            <Input
              prefix={<MailOutlined />}
              placeholder="correo@empresa.com"
              size="large"
              className="form-input"
              autoComplete="username"
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[
              {
                required: true,
                message: 'Por favor ingrese su contraseña',
              },
            ]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="Contraseña"
              size="large"
              className="form-input"
              iconRender={visible =>
                visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
              }
              autoComplete="current-password"
            />
          </Form.Item>

          {errorMsg && (
            <div className="login-error-message">
              <Text type="danger">{errorMsg}</Text>
            </div>
          )}

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={loading}
              block
              size="large"
              className="login-button"
            >
              Iniciar Sesión
            </Button>
          </Form.Item>

          <div className="login-footer">
            <Text className="footer-text">
              ¿Problemas para ingresar?{' '}
              <a href="mailto:soporte@jegasolutions.co">Contacta al soporte</a>
            </Text>
            {isFromSSO && (
              <div style={{ marginTop: 8 }}>
                <Button
                  type="link"
                  onClick={handleGoToDashboard}
                  icon={<ArrowLeftOutlined />}
                  style={{ padding: 0 }}
                >
                  Volver al Dashboard Principal
                </Button>
              </div>
            )}
          </div>
        </Form>
      </div>
    </div>
  );
};

export default Login;
