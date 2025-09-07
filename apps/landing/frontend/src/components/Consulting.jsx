import { motion } from "framer-motion";
import { BarChart, Zap, Shield, Award, ArrowRight } from "lucide-react";

const services = [
  {
    icon: BarChart,
    title: "Diagnóstico",
    description:
      "Análisis profundo de procesos actuales e identificación de oportunidades de mejora.",
    color: "blue",
  },
  {
    icon: Zap,
    title: "Optimización",
    description:
      "Diseño de soluciones personalizadas y eficientes para tu empresa.",
    color: "green",
  },
  {
    icon: Shield,
    title: "Cumplimiento",
    description:
      "Garantía de conformidad con normativas vigentes y mejores prácticas.",
    color: "purple",
  },
  {
    icon: Award,
    title: "Acompañamiento",
    description:
      "Soporte continuo en la implementación y adopción de soluciones.",
    color: "yellow",
  },
];

const processSteps = [
  {
    step: "01",
    title: "Análisis",
    description: "Evaluación detallada de procesos y necesidades.",
  },
  {
    step: "02",
    title: "Diseño",
    description: "Propuesta de solución personalizada.",
  },
  {
    step: "03",
    title: "Implementación",
    description: "Desarrollo e instalación de la solución.",
  },
  {
    step: "04",
    title: "Soporte",
    description: "Acompañamiento continuo y optimización.",
  },
];

const getIconColor = (color) => {
  const colors = {
    blue: "from-jega-blue-500 to-jega-blue-600",
    green: "from-green-500 to-green-600",
    purple: "from-purple-500 to-purple-600",
    yellow: "from-jega-gold-400 to-jega-gold-500",
  };
  return colors[color] || colors.blue;
};

const Consulting = ({ onContactClick }) => {
  return (
    <section className="section-with-header bg-gradient-to-br from-blue-100 to-blue-300">
      <div className="container-max">
        <div className="grid lg:grid-cols-2 gap-16 items-center">
          <motion.div
            initial={{ opacity: 0, x: -30 }}
            whileInView={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.8 }}
            viewport={{ once: true }}
          >
            <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-6">
              Consultoría <span className="gradient-text">Tecnológica</span>
            </h2>
            <p className="text-xl text-gray-600 mb-8 leading-relaxed">
              Acompañamos a tu empresa en su transformación digital con un
              enfoque integral que va más allá del software. Entendemos tu
              negocio y diseñamos soluciones que generan valor real.
            </p>
            <div className="grid sm:grid-cols-2 gap-6 mb-8">
              {services.map((service, index) => (
                <motion.div
                  key={service.title}
                  initial={{ opacity: 0, y: 20 }}
                  whileInView={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.6, delay: index * 0.1 }}
                  viewport={{ once: true }}
                  className="flex items-start space-x-3 group"
                >
                  <div
                    className={`w-10 h-10 bg-gradient-to-br ${getIconColor(
                      service.color
                    )} rounded-xl flex items-center justify-center flex-shrink-0 group-hover:scale-110 transition-transform duration-300`}
                  >
                    <service.icon className="w-5 h-5 text-white" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900 mb-2">
                      {service.title}
                    </h3>
                    <p className="text-sm text-gray-600 leading-relaxed">
                      {service.description}
                    </p>
                  </div>
                </motion.div>
              ))}
            </div>
            <button className="btn-outline group" onClick={onContactClick}>
              <span className="flex items-center">
                Agenda una consulta gratuita
                <ArrowRight className="w-4 h-4 ml-2 group-hover:translate-x-1 transition-transform" />
              </span>
            </button>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, x: 30 }}
            whileInView={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.8 }}
            viewport={{ once: true }}
          >
            <h3 className="text-3xl font-bold text-gray-900 mb-8 text-center lg:text-left">
              Nuestro Proceso de Consultoría
            </h3>
            <div className="grid grid-cols-2 gap-6">
              {processSteps.map((item, index) => (
                <motion.div
                  key={item.step}
                  initial={{ opacity: 0, y: 20 }}
                  whileInView={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.6, delay: index * 0.1 }}
                  viewport={{ once: true }}
                  className="text-center group bg-white/60 backdrop-blur-sm p-4 rounded-xl border border-gray-200/80 shadow-sm"
                >
                  <div className="w-16 h-16 bg-gradient-to-br from-jega-blue-500 to-jega-indigo-800 rounded-full flex items-center justify-center mx-auto mb-4 group-hover:scale-110 transition-transform duration-300">
                    <span className="text-white font-bold text-lg">
                      {item.step}
                    </span>
                  </div>
                  <h4 className="text-lg font-semibold text-gray-900 mb-2">
                    {item.title}
                  </h4>
                  <p className="text-gray-600 text-sm">{item.description}</p>
                </motion.div>
              ))}
            </div>
          </motion.div>
        </div>
      </div>
    </section>
  );
};

export default Consulting;
