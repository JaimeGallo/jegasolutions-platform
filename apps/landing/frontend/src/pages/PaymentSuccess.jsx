import React, { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { 
  CheckCircle, 
  AlertCircle, 
  Loader2, 
  Mail, 
  Key, 
  Home,
  ExternalLink 
} from "lucide-react";
import { Link, useSearchParams } from "react-router-dom";

const PaymentSuccess = () => {
  const [searchParams] = useSearchParams();
  const [paymentData, setPaymentData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const transactionId = searchParams.get("id");
  const environment = searchParams.get("env");

  useEffect(() => {
    const fetchPaymentStatus = async () => {
      if (!transactionId) {
        setError("No se encontró el ID de transacción");
        setLoading(false);
        return;
      }

      try {
        // Extraer la referencia del transaction ID si es necesario
        // O usar directamente el endpoint del backend
        const apiUrl = import.meta.env.VITE_API_URL || "https://jegasolutions-platform.onrender.com";
        
        // Primero intentamos obtener el estado desde Wompi directamente
        const wompiResponse = await fetch(
          `https://${environment === "test" ? "sandbox" : "production"}.wompi.co/v1/transactions/${transactionId}`
        );

        if (wompiResponse.ok) {
          const wompiData = await wompiResponse.json();
          const reference = wompiData.data.reference;

          // Ahora consultamos nuestro backend con la referencia
          const backendResponse = await fetch(`${apiUrl}/api/payments/status/${reference}`);
          
          if (backendResponse.ok) {
            const backendData = await backendResponse.json();
            setPaymentData({
              ...backendData,
              wompiStatus: wompiData.data.status,
              wompiId: transactionId
            });
          } else {
            // Si no está en nuestro backend aún, usamos la data de Wompi
            setPaymentData({
              reference: reference,
              status: wompiData.data.status,
              amount: wompiData.data.amount_in_cents / 100,
              customerEmail: wompiData.data.customer_email,
              wompiStatus: wompiData.data.status,
              wompiId: transactionId
            });
          }
        } else {
          throw new Error("No se pudo obtener información del pago");
        }
      } catch (err) {
        console.error("Error fetching payment status:", err);
        setError("Error al obtener el estado del pago. Por favor, revisa tu correo electrónico.");
      } finally {
        setLoading(false);
      }
    };

    fetchPaymentStatus();
  }, [transactionId, environment]);

  // Estados de la UI
  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-50 flex items-center justify-center p-4">
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          className="bg-white rounded-2xl shadow-2xl p-8 max-w-md w-full text-center"
        >
          <Loader2 className="w-16 h-16 text-blue-500 mx-auto animate-spin mb-4" />
          <h2 className="text-xl font-semibold text-gray-800 mb-2">
            Verificando tu pago...
          </h2>
          <p className="text-gray-600">
            Estamos confirmando tu transacción con el banco
          </p>
        </motion.div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-red-50 to-orange-50 flex items-center justify-center p-4">
        <motion.div
          initial={{ opacity: 0, scale: 0.9 }}
          animate={{ opacity: 1, scale: 1 }}
          className="bg-white rounded-2xl shadow-2xl p-8 max-w-md w-full text-center"
        >
          <AlertCircle className="w-16 h-16 text-red-500 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            Información no disponible
          </h2>
          <p className="text-gray-600 mb-6">{error}</p>
          <Link
            to="/"
            className="inline-flex items-center gap-2 bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors"
          >
            <Home className="w-5 h-5" />
            Volver al inicio
          </Link>
        </motion.div>
      </div>
    );
  }

  // Renderizar según el estado del pago
  const isApproved = paymentData?.status === "APPROVED" || paymentData?.wompiStatus === "APPROVED";
  const isPending = paymentData?.status === "PENDING" || paymentData?.wompiStatus === "PENDING";
  const isDeclined = paymentData?.status === "DECLINED" || paymentData?.wompiStatus === "DECLINED";

  return (
    <div 
      className={`min-h-screen bg-gradient-to-br ${
        isApproved ? "from-green-50 to-blue-50" : 
        isPending ? "from-yellow-50 to-orange-50" : 
        "from-red-50 to-pink-50"
      } flex items-center justify-center p-4`}
    >
      <motion.div
        initial={{ opacity: 0, scale: 0.9 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.6 }}
        className="bg-white rounded-2xl shadow-2xl p-8 max-w-2xl w-full"
      >
        {/* Icono de estado */}
        <motion.div
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          transition={{ delay: 0.2, type: "spring", stiffness: 200 }}
          className="mb-6 text-center"
        >
          {isApproved ? (
            <CheckCircle className="w-20 h-20 text-green-500 mx-auto" />
          ) : isPending ? (
            <Loader2 className="w-20 h-20 text-yellow-500 mx-auto animate-spin" />
          ) : (
            <AlertCircle className="w-20 h-20 text-red-500 mx-auto" />
          )}
        </motion.div>

        {/* Título según estado */}
        <motion.h1
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.3 }}
          className="text-3xl font-bold text-gray-900 mb-4 text-center"
        >
          {isApproved ? "¡Pago Exitoso!" : 
           isPending ? "Pago en Proceso" : 
           "Pago Rechazado"}
        </motion.h1>

        {/* Mensaje según estado */}
        <motion.p
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.4 }}
          className="text-gray-600 mb-6 text-center"
        >
          {isApproved ? 
            "Tu pago ha sido procesado correctamente. Estamos configurando tu cuenta." :
           isPending ?
            "Tu pago está siendo procesado. Te notificaremos cuando se complete." :
            "Tu pago no pudo ser procesado. Por favor, intenta nuevamente."}
        </motion.p>

        {/* Información de la transacción */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.5 }}
          className="bg-gray-50 rounded-xl p-6 mb-6"
        >
          <h3 className="font-semibold text-gray-900 mb-4">
            Detalles de la Transacción
          </h3>
          <div className="space-y-3 text-sm">
            <div className="flex justify-between">
              <span className="text-gray-600">ID de Transacción:</span>
              <span className="font-medium text-gray-900 font-mono text-xs">
                {paymentData?.wompiId || transactionId}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Referencia:</span>
              <span className="font-medium text-gray-900 font-mono text-xs">
                {paymentData?.reference}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Monto:</span>
              <span className="font-medium text-gray-900">
                ${paymentData?.amount?.toLocaleString('es-CO')} COP
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Estado:</span>
              <span className={`font-medium ${
                isApproved ? "text-green-600" : 
                isPending ? "text-yellow-600" : 
                "text-red-600"
              }`}>
                {isApproved ? "Aprobado" : 
                 isPending ? "Pendiente" : 
                 "Rechazado"}
              </span>
            </div>
          </div>
        </motion.div>

        {/* Información adicional para pagos aprobados */}
        {isApproved && (
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.6 }}
            className="bg-blue-50 border border-blue-200 rounded-xl p-6 mb-6"
          >
            <h3 className="font-semibold text-blue-900 mb-3 flex items-center gap-2">
              <Mail className="w-5 h-5" />
              Próximos Pasos
            </h3>
            <ul className="space-y-2 text-sm text-blue-800">
              <li className="flex items-start gap-2">
                <CheckCircle className="w-4 h-4 mt-0.5 flex-shrink-0" />
                <span>
                  Recibirás un correo en <strong>{paymentData?.customerEmail}</strong> con tus credenciales de acceso
                </span>
              </li>
              <li className="flex items-start gap-2">
                <Key className="w-4 h-4 mt-0.5 flex-shrink-0" />
                <span>
                  Tu plataforma estará disponible en los próximos <strong>2-3 minutos</strong>
                </span>
              </li>
              <li className="flex items-start gap-2">
                <ExternalLink className="w-4 h-4 mt-0.5 flex-shrink-0" />
                <span>
                  Podrás acceder a través del subdominio asignado a tu empresa
                </span>
              </li>
            </ul>
          </motion.div>
        )}

        {/* Botones de acción */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.7 }}
          className="flex flex-col sm:flex-row gap-4 justify-center"
        >
          <Link
            to="/"
            className="inline-flex items-center justify-center gap-2 bg-gray-600 text-white px-6 py-3 rounded-lg hover:bg-gray-700 transition-colors"
          >
            <Home className="w-5 h-5" />
            Volver al inicio
          </Link>
          
          {isApproved && (
            <button
              onClick={() => window.open('mailto:JaimeGallo@jegasolutions.co', '_blank')}
              className="inline-flex items-center justify-center gap-2 bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors"
            >
              <Mail className="w-5 h-5" />
              ¿No recibiste el correo?
            </button>
          )}
        </motion.div>

        {/* Ambiente de prueba indicator */}
        {environment === "test" && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.8 }}
            className="mt-6 text-center text-xs text-gray-500 bg-yellow-50 py-2 px-4 rounded-lg"
          >
            🧪 Transacción realizada en ambiente de pruebas
          </motion.div>
        )}
      </motion.div>
    </div>
  );
};

export default PaymentSuccess;