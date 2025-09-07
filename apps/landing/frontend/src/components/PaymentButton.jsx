import React, { useState } from "react";
import { motion } from "framer-motion";
import { CreditCard, Loader2 } from "lucide-react";
import { useWompi } from "../hooks/useWompi";

const PaymentButton = ({
  amount,
  modules,
  deploymentType,
  employeeCount,
  onPaymentInitiated,
}) => {
  const { createPayment, isLoading, error } = useWompi();
  const [showPaymentForm, setShowPaymentForm] = useState(false);
  const [customerData, setCustomerData] = useState({
    email: "",
    fullName: "",
    phone: "",
  });

  const generateReference = () => {
    const timestamp = Date.now();
    const modulesStr = Object.keys(modules)
      .filter((key) => modules[key])
      .join("-");
    return `JEGA-${modulesStr}-${deploymentType}-${timestamp}`;
  };

  const handlePayment = async () => {
    if (!showPaymentForm) {
      setShowPaymentForm(true);
      return;
    }

    if (!customerData.email || !customerData.fullName) {
      alert("Por favor completa todos los campos obligatorios");
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
      {showPaymentForm && (
        <motion.div
          initial={{ opacity: 0, height: 0 }}
          animate={{ opacity: 1, height: "auto" }}
          exit={{ opacity: 0, height: 0 }}
          className="bg-white border border-gray-200 rounded-lg p-4 space-y-3"
        >
          <h4 className="font-semibold text-gray-800">Datos de Facturación</h4>
          <div className="grid grid-cols-1 gap-3">
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
            <input
              type="tel"
              placeholder="Teléfono (opcional)"
              value={customerData.phone}
              onChange={(e) =>
                setCustomerData((prev) => ({ ...prev, phone: e.target.value }))
              }
              className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-jega-blue-500 focus:border-jega-blue-500"
            />
          </div>
        </motion.div>
      )}

      <motion.button
        whileHover={{ scale: 1.02 }}
        whileTap={{ scale: 0.98 }}
        onClick={handlePayment}
        disabled={isLoading}
        className="w-full bg-gradient-to-r from-jega-blue-600 to-jega-blue-900 text-white py-4 px-6 rounded-xl font-semibold text-lg shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-3"
      >
        {isLoading ? (
          <>
            <Loader2 className="w-5 h-5 animate-spin" />
            Procesando...
          </>
        ) : (
          <>
            <CreditCard className="w-5 h-5" />
            {showPaymentForm ? "Pagar con Wompi" : "Proceder al Pago"}
          </>
        )}
      </motion.button>

      <div className="text-xs text-gray-500 text-center">
        <p>Pago seguro procesado por Wompi</p>
        <p>Tarjetas de crédito, débito, PSE y efectivo</p>
      </div>
    </div>
  );
};

export default PaymentButton;
