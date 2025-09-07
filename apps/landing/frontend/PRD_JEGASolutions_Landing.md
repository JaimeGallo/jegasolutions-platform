# 📑 PRD – Landing Page JEGASolutions

## 1. Objetivo

Diseñar e implementar una **landing page corporativa** para **JEGASolutions** que:

- Presente la **suite digital modular** (Horas Extra + Report Builder con IA + futuros módulos).
- Promueva la **consultoría tecnológica** como línea de negocio adicional.
- Integre la **calculadora de precios interactiva**.
- Mantenga un estilo **moderno, minimalista y horizontal** (inspirado en Framer), siguiendo la **línea gráfica del logo** (paleta azul/índigo con dorado).

---

## 2. Público Objetivo

- **Gerentes y directores** de operaciones, talento humano y administración en pymes, medianas y grandes empresas.
- **Sectores:** industrial, logístico, hotelero, bares, restaurantes, almacenes de cadena, público.
- **Usuarios clave:** tomadores de decisión que buscan **digitalizar procesos administrativos** con bajo costo de entrada.

---

## 3. Alcance

### La landing page incluirá:

#### 🟦 **Sección 1 – Hero Principal (primer pantallazo)**

- logo de **JEGASolutions** (archivo proporcionado).
- Slogan: **“Soluciones que nacen del corazón.”**
- Video corto / GIF mostrando el módulo de horas extra en acción.
- Botón CTA primario: **“Solicita demo”**.
- Botón CTA secundario: **“Contáctanos”**.
- Menú superior minimalista con 3 ítems:
  - **Módulos**
  - **Consultoría**
  - **Contacto**

#### 🟦 **Sección 2 – Bloques Horizontales (One Page)**

- **Bloque 1 – Módulos**

  - Horas Extra (automatización + cumplimiento normativo).
  - Report Builder con IA (análisis + narrativas inteligentes).
  - Próximos módulos (teaser con “Muy pronto”).

- **Bloque 2 – Consultoría Tecnológica**

  - Breve texto: “Acompañamos a tu empresa en su transformación digital”.
  - Servicios: diagnóstico, diseño de soluciones, cumplimiento normativo, optimización de procesos.

- **Bloque 3 – Calculadora de Precios**
  - Integrar la **JEGAPricingCalculator**.
  - Desplegable horizontal (sin scroll vertical excesivo).
  - Modelos SaaS y On-Premise, comparación rápida.

#### 🟦 **Sección 3 – Footer Compacto**

- Datos legales.
- Correo: soporte@jegasolutions.com
- Redes sociales (LinkedIn, Twitter, GitHub).
- Copyright.

---

## 4. Requisitos Funcionales

1. **Frontend en React + Vite + Tailwind CSS.**
2. **Calculadora de precios integrada** (componente existente `JEGAPricingCalculator`).
3. **Responsive design** (desktop horizontal, adaptativo a móvil ).
4. **CTA con tracking** (clics en “Solicita demo” y “Contáctanos” medidos con Google Analytics/GA4).
5. **Animaciones suaves** tipo Framer Motion:
   - Fade-in en hero.
   - Hover states en botones/cards.
6. **Footer fijo**, siempre visible en pantallas pequeñas.

---

## 5. Requisitos No Funcionales

- **Performance:** Lighthouse score >90.
- **SEO:** meta tags, título optimizado “JEGASolutions – Suite Digital Modular”.
- **Seguridad:** HTTPS obligatorio, sin datos sensibles almacenados en frontend.
- **Accesibilidad:** contraste alto (azul/índigo + dorado), soporte teclado y lectores de pantalla.

---

## 6. Estilo y Lineamientos UI

- **Colores base:**
  - Azul/Índigo (#030B1B / #f5cc00).
  - Dorado (#facc15) para acentos y botones CTA.
- **Tipografía:** Sans-serif moderna (Inter, Poppins o similar).
- **Diseño:**
  - **Estructura horizontal en bloques** (tipo carrusel con flechas).
  - **Hero fullscreen** con logo centrado y fondo azul degradado.
  - **Cards minimalistas** para módulos y consultoría.
  - **Calculadora** en card principal, con colores consistentes.

---

## 7. Integraciones

- **Google Analytics (GA4):** para medir tráfico y conversiones.
- **WhatsApp Business / Formulario Contacto:** en CTA de “Contáctanos”.
- **Hosting:** Vercel recomendado (consistente con Vite + React).

---
