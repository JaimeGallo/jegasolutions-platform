import React from "react";
import { motion } from "framer-motion";
import ExtraHoursModule from "../modules/ExtraHoursModule";
import AIReportsModule from "../modules/AIReportsModule";
import UpcomingModules from "../modules/UpcomingModules";

const ModulesSection = () => {
  return (
    <section className="section-with-header bg-gradient-to-br from-gray-50 to-blue-100">
      <div className="container-max grid lg:grid-cols-5 gap-16 items-center">
        <motion.div
          initial={{ opacity: 0, x: -30 }}
          whileInView={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.8 }}
          viewport={{ once: true }}
          className="lg:col-span-2 text-center lg:text-left mb-12 lg:mb-0"
        >
          <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-6">
            Suite Digital <span className="gradient-text">Modular</span>
          </h2>
          <p className="text-xl text-gray-600 max-w-lg mx-auto lg:mx-0 leading-relaxed">
            Módulos especializados que se integran perfectamente para crear una
            solución completa adaptada a las necesidades específicas de tu
            empresa.
          </p>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, x: 30 }}
          whileInView={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.8 }}
          viewport={{ once: true }}
          className="lg:col-span-3"
        >
          <div className="grid sm:grid-cols-2 gap-8">
            <ExtraHoursModule />
            <AIReportsModule />
          </div>
        </motion.div>

        {/* Upcoming Modules */}
        {/*<UpcomingModules />*/}
      </div>
    </section>
  );
};

export default ModulesSection;
