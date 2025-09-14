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
      const paymentData = {
        amount: amount,
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
        taxInCents: Math.round(amount * 0.19 * 100), // IVA 19%
      };

      onPaymentInitiated?.(paymentData);
      await createPayment(paymentData);
      setIsModalOpen(false);
    } catch (err) {
      console.error("Error initiating payment:", err);
      alert("Error al procesar el pago. Por favor intenta nuevamente.");
    }
  };

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-4">
        <p className="text-red-700 text-sm">Error: {error}</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="Datos de Facturación"
      >
        <div className="space-y-4">
          <input
            type="email"
            placeholder="Correo electrónico *"
            value={customerData.email}
            onChange={(e) =>
              setCustomerData((prev) => ({ ...prev, email: e.target.value }))
            }
            className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-jega-blue-500 focus:border-jega-blue-500"
            required
          />
          {validationErrors.email && (
            <p className="text-red-500 text-xs mt-1">
              {validationErrors.email}
            </p>
          )}
          <input
            type="text"
            placeholder="Nombre completo *"
            value={customerData.fullName}
            onChange={(e) =>
              setCustomerData((prev) => ({
                ...prev,
                fullName: e.target.value,
              }))
            }
            className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-jega-blue-500 focus:border-jega-blue-500"
            required
          />
          {validationErrors.fullName && (
            <p className="text-red-500 text-xs mt-1">
              {validationErrors.fullName}
            </p>
          )}
          <input
            type="tel"
            placeholder="Teléfono (opcional)"
            value={customerData.phone}
            onChange={(e) =>
              setCustomerData((prev) => ({ ...prev, phone: e.target.value }))
            }
            className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-jega-blue-500 focus:border-jega-blue-500"
          />
          <button
            onClick={handlePayment}
            disabled={isLoading}
            className="w-full bg-gradient-to-r from-jega-blue-600 to-jega-blue-900 text-white py-3 px-6 rounded-lg font-semibold text-lg shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-3"
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
        <p>Pago seguro procesado por Wompi</p>
        <p>Tarjetas de crédito, débito, PSE y efectivo</p>
      </div>
    </div>
  );
};

export default PaymentButton;
