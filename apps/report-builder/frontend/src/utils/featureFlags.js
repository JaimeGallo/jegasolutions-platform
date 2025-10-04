/**
 * Feature Flags para habilitar/deshabilitar funcionalidades
 * Esto permite controlar qué funcionalidades están disponibles
 */

const FEATURE_FLAGS = {
  // Funcionalidades de AI
  NARRATIVE: true, // Generación de narrativas con AI
  CHARTS: true, // Generación automática de gráficos
  KPIS: true, // Generación automática de KPIs
  TRENDS: true, // Análisis de tendencias
  PATTERNS: true, // Detección de patrones
  RECOMMENDATIONS: true, // Recomendaciones basadas en datos
  
  // Funcionalidades avanzadas
  HYBRID_BUILDER: true, // Constructor híbrido de plantillas
  AREA_ASSIGNMENT: true, // Asignación de áreas
  AUTO_ASSIGN_AI: true, // Asignación automática con IA
  
  // Funcionalidades experimentales
  EXPERIMENTAL_AI: false, // Funcionalidades AI experimentales
  ADVANCED_ANALYTICS: true, // Analytics avanzados
};

/**
 * Verifica si una funcionalidad está habilitada
 * @param {string} feature - Nombre de la funcionalidad
 * @returns {boolean} - Si está habilitada
 */
export const isFeatureEnabled = (feature) => {
  return FEATURE_FLAGS[feature] ?? false;
};

/**
 * Obtiene todas las funcionalidades habilitadas
 * @returns {object} - Objeto con todas las funcionalidades
 */
export const getAllFeatureFlags = () => {
  return { ...FEATURE_FLAGS };
};

/**
 * Habilita o deshabilita una funcionalidad (solo en desarrollo)
 * @param {string} feature - Nombre de la funcionalidad
 * @param {boolean} enabled - Si debe estar habilitada
 */
export const setFeatureFlag = (feature, enabled) => {
  if (import.meta.env.DEV) {
    FEATURE_FLAGS[feature] = enabled;
    console.log(`🚩 Feature flag ${feature} = ${enabled}`);
  } else {
    console.warn("Feature flags solo pueden modificarse en desarrollo");
  }
};

/**
 * Obtiene la configuración por defecto de AI
 * @returns {object} - Configuración por defecto
 */
export const getDefaultAIConfig = () => {
  return {
    analysisType: "comprehensive",
    language: "es",
    tone: "professional",
    includeNarrative: true,
    includeCharts: false,
    includeKPIs: false,
    includeTrends: false,
    includePatterns: false,
    includeRecommendations: false,
  };
};

/**
 * Filtra la configuración de AI según los feature flags
 * @param {object} config - Configuración completa
 * @returns {object} - Configuración filtrada
 */
export const getFilteredAIConfig = (config = {}) => {
  const defaultConfig = getDefaultAIConfig();
  const mergedConfig = { ...defaultConfig, ...config };

  // Filtrar funcionalidades no habilitadas
  return {
    analysisType: mergedConfig.analysisType,
    language: mergedConfig.language,
    tone: mergedConfig.tone,
    includeNarrative: mergedConfig.includeNarrative && isFeatureEnabled("NARRATIVE"),
    includeCharts: mergedConfig.includeCharts && isFeatureEnabled("CHARTS"),
    includeKPIs: mergedConfig.includeKPIs && isFeatureEnabled("KPIS"),
    includeTrends: mergedConfig.includeTrends && isFeatureEnabled("TRENDS"),
    includePatterns: mergedConfig.includePatterns && isFeatureEnabled("PATTERNS"),
    includeRecommendations: mergedConfig.includeRecommendations && isFeatureEnabled("RECOMMENDATIONS"),
  };
};

/**
 * Muestra un log de todos los feature flags (solo desarrollo)
 */
export const logFeatureFlags = () => {
  if (import.meta.env.DEV) {
    console.log("🚩 Feature Flags Activos:");
    Object.entries(FEATURE_FLAGS).forEach(([key, value]) => {
      console.log(`  ${key}: ${value ? "✅" : "❌"}`);
    });
  }
};

export default {
  isFeatureEnabled,
  getAllFeatureFlags,
  setFeatureFlag,
  getDefaultAIConfig,
  getFilteredAIConfig,
  logFeatureFlags,
};

