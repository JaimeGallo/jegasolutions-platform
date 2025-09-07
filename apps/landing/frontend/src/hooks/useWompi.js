import { useState, useCallback } from "react";

export const useWompi = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const initializeWompi = useCallback(() => {
    return new Promise((resolve, reject) => {
      if (window.WompiCheckoutWidget) {
        resolve(window.WompiCheckoutWidget);
        return;
      }

      const script = document.createElement("script");
      script.src = "https://checkout.wompi.co/widget.js";
      script.async = true;
      script.onload = () => {
        if (window.WompiCheckoutWidget) {
          resolve(window.WompiCheckoutWidget);
        } else {
          reject(new Error("Wompi widget failed to load"));
        }
      };
      script.onerror = () => reject(new Error("Failed to load Wompi script"));
      document.head.appendChild(script);
    });
  }, []);

  const createPayment = useCallback(
    async (paymentData) => {
      setIsLoading(true);
      setError(null);

      try {
        const WompiCheckoutWidget = await initializeWompi();

        const checkout = new WompiCheckoutWidget({
          publicKey: import.meta.env.VITE_WOMPI_PUBLIC_KEY,
          currency: "COP",
          amountInCents: paymentData.amount * 100,
          reference: paymentData.reference,
          redirectUrl: paymentData.redirectUrl,
          customerData: paymentData.customerData,
          taxInCents: paymentData.taxInCents || 0,
          customerEmail: paymentData.customerEmail,
          phoneNumber: paymentData.phoneNumber,
          customerFullName: paymentData.customerFullName,
        });

        checkout.open();

        return checkout;
      } catch (err) {
        setError(err.message);
        throw err;
      } finally {
        setIsLoading(false);
      }
    },
    [initializeWompi]
  );

  return {
    createPayment,
    isLoading,
    error,
  };
};
