import React, { useState } from "react";
import { motion } from "framer-motion";
import { CreditCard, Loader2 } from "lucide-react";
import { useWompi } from "../hooks/useWompi";
import Modal from "./Modal";

const PaymentButton = ({
  amount,
  modules,
  deploymentType,
  employeeCount,
  onPaymentInitiated,
}) => {
  const { createPayment, isLoading, error } = useWompi();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [customerData, setCustomerData] = useState({
    email: "",
    fullName: "",
    phone: "",
  });
  const [validationErrors, setValidationErrors] = useState({});

  const generateReference = () => {
    const timestamp = Date.now();
    const modulesStr = Object.keys(modules)
      .filter((key) => modules[key])
      .join("-");
    return `JEGA-${modulesStr}-${deploymentType}-${timestamp}`.toUpperCase();
  };

  const validateForm = () => {
    const errors = {};
    if (!customerData.email.trim()) errors.email = "El correo es obligatorio.";
    else if (!/\S+@\S+\.\S+/.test(customerData.email))
      errors.email = "El formato del correo no es válido.";
    if (!customerData.fullName.trim())
      errors.fullName = "El nombre es obligatorio.";
    return errors;
  };

  const handlePayment = async () => {
    const errors = validateForm();
    setValidationErrors(errors);

    if (Object.keys(errors).length > 0) {
      return;
    }

    try {
      // ==========================================
      // 🧪 MODO PRUEBA - MONTO FIJO DE $10 COP
      // ==========================================
      // Se ignora el monto calculado y se usa $10 COP para testing
      const testAmount = 10;

      /* PRODUCCIÓN (comentado):
       * const productionAmount = amount;
       * Descomentar cuando se termine el periodo de prueba
       */

      const paymentData = {
        amount: testAmount, // 🧪 Monto de prueba
        reference: generateReference(),
        redirectUrl: `${window.location.origin}/payment-success`,
        customerData: {
          email: customerData.email,
          fullName: customerData.fullName,
          phoneNumber: customerData.phone,
        },
        customerEmail: customerData.email,
        customerFullName: customerData.fullName,
        phoneNumber: customerData.phone,
        taxInCents: Math.round(testAmount * 0.19 * 100), // IVA 19% sobre monto de prueba
      };

      console.log("🧪 PAGO DE PRUEBA - Monto: $10 COP");
      console.log("Payment data:", paymentData);

      onPaymentInitiated?.(paymentData);
      await createPayment(paymentData);
      setIsModalOpen(false);
    } catch (err) {
      console.error("Error initiating payment:", err);
      alert("Error al procesar el pago. Por favor intenta nuevamente.");
    }
  };

  return (
    <div className="space-y-4">
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
          {error}
        </div>
      )}

      {/* Modal de Datos del Cliente */}
      <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)}>
        <div className="space-y-4">
          <h3 className="text-xl font-bold text-gray-900">
            Información del Cliente
          </h3>

          {/* Banner de Modo Prueba en Modal */}
          <div className="bg-yellow-50 border-l-4 border-yellow-400 p-3 rounded-r">
            <p className="text-sm text-yellow-700">
              <strong>🧪 Modo Prueba:</strong> Monto fijo de $10 COP para testing
            </p>
          </div>

          <div className="space-y-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Nombre Completo *
              </label>
              <input
                type="text"
                value={customerData.fullName}
                onChange={(e) =>
                  setCustomerData({ ...customerData, fullName: e.target.value })
                }
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${
                  validationErrors.fullName
                    ? "border-red-500"
                    : "border-gray-300"
                }`}
                placeholder="Juan Pérez"
              />
              {validationErrors.fullName && (
                <p className="text-red-500 text-xs mt-1">
                  {validationErrors.fullName}
                </p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Email *
              </label>
              <input
                type="email"
                value={customerData.email}
                onChange={(e) =>
                  setCustomerData({ ...customerData, email: e.target.value })
                }
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${
                  validationErrors.email ? "border-red-500" : "border-gray-300"
                }`}
                placeholder="juan@empresa.com"
              />
              {validationErrors.email && (
                <p className="text-red-500 text-xs mt-1">
                  {validationErrors.email}
                </p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Teléfono (Opcional)
              </label>
              <input
                type="tel"
                value={customerData.phone}
                onChange={(e) =>
                  setCustomerData({ ...customerData, phone: e.target.value })
                }
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                placeholder="+57 300 123 4567"
              />
            </div>
          </div>

          <button
            onClick={handlePayment}
            disabled={isLoading}
            className="w-full bg-gradient-to-r from-jega-blue-600 to-jega-blue-900 text-white py-3 px-6 rounded-lg font-semibold hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            {isLoading ? (
              <Loader2 className="w-5 h-5 animate-spin" />
            ) : (
              <CreditCard className="w-5 h-5" />
            )}
            <span>{isLoading ? "Procesando..." : "Pagar con Wompi"}</span>
          </button>
        </div>
      </Modal>

      <motion.button
        whileHover={{ scale: 1.02 }}
        whileTap={{ scale: 0.98 }}
        onClick={() => setIsModalOpen(true)}
        disabled={isLoading}
        className="w-full bg-gradient-to-r from-jega-blue-600 to-jega-blue-900 text-white py-4 px-6 rounded-xl font-semibold text-lg shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-3"
      >
        {isLoading ? (
          <Loader2 className="w-5 h-5 animate-spin" />
        ) : (
          <CreditCard className="w-5 h-5" />
        )}
        <span>{isLoading ? "Redirigiendo..." : "Proceder al Pago"}</span>
      </motion.button>

      <div className="text-xs text-gray-500 text-center">
        <p>🧪 Pago de prueba: $10 COP</p>
        <p>Pago seguro procesado por Wompi</p>
        <p>Tarjetas de crédito, débito, PSE y efectivo</p>
      </div>
    </div>
  );
};

export default PaymentButton;