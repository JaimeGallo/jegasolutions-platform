import React from "react";
import { motion } from "framer-motion";
import { XCircle, ArrowRight, Home, RefreshCw } from "lucide-react";
import { Link } from "react-router-dom";

const PaymentFailure = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-red-50 to-orange-50 flex items-center justify-center p-4">
      <motion.div
        initial={{ opacity: 0, scale: 0.9 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.6 }}
        className="bg-white rounded-2xl shadow-2xl p-8 max-w-md w-full text-center"
      >
        <motion.div
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          transition={{ delay: 0.2, type: "spring", stiffness: 200 }}
          className="mb-6"
        >
          <XCircle className="w-20 h-20 text-red-500 mx-auto" />
        </motion.div>

        <motion.h1
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.3 }}
          className="text-3xl font-bold text-gray-900 mb-4"
        >
          Pago No Procesado
        </motion.h1>

        <motion.p
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.4 }}
          className="text-gray-600 mb-6"
        >
          No pudimos procesar tu pago en este momento. Esto puede deberse a
          fondos insuficientes, datos incorrectos o problemas temporales con tu
          banco.
        </motion.p>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.5 }}
          className="space-y-3"
        >
          <div className="bg-red-50 border border-red-200 rounded-lg p-4">
            <h3 className="font-semibold text-red-800 mb-2">
              Posibles causas:
            </h3>
            <ul className="text-sm text-red-700 space-y-1 text-left">
              <li>• Fondos insuficientes en la cuenta</li>
              <li>• Datos de tarjeta incorrectos</li>
              <li>• Problemas temporales del banco</li>
              <li>• Límites de transacción excedidos</li>
            </ul>
          </div>

          <div className="flex flex-col sm:flex-row gap-3">
            <button
              onClick={() => window.history.back()}
              className="flex items-center justify-center gap-2 bg-jega-blue-600 text-white px-6 py-3 rounded-lg hover:bg-jega-blue-700 transition-colors"
            >
              <RefreshCw className="w-4 h-4" />
              Intentar Nuevamente
            </button>
            <Link
              to="/"
              className="flex items-center justify-center gap-2 bg-gray-100 text-gray-700 px-6 py-3 rounded-lg hover:bg-gray-200 transition-colors"
            >
              <Home className="w-4 h-4" />
              Volver al Inicio
            </Link>
          </div>

          <div className="pt-4 border-t border-gray-200">
            <p className="text-sm text-gray-500 mb-2">¿Necesitas ayuda?</p>
            <button
              onClick={() =>
                (window.location.href = "mailto:soporte@jegasolutions.co")
              }
              className="flex items-center justify-center gap-2 text-jega-blue-600 hover:text-jega-blue-700 transition-colors text-sm"
            >
              <ArrowRight className="w-4 h-4" />
              Contactar Soporte
            </button>
          </div>
        </motion.div>
      </motion.div>
    </div>
  );
};

export default PaymentFailure;
