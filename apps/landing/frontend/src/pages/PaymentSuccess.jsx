import React from "react";
import { motion } from "framer-motion";
import { CheckCircle, ArrowRight, Home } from "lucide-react";
import { Link } from "react-router-dom";

const PaymentSuccess = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-green-50 to-blue-50 flex items-center justify-center p-4">
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
          <CheckCircle className="w-20 h-20 text-green-500 mx-auto" />
        </motion.div>

        <motion.h1
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.3 }}
          className="text-3xl font-bold text-gray-900 mb-4"
        >
          ¡Pago Exitoso!
        </motion.h1>

        <motion.p
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.4 }}
          className="text-gray-600 mb-6"
        >
          Tu pago ha sido procesado correctamente. Estamos configurando tu
          cuenta y te enviaremos las credenciales de acceso por correo
          electrónico en los próximos minutos.
        </motion.p>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.5 }}
          className="space-y-3"
        >
          <div className="bg-green-50 border border-green-200 rounded-lg p-4">
            <h3 className="font-semibold text-green-800 mb-2">
              Próximos pasos:
            </h3>
            <ul className="text-sm text-green-700 space-y-1 text-left">
              <li>• Recibirás un correo con tus credenciales</li>
              <li>• Tu subdominio estará listo en 5-10 minutos</li>
              <li>• Podrás acceder a tus módulos desde tu dashboard</li>
            </ul>
          </div>

          <div className="flex flex-col sm:flex-row gap-3">
            <Link
              to="/"
              className="flex items-center justify-center gap-2 bg-jega-blue-600 text-white px-6 py-3 rounded-lg hover:bg-jega-blue-700 transition-colors"
            >
              <Home className="w-4 h-4" />
              Volver al Inicio
            </Link>
            <button
              onClick={() =>
                (window.location.href = "mailto:soporte@jegasolutions.co")
              }
              className="flex items-center justify-center gap-2 bg-gray-100 text-gray-700 px-6 py-3 rounded-lg hover:bg-gray-200 transition-colors"
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

export default PaymentSuccess;
