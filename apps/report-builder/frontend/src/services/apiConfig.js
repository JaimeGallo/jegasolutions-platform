/**
 * Centralized API URL configuration
 * Ensures consistent API URL handling across all services
 */

/**
 * Get the base API URL with proper /api suffix
 * @returns {string} The complete API base URL
 */
export const getApiBaseUrl = () => {
  const baseUrl = import.meta.env.VITE_API_URL || "http://localhost:5000";
  // Remove trailing slash if present
  const cleanUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
  // Add /api if not already present
  return cleanUrl.endsWith('/api') ? cleanUrl : `${cleanUrl}/api`;
};

/**
 * Get the raw API URL without /api suffix (for direct API calls that already include /api in path)
 * @returns {string} The base URL without /api
 */
export const getRawApiUrl = () => {
  const baseUrl = import.meta.env.VITE_API_URL || "http://localhost:5000";
  // Remove trailing slash if present
  const cleanUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
  // Remove /api if present
  return cleanUrl.endsWith('/api') ? cleanUrl.slice(0, -4) : cleanUrl;
};

export default {
  getApiBaseUrl,
  getRawApiUrl,
};

