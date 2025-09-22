import { useState, useCallback } from "react";

// La URL base de tu API de backend. Debería estar en una variable de entorno.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "";

export const useWompi = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const createPayment = useCallback(async (paymentData) => {
    setIsLoading(true);
    setError(null);

    try {
      // Generar referencia única si no se proporciona
      const reference =
        paymentData.reference ||
        `PAY-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;

      const requestPayload = {
        reference: reference,
        amount: parseFloat(paymentData.amount),
        currency: paymentData.currency || "COP", // Campo requerido
        customerEmail: paymentData.customerEmail,
        customerName: paymentData.customerName,
        customerPhone: paymentData.customerPhone || null,
        redirectUrl: paymentData.redirectUrl || window.location.origin, // Campo opcional pero útil
        metadata: paymentData.metadata
          ? JSON.stringify(paymentData.metadata)
          : null,
      };

      console.log("Sending payment request:", requestPayload);

      const response = await fetch(`${API_BASE_URL}/api/payments/create`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(requestPayload),
      });

      if (!response.ok) {
        const errorData = await response
          .json()
          .catch(() => ({ message: "Server error" }));
        console.error("Server error response:", errorData);
        throw new Error(
          errorData.message || `Server error: ${response.status}`
        );
      }

      const result = await response.json();

      if (!result.checkoutUrl && !result.reference) {
        throw new Error("Invalid response from server");
      }

      // Si el backend devuelve una URL de checkout, redirigir
      if (result.checkoutUrl) {
        window.location.href = result.checkoutUrl;
      } else {
        // Si no, puedes construir la URL o manejar la respuesta según tu lógica
        console.log("Payment created successfully:", result);
        return result;
      }
    } catch (err) {
      console.error("Error creating payment:", err);
      setError(err.message);
      throw err; // Re-lanzar para que el componente que llama pueda manejar el error
    } finally {
      setIsLoading(false);
    }
  }, []);

  return { createPayment, isLoading, error };
};
