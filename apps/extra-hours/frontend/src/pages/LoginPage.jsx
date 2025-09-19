import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Form, Input, Button, message, Typography, Divider } from "antd";
import { useAuth } from "../utils/useAuth";
import { UserService } from "../services/UserService";
import { jwtDecode } from "jwt-decode";
import {
  LockOutlined,
  MailOutlined,
  EyeInvisibleOutlined,
  EyeTwoTone,
} from "@ant-design/icons";
import "./LoginPage.scss";

const { Title, Text } = Typography;

const Login = () => {
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { login } = useAuth();
  const [errorMsg, setErrorMsg] = useState("");

  const handleLogin = async (values) => {
    setLoading(true);
    setErrorMsg("");
    try {
      const data = await UserService.login(values.email, values.password);
      const { token } = data;
      const decodedToken = jwtDecode(token);

      if (decodedToken.role) {
        login({ token, role: decodedToken.role });
        navigate("/menu");
        message.success(`Bienvenid@ ${decodedToken.unique_name}`);
      } else {
        setErrorMsg("No se pudo determinar el rol del usuario");
      }
    } catch (error) {
      setErrorMsg("Usuario o contraseña incorrectos");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          {/* <img src={Logo} alt="Logo Amadeus" className="logo" /> */}
          <Title level={2} className="welcome-title">
            Bienvenid@
          </Title>
          <Text type="secondary" className="welcome-subtitle">
            Accede a tu cuenta corporativa para gestionar tus horas extra
          </Text>
        </div>

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
                message: "Por favor ingrese su correo electrónico",
              },
              {
                type: "email",
                message: "Ingrese un correo electrónico válido",
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
                message: "Por favor ingrese su contraseña",
              },
            ]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="Contraseña"
              size="large"
              className="form-input"
              iconRender={(visible) =>
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
              ¿Problemas para ingresar?{" "}
              <a href="mailto:soporte@empresa.com">Contacta al soporte</a>
            </Text>
          </div>
        </Form>
      </div>
    </div>
  );
};

export default Login;
