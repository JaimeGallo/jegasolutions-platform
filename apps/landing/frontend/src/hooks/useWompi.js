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

      // Extraer correctamente el customerName desde diferentes posibles ubicaciones
      const customerName =
        paymentData.customerName ||
        paymentData.customerData?.name ||
        paymentData.customerData?.customerName ||
        paymentData.customerData?.firstName ||
        "Cliente"; // fallback si no se encuentra

      const requestPayload = {
        reference: reference,
        amount: parseFloat(paymentData.amount),
        currency: paymentData.currency || "COP", // Campo requerido
        customerEmail: paymentData.customerEmail,
        customerName: customerName, // Ahora siempre tendrá un valor
        customerPhone:
          paymentData.customerPhone ||
          paymentData.customerData?.phone ||
          paymentData.customerData?.phoneNumber ||
          null,
        redirectUrl: paymentData.redirectUrl || window.location.origin,
        metadata: paymentData.metadata
          ? JSON.stringify(paymentData.metadata)
          : null,
      };

      // Log para debugging - muestra todos los campos que se envían
      console.log("Payment initiated:", paymentData);
      console.log("Sending payment request:", requestPayload);

      const response = await fetch(`${API_BASE_URL}/api/payments/create`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(requestPayload),
      });

      // Manejo mejorado de errores
      if (!response.ok) {
        let errorMessage = `Server error: ${response.status}`;

        try {
          const errorData = await response.json();
          console.error("Server error response:", errorData);

          // Si hay errores de validación, mostrarlos específicamente
          if (errorData.errors && typeof errorData.errors === "object") {
            const validationErrors = Object.entries(errorData.errors)
              .map(
                ([field, errors]) =>
                  `${field}: ${
                    Array.isArray(errors) ? errors.join(", ") : errors
                  }`
              )
              .join("; ");
            errorMessage = `Errores de validación: ${validationErrors}`;
          } else if (errorData.message) {
            errorMessage = errorData.message;
          }
        } catch (parseError) {
          console.error("Could not parse error response:", parseError);
        }

        throw new Error(errorMessage);
      }

      const result = await response.json();
      console.log("Payment created successfully:", result);

      // Manejar diferentes tipos de respuesta del backend
      if (result.checkoutUrl) {
        window.location.href = result.checkoutUrl;
      } else if (result.reference) {
        // Si solo devuelve la referencia, puedes construir la URL de Wompi o manejar según tu lógica
        return result;
      } else {
        throw new Error(
          "Respuesta inválida del servidor - falta checkoutUrl o reference"
        );
      }
    } catch (err) {
      console.error("Error creating payment:", err);
      setError(err.message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  return { createPayment, isLoading, error };
};
