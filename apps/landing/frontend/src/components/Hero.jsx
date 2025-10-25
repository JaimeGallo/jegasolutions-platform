import { motion } from 'framer-motion';
import { ArrowRight } from 'lucide-react';

const Hero = ({ onContactClick, onDemoClick }) => {
  return (
    <section className="min-h-screen bg-gray-50 flex items-center justify-center text-gray-900 relative overflow-hidden pt-32 pb-16">
      <div className="relative z-10 text-center max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Logo con mejor espaciado */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          className="mb-8"
        >
          <img
            src="/LogoV2.png"
            alt="JEGASolutions Logo"
            className="mx-auto h-auto w-full max-w-[20rem] object-contain relative -translate-x-2 md:max-w-[24rem] lg:max-w-[28rem]"
          />
        </motion.div>

        {/* Descripción principal */}
        <motion.p
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.2 }}
          className="text-lg md:text-xl text-gray-600 mb-10 max-w-3xl mx-auto leading-relaxed"
        >
          Suite digital modular para la transformación de procesos
          administrativos. Automatiza, optimiza y digitaliza con herramientas
          inteligentes que{' '}
          <span className="text-jega-blue-800 font-semibold">
            revolucionan la gestión empresarial
          </span>
          .
        </motion.p>

        {/* Botones de acción */}
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.4 }}
          className="flex flex-col sm:flex-row gap-4 sm:gap-6 justify-center mb-16"
        >
          <button
            onClick={onDemoClick}
            className="btn-primary text-base md:text-lg px-8 py-4 group"
          >
            <span className="flex items-center justify-center">
              Ver Módulos
              <ArrowRight className="w-5 h-5 ml-2 group-hover:translate-x-1 transition-transform" />
            </span>
          </button>
          <button
            onClick={onContactClick}
            className="btn-outline text-base md:text-lg px-8 py-4 group"
          >
            <span className="flex items-center justify-center">
              Contáctanos
              <ArrowRight className="w-5 h-5 ml-2 group-hover:translate-x-1 transition-transform" />
            </span>
          </button>
        </motion.div>

        {/* Sección de aplicabilidad */}
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ duration: 1, delay: 0.6 }}
          className="pt-8 border-t border-gray-200"
        >
          <p className="text-gray-600 text-sm mb-6">
            Una solución que se adapta a tu industria:
          </p>
          <div className="flex flex-wrap justify-center gap-x-6 gap-y-3 sm:gap-x-8 lg:gap-x-10 text-gray-700 text-sm">
            <span className="flex items-center">
              <div className="w-2 h-2 bg-jega-gold-400 rounded-full mr-2"></div>
              Sector Industrial
            </span>
            <span className="flex items-center">
              <div className="w-2 h-2 bg-jega-gold-400 rounded-full mr-2"></div>
              Logística y Transporte
            </span>
            <span className="flex items-center">
              <div className="w-2 h-2 bg-jega-gold-400 rounded-full mr-2"></div>
              Retail y Comercio
            </span>
            <span className="flex items-center">
              <div className="w-2 h-2 bg-jega-gold-400 rounded-full mr-2"></div>
              Servicios Profesionales
            </span>
            <span className="flex items-center">
              <div className="w-2 h-2 bg-jega-gold-400 rounded-full mr-2"></div>
              Hotelería y Turismo
            </span>
            <span className="flex items-center">
              <div className="w-2 h-2 bg-jega-gold-400 rounded-full mr-2"></div>
              Tecnología y Software
            </span>
            <span className="flex items-center">
              <div className="w-2 h-2 bg-jega-gold-400 rounded-full mr-2"></div>
              Construcción e Inmobiliaria
            </span>
            <span className="flex items-center">
              <div className="w-2 h-2 bg-jega-gold-400 rounded-full mr-2"></div>
              Restaurantes y Alimentación
            </span>
          </div>
        </motion.div>
      </div>
    </section>
  );
};

export default Hero;
