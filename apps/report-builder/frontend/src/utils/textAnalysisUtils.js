/**
 * Formatea valores de Excel para mostrarlos correctamente
 * @param {any} value - Valor a formatear
 * @returns {string|number} - Valor formateado
 */
export const formatExcelValue = (value) => {
  if (value === null || value === undefined) {
    return "";
  }

  // Si es un número, retornarlo directamente
  if (typeof value === "number") {
    return value;
  }

  // Si es una cadena, limpiar espacios
  if (typeof value === "string") {
    return value.trim();
  }

  // Si es una fecha de Excel (número serial)
  if (typeof value === "number" && value > 40000 && value < 50000) {
    // Convertir número serial de Excel a fecha
    const date = new Date((value - 25569) * 86400 * 1000);
    return date.toLocaleDateString();
  }

  // Para otros tipos, convertir a string
  return String(value);
};

/**
 * Convierte valores de Excel a números
 * @param {any} value - Valor a convertir
 * @returns {number} - Número o 0 si no se puede convertir
 */
export const excelValueToNumber = (value) => {
  if (value === null || value === undefined || value === "") {
    return 0;
  }

  const num = Number(value);
  return isNaN(num) ? 0 : num;
};

/**
 * Limpia y normaliza texto para análisis
 * @param {string} text - Texto a limpiar
 * @returns {string} - Texto limpio
 */
export const cleanText = (text) => {
  if (!text) return "";
  
  return text
    .trim()
    .replace(/\s+/g, " ") // Reemplazar múltiples espacios con uno solo
    .replace(/\n+/g, "\n"); // Normalizar saltos de línea
};

/**
 * Extrae palabras clave de un texto
 * @param {string} text - Texto a analizar
 * @param {number} limit - Número máximo de palabras clave
 * @returns {string[]} - Array de palabras clave
 */
export const extractKeywords = (text, limit = 10) => {
  if (!text) return [];

  // Palabras comunes a ignorar (stop words en español)
  const stopWords = new Set([
    "el", "la", "de", "que", "y", "a", "en", "un", "ser", "se", "no", "haber",
    "por", "con", "su", "para", "como", "estar", "tener", "le", "lo", "todo",
    "pero", "más", "hacer", "o", "poder", "decir", "este", "ir", "otro", "ese",
    "si", "me", "ya", "ver", "porque", "dar", "cuando", "él", "muy", "sin",
  ]);

  const words = text
    .toLowerCase()
    .replace(/[^\wáéíóúñü\s]/g, "") // Mantener solo letras, números y espacios
    .split(/\s+/)
    .filter((word) => word.length > 3 && !stopWords.has(word));

  // Contar frecuencias
  const frequencies = {};
  words.forEach((word) => {
    frequencies[word] = (frequencies[word] || 0) + 1;
  });

  // Ordenar por frecuencia y retornar las top N
  return Object.entries(frequencies)
    .sort((a, b) => b[1] - a[1])
    .slice(0, limit)
    .map(([word]) => word);
};

/**
 * Calcula estadísticas básicas de un array de números
 * @param {number[]} numbers - Array de números
 * @returns {object} - Objeto con estadísticas
 */
export const calculateStats = (numbers) => {
  if (!numbers || numbers.length === 0) {
    return { sum: 0, avg: 0, min: 0, max: 0, count: 0 };
  }

  const validNumbers = numbers.filter((n) => typeof n === "number" && !isNaN(n));
  
  if (validNumbers.length === 0) {
    return { sum: 0, avg: 0, min: 0, max: 0, count: 0 };
  }

  const sum = validNumbers.reduce((acc, val) => acc + val, 0);
  const avg = sum / validNumbers.length;
  const min = Math.min(...validNumbers);
  const max = Math.max(...validNumbers);

  return {
    sum,
    avg,
    min,
    max,
    count: validNumbers.length,
  };
};

