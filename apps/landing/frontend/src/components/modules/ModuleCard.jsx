import React from "react";
import { motion } from "framer-motion";
import { CheckCircle } from "lucide-react";

const ModuleCard = ({ module, index, isUpcoming = false }) => {
  const getColorClasses = (color) => {
    const colors = {
      blue: "from-jega-blue-500 to-jega-blue-600",
      purple: "from-purple-500 to-purple-600",
      green: "from-green-500 to-green-600",
      yellow: "from-jega-gold-400 to-jega-gold-500",
    };
    return colors[color] || colors.blue;
  };

  const getPriceCardClasses = (color) => {
    const colorMap = {
      blue: "bg-blue-50 border-blue-200",
      purple: "bg-purple-50 border-purple-200",
      green: "bg-green-50 border-green-200",
      yellow: "bg-yellow-50 border-yellow-200",
    };
    return colors[color] || colors.blue;
  };

  if (isUpcoming) {
    return (
      <motion.div
        initial={{ opacity: 0, y: 30 }}
        whileInView={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.8, delay: index * 0.1 }}
        viewport={{ once: true }}
        className="bg-gradient-to-br from-jega-gold-50 to-yellow-100 rounded-2xl p-8 border border-jega-gold-200 shadow-lg hover:shadow-xl transition-all duration-300"
      >
        <div className="w-12 h-12 bg-jega-gold-200 rounded-xl flex items-center justify-center mb-4">
          <module.icon className="w-6 h-6 text-jega-gold-600" />
        </div>
        <h4 className="text-xl font-bold text-gray-900 mb-3">{module.title}</h4>
        <p className="text-gray-600 mb-4 text-sm leading-relaxed">
          {module.description}
        </p>
        <div className="bg-jega-gold-200 rounded-lg px-3 py-2 text-center">
          <span className="text-sm font-medium text-jega-gold-800">
            Muy Pronto
          </span>
        </div>
      </motion.div>
    );
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 30 }}
      whileInView={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.8, delay: index * 0.2 }}
      viewport={{ once: true }}
      className="bg-gradient-to-br from-white to-gray-100 rounded-2xl p-8 border border-gray-200 shadow-xl hover:shadow-2xl transition-all duration-300 group"
    >
      <div
        className={`w-16 h-16 bg-gradient-to-br ${getColorClasses(
          module.color
        )} rounded-2xl flex items-center justify-center mb-6 group-hover:scale-110 transition-transform duration-300`}
      >
        <module.icon className="w-8 h-8 text-white" />
      </div>

      <h3 className="text-2xl font-bold text-gray-900 mb-4">{module.title}</h3>
      <p className="text-gray-600 mb-6 leading-relaxed">{module.description}</p>

      <ul className="space-y-3 mb-6">
        {module.features.map((feature, idx) => (
          <li key={idx} className="flex items-center text-sm text-gray-600">
            <CheckCircle className="w-4 h-4 text-green-600 mr-3 flex-shrink-0" />
            {feature}
          </li>
        ))}
      </ul>

      <div
        className={`rounded-xl p-4 border ${getPriceCardClasses(module.color)}`}
      >
        <div className="flex items-center justify-between">
          <span className="text-sm font-medium text-gray-700">Precio</span>
          <span className="text-lg font-bold text-gray-900">
            {module.price}
          </span>
        </div>
      </div>
    </motion.div>
  );
};

export default ModuleCard;
