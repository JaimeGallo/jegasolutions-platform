import { useState, useEffect } from "react";

/**
 * Un hook personalizado para detectar si una media query de CSS coincide.
 * @param {string} query - La media query a evaluar (ej. '(max-width: 768px)').
 * @returns {boolean} - `true` si la media query coincide, de lo contrario `false`.
 */
export const useMediaQuery = (query) => {
  // Inicializa el estado con el valor correcto en el cliente,
  // de forma segura para Server-Side Rendering (SSR).
  const [matches, setMatches] = useState(() => {
    if (typeof window === "undefined") {
      return false;
    }
    return window.matchMedia(query).matches;
  });

  useEffect(() => {
    const mediaQueryList = window.matchMedia(query);
    const listener = (event) => setMatches(event.matches);

    // Usamos addEventListener que es el estándar moderno.
    mediaQueryList.addEventListener("change", listener);

    return () => mediaQueryList.removeEventListener("change", listener);
  }, [query]); // El efecto solo necesita volver a ejecutarse si la 'query' cambia.

  return matches;
};
