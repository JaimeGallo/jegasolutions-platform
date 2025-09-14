import { useState, useCallback } from "react";

// La URL base de tu API de backend. Debería estar en una variable de entorno.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "/api";

export const useWompi = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const createPayment = useCallback(
    async (paymentData) => {
      setIsLoading(true);
      setError(null);

      try {
        // 1. Llamar a nuestro backend para crear el pago
        const response = await fetch(`${API_BASE_URL}/payments/create`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(paymentData),
        });

        const result = await response.json();

        if (!response.ok || !result.checkoutUrl) {
          throw new Error(
            result.message || "Error al iniciar el pago desde el servidor."
          );
        }

        // 2. Redirigir al usuario a la URL de checkout de Wompi
        window.location.href = result.checkoutUrl;
      } catch (err) {
        console.error("Error creating payment:", err);
        setError(err.message);
      } finally {
        setIsLoading(false);
      }
    },
    [] // No hay dependencias externas, ya que todo está contenido en la función.
  );

  return { createPayment, isLoading, error };
};
