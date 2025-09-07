import React, { useState, useEffect } from "react";
import {
  Calculator,
  Users,
  Building2,
  Clock,
  Cpu,
  FileText,
  Check,
  X,
} from "lucide-react";
import { motion } from "framer-motion";

const getCompanySizeForEmployees = (count) => {
  if (count <= 10) return "micro";
  if (count <= 50) return "small";
  if (count <= 200) return "medium";
  return "large";
};

const PricingCalculator = () => {
  const [deploymentType, setDeploymentType] = useState("saas");
  const [companySize, setCompanySize] = useState("small");
  const [selectedModules, setSelectedModules] = useState({
    extraHours: false,
    reports: false,
  });
  const [employeeCount, setEmployeeCount] = useState(50);
  const [customEmployeeCount, setCustomEmployeeCount] = useState("");

  useEffect(() => {
    const actualEmployeeCount = customEmployeeCount
      ? parseInt(customEmployeeCount)
      : employeeCount;

    if (actualEmployeeCount > 0 && !isNaN(actualEmployeeCount)) {
      const newSize = getCompanySizeForEmployees(actualEmployeeCount);
      if (newSize !== companySize) {
        setCompanySize(newSize);
      }
    }
  }, [employeeCount, customEmployeeCount, companySize]);

  const basePricing = {
    saas: {
      micro: { min: 97, max: 139 },
      small: { min: 157, max: 189 },
      medium: { min: 199, max: 349 },
      large: { min: 499, max: 1999 },
    },
    onpremise: {
      micro: { license: 5000, maintenance: 0.2, implementation: 2000 },
      small: { license: 8000, maintenance: 0.22, implementation: 3000 },
      medium: { license: 15000, maintenance: 0.23, implementation: 5000 },
      large: { license: 25000, maintenance: 0.25, implementation: 8000 },
    },
  };

  const moduleMultipliers = {
    extraHours: { saas: 1.0, onpremise: 1.0 },
    reports: { saas: 1.3, onpremise: 1.4 }, // Mayor valor por IA
  };

  const companySizes = {
    micro: { label: "Micro (hasta 10 empleados)", range: [1, 10] },
    small: { label: "Pequeña (11-50 empleados)", range: [11, 50] },
    medium: { label: "Mediana (51-200 empleados)", range: [51, 200] },
    large: { label: "Grande (201+ empleados)", range: [201, 5000] },
  };

  // Calcular precio automáticamente
  const calculatePrice = () => {
    const selectedModulesCount =
      Object.values(selectedModules).filter(Boolean).length;
    if (selectedModulesCount === 0) {
      return { monthly: 0, annual: 0, setup: 0, total3Years: 0 };
    }

    const actualEmployeeCount = customEmployeeCount
      ? parseInt(customEmployeeCount) || 0
      : employeeCount;

    if (actualEmployeeCount <= 0) {
      return { monthly: 0, annual: 0, setup: 0, total3Years: 0 };
    }

    const sizeForPricing = getCompanySizeForEmployees(actualEmployeeCount);
    const pricing = basePricing[deploymentType][sizeForPricing];
    const sizeInfo = companySizes[sizeForPricing];

    // Calcular multiplicador por módulos seleccionados
    let moduleMultiplier = 0;
    if (selectedModules.extraHours)
      moduleMultiplier += moduleMultipliers.extraHours[deploymentType];
    if (selectedModules.reports)
      moduleMultiplier += moduleMultipliers.reports[deploymentType];

    if (deploymentType === "saas") {
      const [minEmployees, maxEmployees] = sizeInfo.range;
      let monthlyPrice;

      if (actualEmployeeCount <= minEmployees) {
        monthlyPrice = pricing.min;
      } else if (
        actualEmployeeCount >= maxEmployees &&
        sizeForPricing !== "large"
      ) {
        monthlyPrice = pricing.max;
      } else {
        const progress =
          (actualEmployeeCount - minEmployees) / (maxEmployees - minEmployees);
        monthlyPrice = pricing.min + (pricing.max - pricing.min) * progress;
      }

      monthlyPrice *= moduleMultiplier;

      const annualPrice = monthlyPrice * 12 * 0.85; // 15% descuento anual

      return {
        monthly: Math.round(monthlyPrice),
        annual: Math.round(annualPrice),
        setup: 500,
        total3Years: Math.round(annualPrice * 3 + 500),
      };
    } else {
      const licensePrice = pricing.license * moduleMultiplier;
      const maintenanceAnnual = licensePrice * pricing.maintenance;
      const implementation = pricing.implementation;

      return {
        license: Math.round(licensePrice),
        maintenance: Math.round(maintenanceAnnual),
        implementation: Math.round(implementation),
        total3Years: Math.round(
          licensePrice + maintenanceAnnual * 3 + implementation
        ),
      };
    }
  };

  const price = calculatePrice();

  const handleModuleChange = (module) => {
    setSelectedModules((prev) => ({
      ...prev,
      [module]: !prev[module],
    }));
  };

  const handleCompanySizeChange = (size) => {
    const representativeCounts = {
      micro: 10,
      small: 30,
      medium: 100,
      large: 250,
    };
    setEmployeeCount(representativeCounts[size]);
    setCustomEmployeeCount("");
  };

  const handleEmployeeCountChange = (value) => {
    setEmployeeCount(value);
    setCustomEmployeeCount("");
  };

  return (
    <div className="section-with-header bg-gradient-to-br from-blue-400 to-blue-800">
      <motion.div
        initial={{ opacity: 0, y: 30 }}
        whileInView={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.8 }}
        viewport={{ once: true }}
        className="relative z-10 bg-white rounded-3xl shadow-2xl p-8 w-full max-w-6xl border border-gray-100"
      >
        <div className="text-center mb-10">
          <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Calcula tu <span className="gradient-text">Inversión</span>
          </h2>
          <p className="text-lg text-gray-600 max-w-3xl mx-auto">
            Obtén una cotización personalizada en tiempo real.
          </p>
        </div>

        <div className="grid lg:grid-cols-3 gap-8 items-start">
          {/* Configuración */}
          <div className="lg:col-span-2 grid md:grid-cols-2 gap-6">
            {/* Deployment Type */}
            <div className="bg-gray-50 rounded-xl p-4">
              <h3 className="font-semibold mb-3 flex items-center gap-2 text-gray-800">
                <Building2 className="w-5 h-5 text-jega-blue-600" />
                Despliegue
              </h3>
              <div className="grid grid-cols-2 gap-2">
                <button
                  onClick={() => setDeploymentType("saas")}
                  className={`p-3 rounded-lg border text-center text-sm transition-all h-full ${
                    deploymentType === "saas"
                      ? "border-jega-blue-500 bg-jega-blue-50 text-jega-blue-700 font-semibold"
                      : "border-gray-200 hover:border-jega-blue-300"
                  }`}
                >
                  <div>
                    <div className="font-bold">SaaS (Nube)</div>
                    <div className="text-xs text-gray-500 font-normal mt-1">
                      Suscripción mensual/anual
                    </div>
                  </div>
                </button>
                <button
                  onClick={() => setDeploymentType("onpremise")}
                  className={`p-3 rounded-lg border text-center text-sm transition-all h-full ${
                    deploymentType === "onpremise"
                      ? "border-jega-blue-500 bg-jega-blue-50 text-jega-blue-700 font-semibold"
                      : "border-gray-200 hover:border-jega-blue-300"
                  }`}
                >
                  <div>
                    <div className="font-bold">On-Premise</div>
                    <div className="text-xs text-gray-500 font-normal mt-1">
                      Compra única a perpetuidad
                    </div>
                  </div>
                </button>
              </div>
            </div>

            {/* Company Size */}
            <div className="bg-gray-50 rounded-xl p-4">
              <h3 className="font-semibold mb-3 flex items-center gap-2 text-gray-800">
                <Users className="w-5 h-5 text-jega-blue-600" />
                Tamaño
              </h3>
              <select
                value={companySize}
                onChange={(e) => handleCompanySizeChange(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-lg focus:ring-1 focus:ring-blue-500 focus:border-blue-500 text-sm bg-white"
              >
                {Object.entries(companySizes).map(([size, info]) => (
                  <option key={size} value={size}>
                    {info.label}
                  </option>
                ))}
              </select>
            </div>

            {/* Modules */}
            <div className="bg-gray-50 rounded-xl p-4">
              <h3 className="font-semibold mb-3 flex items-center gap-2 text-gray-800">
                <Cpu className="w-5 h-5 text-jega-blue-600" />
                Módulos
              </h3>
              <div className="space-y-2">
                <div
                  className={`p-2 rounded-lg border-2 cursor-pointer transition-all ${
                    selectedModules.extraHours
                      ? "border-green-500 bg-green-50"
                      : "border-gray-200"
                  }`}
                  onClick={() => handleModuleChange("extraHours")}
                >
                  <div className="flex items-center gap-3">
                    <div
                      className={`w-5 h-5 rounded border-2 flex items-center justify-center flex-shrink-0 ${
                        selectedModules.extraHours
                          ? "bg-green-500 border-green-500"
                          : "border-gray-300"
                      }`}
                    >
                      {selectedModules.extraHours && (
                        <Check className="w-3 h-3 text-white" />
                      )}
                    </div>
                    <span className="font-semibold text-sm">
                      Gestión de Horas Extra
                    </span>
                  </div>
                </div>
                <div
                  className={`p-2 rounded-lg border-2 cursor-pointer transition-all ${
                    selectedModules.reports
                      ? "border-green-500 bg-green-50"
                      : "border-gray-200"
                  }`}
                  onClick={() => handleModuleChange("reports")}
                >
                  <div className="flex items-center gap-3">
                    <div
                      className={`w-5 h-5 rounded border-2 flex items-center justify-center flex-shrink-0 ${
                        selectedModules.reports
                          ? "bg-green-500 border-green-500"
                          : "border-gray-300"
                      }`}
                    >
                      {selectedModules.reports && (
                        <Check className="w-3 h-3 text-white" />
                      )}
                    </div>
                    <span className="font-semibold text-sm">
                      Reportes con IA
                    </span>
                  </div>
                </div>
              </div>
            </div>

            {/* Employee Count */}
            <div className="bg-gray-50 rounded-xl p-4">
              <h3 className="font-semibold mb-3 flex items-center gap-2 text-gray-800">
                <Calculator className="w-5 h-5 text-jega-blue-600" />
                Empleados
              </h3>
              <div className="space-y-3">
                <input
                  type="range"
                  min="1"
                  max="500"
                  value={customEmployeeCount || employeeCount}
                  onChange={(e) => handleEmployeeCountChange(e.target.value)}
                  className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer"
                />
                <input
                  type="number"
                  placeholder="O cantidad personalizada"
                  value={customEmployeeCount || employeeCount}
                  onChange={(e) => setCustomEmployeeCount(e.target.value)}
                  className="w-full p-2 border border-gray-300 rounded-lg focus:ring-1 focus:ring-blue-500 focus:border-blue-500 text-sm"
                />
              </div>
            </div>
          </div>

          {/* Resultados */}
          <div className="lg:col-span-1">
            <div className="bg-gradient-to-br from-jega-blue-50 via-jega-indigo-50 to-jega-blue-100 p-6 rounded-2xl border border-jega-blue-200 h-full flex flex-col">
              <h3 className="text-2xl font-bold mb-4 text-jega-blue-900 flex items-center gap-2">
                <Calculator className="w-6 h-6" />
                Cotización
              </h3>

              {Object.values(selectedModules).some(Boolean) ? (
                <div className="space-y-4 flex-grow flex flex-col">
                  <div className="bg-white/60 rounded-lg p-4 flex-grow">
                    <div className="text-sm text-gray-600 mb-2">
                      {companySizes[companySize].label} (
                      {customEmployeeCount
                        ? parseInt(customEmployeeCount)
                        : employeeCount}
                      )
                    </div>

                    {deploymentType === "saas" ? (
                      <div className="space-y-2">
                        <div className="flex justify-between items-baseline">
                          <span className="text-gray-700">Mensual:</span>
                          <span className="font-bold text-3xl text-jega-blue-800">
                            ${price.monthly}
                          </span>
                        </div>
                        <div className="flex justify-between items-baseline text-green-700">
                          <span>Anual (15% desc.):</span>
                          <span className="font-semibold text-lg">
                            ${price.annual}
                          </span>
                        </div>
                        <div className="flex justify-between items-baseline text-sm text-gray-600">
                          <span>Setup único:</span>
                          <span>${price.setup}</span>
                        </div>
                        <hr className="my-2 border-blue-200" />
                        <div className="flex justify-between items-center font-bold text-lg">
                          <span>Total 3 años:</span>
                          <span className="text-green-800">
                            ${price.total3Years}
                          </span>
                        </div>
                      </div>
                    ) : (
                      <div className="space-y-2">
                        <div className="flex justify-between items-baseline">
                          <span>Licencia:</span>
                          <span className="font-bold text-3xl text-jega-blue-800">
                            ${price.license}
                          </span>
                        </div>
                        <div className="flex justify-between items-baseline">
                          <span>Implementación:</span>
                          <span className="font-semibold text-lg">
                            ${price.implementation}
                          </span>
                        </div>
                        <div className="flex justify-between items-baseline text-sm text-gray-600">
                          <span>Mantenimiento/año:</span>
                          <span>${price.maintenance}</span>
                        </div>
                        <hr className="my-2 border-blue-200" />
                        <div className="flex justify-between items-center font-bold text-lg">
                          <span>Total 3 años:</span>
                          <span className="text-green-800">
                            ${price.total3Years}
                          </span>
                        </div>
                      </div>
                    )}
                  </div>

                  {/* Incluye */}
                  <div className="bg-blue-100/50 rounded-lg p-3 border border-blue-200">
                    <h4 className="font-semibold text-blue-800 mb-2 text-sm">
                      ✅ Incluye
                    </h4>
                    <div className="text-xs text-blue-700 grid grid-cols-2 gap-1">
                      {deploymentType === "saas" ? (
                        <>
                          <span>• Hosting y mantenimiento</span>
                          <span>• Actualizaciones</span>
                          <span>• Soporte técnico</span>
                          <span>• Backups automáticos</span>
                        </>
                      ) : (
                        <>
                          <span>• Licencia perpetua</span>
                          <span>• Instalación Docker</span>
                          <span>• Capacitación inicial</span>
                          <span>• Soporte implementación</span>
                        </>
                      )}
                    </div>
                  </div>
                </div>
              ) : (
                <div className="text-center py-8 flex-grow flex flex-col justify-center items-center">
                  <div className="text-5xl mb-4">🎯</div>
                  <p className="text-gray-600">
                    Selecciona al menos un módulo para ver la cotización
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default PricingCalculator;
