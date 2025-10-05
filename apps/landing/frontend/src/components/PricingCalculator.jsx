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
import PaymentButton from "./PaymentButton";

const getCompanySizeForEmployees = (count) => {
  if (count <= 10) return "micro";
  if (count <= 50) return "small";
  if (count <= 200) return "medium";
  return "large";
};

// Helper para formatear a pesos colombianos
const formatCOP = (value) => {
  return new Intl.NumberFormat("es-CO", {
    style: "currency",
    currency: "COP",
    minimumFractionDigits: 0,
  }).format(value);
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

  // ==========================================
  // 🧪 MODO PRUEBA - TODOS LOS PRECIOS A $10 COP
  // ==========================================
  // ⚠️ CONFIGURACIÓN TEMPORAL PARA TESTING
  // Todos los módulos cuestan $10 COP independientemente del tamaño de empresa
  
  const TESTING_PRICES = {
    saas: {
      micro: { min: 10, max: 10 },
      small: { min: 10, max: 10 },
      medium: { min: 10, max: 10 },
      large: { min: 10, max: 10 },
    },
    onpremise: {
      micro: { license: 10, maintenance: 0, implementation: 10 },
      small: { license: 10, maintenance: 0, implementation: 10 },
      medium: { license: 10, maintenance: 0, implementation: 10 },
      large: { license: 10, maintenance: 0, implementation: 10 },
    },
  };

  const basePricing = TESTING_PRICES;

  // 🧪 MULTIPLICADORES DE MÓDULOS EN MODO PRUEBA
  // Fijados en 1.0 para mantener precio único de $10 COP
  const moduleMultipliers = {
    extraHours: { saas: 1.0, onpremise: 1.0 },
    reports: { saas: 1.0, onpremise: 1.0 },
  };

  /* ==========================================
   * 💰 PRECIOS REALES (COMENTADOS TEMPORALMENTE)
   * ==========================================
   * Descomentar esta sección cuando se termine el periodo de prueba
   * 
   * // --- Tasa de Cambio USD a COP ---
   * const USD_TO_COP_RATE = 4026;
   * 
   * // --- Precios Base en USD ---
   * const basePricingUSD = {
   *   saas: {
   *     micro: { min: 97, max: 139 },     // $390,522 - $559,614 COP
   *     small: { min: 157, max: 189 },    // $632,082 - $761,094 COP
   *     medium: { min: 199, max: 349 },   // $801,174 - $1,405,074 COP
   *     large: { min: 499, max: 1999 },   // $2,008,974 - $8,047,974 COP
   *   },
   *   onpremise: {
   *     micro: { license: 5000, maintenance: 0.2, implementation: 2000 },
   *     small: { license: 8000, maintenance: 0.22, implementation: 3000 },
   *     medium: { license: 15000, maintenance: 0.23, implementation: 5000 },
   *     large: { license: 25000, maintenance: 0.25, implementation: 8000 },
   *   },
   * };
   * 
   * // --- Conversión a COP ---
   * const convertPricingToCOP = (pricingUSD, rate) => {
   *   const converted = JSON.parse(JSON.stringify(pricingUSD));
   *   for (const type in converted) {
   *     for (const size in converted[type]) {
   *       for (const key in converted[type][size]) {
   *         if (key !== "maintenance") {
   *           converted[type][size][key] *= rate;
   *         }
   *       }
   *     }
   *   }
   *   return converted;
   * };
   * 
   * const basePricing = convertPricingToCOP(basePricingUSD, USD_TO_COP_RATE);
   * 
   * // --- Multiplicadores de Módulos Reales ---
   * const moduleMultipliers = {
   *   extraHours: { saas: 1.0, onpremise: 1.0 },
   *   reports: { saas: 1.3, onpremise: 1.4 }, // Mayor valor por IA
   * };
   */

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

      // 🧪 MODO PRUEBA: Sin descuento anual ni setup fee
      const annualPrice = monthlyPrice * 12; // Sin descuento en prueba
      const setupFee = 10; // Setup también a $10 COP

      return {
        monthly: Math.round(monthlyPrice),
        annual: Math.round(annualPrice),
        setup: setupFee,
        total3Years: Math.round(annualPrice * 3 + setupFee),
      };

      /* CÁLCULO REAL (comentado):
       * const annualPrice = monthlyPrice * 12 * 0.85; // 15% descuento anual
       * return {
       *   monthly: Math.round(monthlyPrice),
       *   annual: Math.round(annualPrice),
       *   setup: 500 * USD_TO_COP_RATE,
       *   total3Years: Math.round(annualPrice * 3 + 500 * USD_TO_COP_RATE),
       * };
       */
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
      micro: 5,
      small: 25,
      medium: 100,
      large: 500,
    };
    setCompanySize(size);
    setEmployeeCount(representativeCounts[size]);
    setCustomEmployeeCount("");
  };

  const handlePaymentInitiated = (paymentData) => {
    console.log("Payment initiated with data:", paymentData);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-blue-50 py-16 px-4">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="max-w-7xl mx-auto"
      >
        {/* 🧪 Banner de Modo Prueba */}
        <div className="mb-8 bg-yellow-50 border-l-4 border-yellow-400 p-4 rounded-r-lg">
          <div className="flex items-center">
            <div className="flex-shrink-0">
              <svg className="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
              </svg>
            </div>
            <div className="ml-3">
              <p className="text-sm text-yellow-700">
                <strong>🧪 MODO PRUEBA ACTIVADO:</strong> Todos los módulos tienen un precio de <strong>$10 COP</strong> para facilitar testing de la plataforma de pagos.
              </p>
            </div>
          </div>
        </div>

        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Calculadora de Precios
          </h1>
          <p className="text-xl text-gray-600">
            Personaliza tu solución según las necesidades de tu empresa
          </p>
        </div>

        <div className="grid lg:grid-cols-3 gap-8">
          {/* Panel Izquierdo - Configuración */}
          <div className="lg:col-span-2 space-y-6">
            {/* Tipo de Despliegue */}
            <div className="bg-white rounded-xl shadow-lg p-6">
              <h3 className="text-xl font-semibold mb-4 flex items-center gap-2">
                <Building2 className="w-5 h-5 text-blue-600" />
                Tipo de Despliegue
              </h3>
              <div className="grid grid-cols-2 gap-4">
                <button
                  onClick={() => setDeploymentType("saas")}
                  className={`p-4 rounded-lg border-2 transition-all ${
                    deploymentType === "saas"
                      ? "border-blue-600 bg-blue-50"
                      : "border-gray-200 hover:border-blue-300"
                  }`}
                >
                  <Cpu className="w-8 h-8 mx-auto mb-2 text-blue-600" />
                  <p className="font-semibold">SaaS</p>
                  <p className="text-sm text-gray-600">Nube (mensual)</p>
                </button>
                <button
                  onClick={() => setDeploymentType("onpremise")}
                  className={`p-4 rounded-lg border-2 transition-all ${
                    deploymentType === "onpremise"
                      ? "border-blue-600 bg-blue-50"
                      : "border-gray-200 hover:border-blue-300"
                  }`}
                >
                  <Building2 className="w-8 h-8 mx-auto mb-2 text-blue-600" />
                  <p className="font-semibold">On-Premise</p>
                  <p className="text-sm text-gray-600">Licencia perpetua</p>
                </button>
              </div>
            </div>

            {/* Tamaño de Empresa */}
            {deploymentType === "saas" && (
              <div className="bg-white rounded-xl shadow-lg p-6">
                <h3 className="text-xl font-semibold mb-4 flex items-center gap-2">
                  <Users className="w-5 h-5 text-blue-600" />
                  Tamaño de Empresa
                </h3>
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Número de empleados
                    </label>
                    <input
                      type="number"
                      value={customEmployeeCount || employeeCount}
                      onChange={(e) => setCustomEmployeeCount(e.target.value)}
                      min="1"
                      max="5000"
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="Ingrese número de empleados"
                    />
                  </div>
                  <div className="grid grid-cols-2 gap-2">
                    {Object.entries(companySizes).map(([key, { label }]) => (
                      <button
                        key={key}
                        onClick={() => handleCompanySizeChange(key)}
                        className={`px-3 py-2 text-sm rounded-lg border transition-all ${
                          companySize === key
                            ? "bg-blue-600 text-white border-blue-600"
                            : "bg-white text-gray-700 border-gray-300 hover:border-blue-400"
                        }`}
                      >
                        {label.split("(")[0]}
                      </button>
                    ))}
                  </div>
                </div>
              </div>
            )}

            {/* Selección de Módulos */}
            <div className="bg-white rounded-xl shadow-lg p-6">
              <h3 className="text-xl font-semibold mb-4 flex items-center gap-2">
                <FileText className="w-5 h-5 text-blue-600" />
                Módulos Disponibles
              </h3>
              <div className="space-y-4">
                {/* Módulo Extra Hours */}
                <div
                  onClick={() => handleModuleChange("extraHours")}
                  className={`p-4 rounded-lg border-2 cursor-pointer transition-all ${
                    selectedModules.extraHours
                      ? "border-green-500 bg-green-50"
                      : "border-gray-200 hover:border-green-300"
                  }`}
                >
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-2">
                        <Clock className="w-5 h-5 text-green-600" />
                        <h4 className="font-semibold text-lg">
                          GestorHorasExtra
                        </h4>
                        {selectedModules.extraHours && (
                          <Check className="w-5 h-5 text-green-600" />
                        )}
                      </div>
                      <p className="text-sm text-gray-600 mt-1">
                        Control completo de horas extra, reportes automáticos y
                        cumplimiento normativo
                      </p>
                      <div className="mt-2 text-xs text-green-700 font-medium">
                        🧪 PRUEBA: $10 COP {/* REAL: Desde $390,522 COP/mes */}
                      </div>
                    </div>
                  </div>
                </div>

                {/* Módulo Reports */}
                <div
                  onClick={() => handleModuleChange("reports")}
                  className={`p-4 rounded-lg border-2 cursor-pointer transition-all ${
                    selectedModules.reports
                      ? "border-purple-500 bg-purple-50"
                      : "border-gray-200 hover:border-purple-300"
                  }`}
                >
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-2">
                        <FileText className="w-5 h-5 text-purple-600" />
                        <h4 className="font-semibold text-lg">
                          ReportBuilder con IA
                        </h4>
                        {selectedModules.reports && (
                          <Check className="w-5 h-5 text-purple-600" />
                        )}
                      </div>
                      <p className="text-sm text-gray-600 mt-1">
                        Generación automática de reportes con IA, análisis de
                        Excel y exportación multi-formato
                      </p>
                      <div className="mt-2 text-xs text-purple-700 font-medium">
                        🧪 PRUEBA: $10 COP {/* REAL: Desde $507,678 COP/mes */}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Panel Derecho - Resumen de Precio */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-xl shadow-lg p-6 sticky top-6">
              <h3 className="text-2xl font-bold mb-6 text-center flex items-center justify-center gap-2">
                <Calculator className="w-6 h-6 text-blue-600" />
                Cotización
              </h3>

              {Object.values(selectedModules).some((v) => v) ? (
                <div className="space-y-6 flex flex-col h-full">
                  {/* Módulos Seleccionados */}
                  <div className="bg-gray-50 rounded-lg p-4">
                    <h4 className="font-semibold text-sm text-gray-700 mb-2">
                      Módulos seleccionados:
                    </h4>
                    <div className="space-y-1 text-sm">
                      {selectedModules.extraHours && (
                        <div className="flex items-center gap-2 text-green-700">
                          <Check className="w-4 h-4" />
                          <span>GestorHorasExtra</span>
                        </div>
                      )}
                      {selectedModules.reports && (
                        <div className="flex items-center gap-2 text-purple-700">
                          <Check className="w-4 h-4" />
                          <span>ReportBuilder con IA</span>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Precio */}
                  <div className="bg-blue-50 rounded-lg p-4 border-2 border-blue-200">
                    {deploymentType === "saas" ? (
                      <div className="space-y-2">
                        <div className="flex justify-between items-baseline">
                          <span className="text-gray-700">Mensual:</span>
                          <span className="font-bold text-3xl text-jega-blue-800">
                            {formatCOP(price.monthly)}
                          </span>
                        </div>
                        <div className="flex justify-between items-baseline text-green-700">
                          <span>Anual:</span>
                          <span className="font-semibold text-lg">
                            {formatCOP(price.annual)}
                          </span>
                        </div>
                        <div className="flex justify-between items-baseline text-sm text-gray-600">
                          <span>Setup único:</span>
                          <span>{formatCOP(price.setup)}</span>
                        </div>
                        <hr className="my-2 border-blue-200" />
                        <div className="flex justify-between items-center font-bold text-lg">
                          <span>Total 3 años:</span>
                          <span className="text-green-800">
                            {formatCOP(price.total3Years)}
                          </span>
                        </div>
                      </div>
                    ) : (
                      <div className="space-y-2">
                        <div className="flex justify-between items-baseline">
                          <span>Licencia:</span>
                          <span className="font-bold text-3xl text-jega-blue-800">
                            {formatCOP(price.license)}
                          </span>
                        </div>
                        <div className="flex justify-between items-baseline">
                          <span>Implementación:</span>
                          <span className="font-semibold text-lg">
                            {formatCOP(price.implementation)}
                          </span>
                        </div>
                        <div className="flex justify-between items-baseline text-sm text-gray-600">
                          <span>Mantenimiento/año:</span>
                          <span>{formatCOP(price.maintenance)}</span>
                        </div>
                        <hr className="my-2 border-blue-200" />
                        <div className="flex justify-between items-center font-bold text-lg">
                          <span>Total 3 años:</span>
                          <span className="text-green-800">
                            {formatCOP(price.total3Years)}
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

                  {/* Botón de Pago */}
                  {deploymentType === "saas" && (
                    <PaymentButton
                      amount={price.monthly}
                      modules={selectedModules}
                      deploymentType={deploymentType}
                      employeeCount={
                        customEmployeeCount
                          ? parseInt(customEmployeeCount)
                          : employeeCount
                      }
                      onPaymentInitiated={handlePaymentInitiated}
                    />
                  )}
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