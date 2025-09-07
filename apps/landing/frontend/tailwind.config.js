/** @type {import('tailwindcss').Config} */
import forms from "@tailwindcss/forms";

export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        "jega-blue": {
          50: "#eef7ff",
          100: "#d9eeff",
          200: "#bce2ff",
          300: "#8ed2ff",
          400: "#5ac0ff",
          500: "#31a9ff",
          600: "#0d8eff",
          700: "#0077ff",
          800: "#0062d1",
          900: "#0d47a1", // Base from PRD
        },
        "jega-gold": {
          50: "#fefce8",
          100: "#fef9c3",
          200: "#fef08a",
          300: "#fde047",
          400: "#facc15", // Base from PRD
          500: "#eab308",
          600: "#ca8a04",
          700: "#a16207",
          800: "#854d0e",
          900: "#713f12",
        },
        "jega-indigo": {
          800: "#1e3a8a", // Base from PRD
          900: "#312e81",
        },
      },
      animation: {
        float: "float 6s ease-in-out infinite",
      },
      keyframes: {
        float: {
          "0%, 100%": { transform: "translateY(0px)" },
          "50%": { transform: "translateY(-20px)" },
        },
      },
    },
  },
  plugins: [forms],
};
