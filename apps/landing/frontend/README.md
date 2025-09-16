# 🚀 JEGASolutions Landing Page

Una landing page moderna y profesional para JEGASolutions, diseñada con React, Tailwind CSS y Framer Motion.

## ✨ Características

- **Diseño Moderno**: Interfaz limpia y profesional con animaciones suaves
- **Componentes Interactivos**: Tarjetas con efecto "flip" para mostrar más detalles
- **Responsive Design**: Optimizada para todos los dispositivos
- **Animaciones**: Transiciones fluidas con Framer Motion
- **SEO Optimizado**: Meta tags y estructura semántica
- **Performance**: Código optimizado y lazy loading
- **Integración de Pagos**: Lista para procesar pagos a través de Wompi

## 🏗️ Estructura del Proyecto

```
src/
├── components/
│   ├── Hero.jsx              # Sección principal con logo y CTA
│   ├── modules/
│   │   ├── ExtraHoursModule.jsx  # Módulo de Horas Extra
│   │   ├── UpcomingModules.jsx   # Módulos en desarrollo
│   │   └── FlippableModuleCard.jsx # Tarjeta reutilizable con efecto flip
│   ├── Contact.jsx           # Formulario de contacto
│   ├── Footer.jsx            # Pie de página
│   ├── PaymentButton.jsx     # Botón de pago con Wompi
│   └── JEGASolutionsLanding.jsx # Componente principal que une las secciones
├── hooks/
│   └── useWompi.js           # Hook para la integración con Wompi
├── utils/
│   └── useMediaQuery.js      # Hook para queries de CSS en JS
├── index.css                 # Estilos globales y Tailwind
└── App.jsx                   # Punto de entrada
```

## 🚀 Tecnologías Utilizadas

- **Vite** - Herramienta de build y servidor de desarrollo ultrarrápido
- **PostCSS** - Procesador de CSS

## 📦 Instalación

1. **Clonar el repositorio**

   ```bash
   git clone <repository-url>
   cd jegasolutions-landing
   ```

2. **Instalar dependencias**
   (Se recomienda usar `npm`, `yarn` o `pnpm`)

   ```bash
   npm install
   ```

3. **Ejecutar en desarrollo**

   ```bash
   npm run dev
   ```

4. **Construir para producción**

   ```bash
   npm run build
   ```

5. **Previsualizar build**
   ```bash
   npm run preview
   ```

## ⚙️ Backend

El frontend se apoya en una API construida con **.NET 7** que gestiona:

- **Procesamiento de Pagos**: Orquesta la creación de transacciones con Wompi y persiste los pagos en la base de datos.
- **Notificaciones por Email**: Envía correos de bienvenida y confirmación de pago usando un servicio SMTP.
- **Arquitectura Limpia**: Sigue principios de Clean Architecture, separando dominio, aplicación e infraestructura.
- **Patrones de Diseño**: Utiliza el patrón Repositorio y Unidad de Trabajo para la abstracción del acceso a datos.

El backend está ubicado en el directorio `apps/landing/backend/`.

---

## 🎨 Personalización

## 🎨 Personalización

### Colores de Marca

Los colores personalizados están definidos en `tailwind.config.js`:

```javascript
colors: {
  'jega-blue': { /* Paleta de azules */ },
  'jega-gold': { /* Paleta de dorados */ },
  'jega-indigo': { /* Paleta de índigos */ }
}
```

### Componentes

Cada sección es un componente independiente que puede ser fácilmente modificado:

- **`Hero.jsx`**: Cambiar logo, eslogan, descripción y botones CTA.
- **`*Module.jsx`**: Agregar/quitar módulos y editar sus características en las tarjetas.
- **`Consulting.jsx`**: Modificar los servicios y el proceso de consultoría.
- **`PricingCalculator.jsx`**: Ajustar la lógica de precios, rangos de empleados y tasas de conversión.
- **`Contact.jsx`**: Personalizar el formulario de contacto y la información de la empresa.

## 📱 Responsive Design

La landing page está optimizada para:

- **Desktop**: 1024px+
- **Tablet**: 768px - 1023px
- **Mobile**: 320px - 767px

## 🚀 Deployment

### Vercel (Recomendado)

1. Conectar repositorio a Vercel
2. Configurar build command: `npm run build`
3. Output directory: `dist`
4. Deploy automático en cada push

### Netlify

1. Conectar repositorio a Netlify
2. Build command: `npm run build`
3. Publish directory: `dist`

### Otros

Cualquier hosting estático que soporte SPA routing.

## 🔧 Scripts Disponibles

```bash
# Inicia el servidor de desarrollo en http://localhost:5173
"dev": "vite"

# Compila la aplicación para producción en la carpeta /dist
"build": "vite build"

# Previsualiza el build de producción localmente
"preview": "vite preview"

# Ejecuta el linter de ESLint para revisar la calidad del código
"lint": "eslint . --ext js,jsx --report-unused-disable-directives --max-warnings 0"
```

## 🎯 Características Principales

### Hero Section

- logo de JEGASolutions integrado
- Slogan "Soluciones que nacen del corazón"
- Botones CTA para demo y contacto
- Indicadores de confianza por sector

### Módulos

- Gestión de Horas Extra (activo)
- Reportes con IA (activo)
- Próximos módulos (teaser)

### Consultoría

- Servicios de diagnóstico y optimización
- Experiencia por sectores
- Testimonios de clientes
- Proceso de consultoría

### Calculadora de Precios

- Modelos SaaS y On-Premise
- Selección de módulos
- Cálculo en tiempo real
- Comparación de costos

### Contacto

- Formulario de demo gratuita
- Información de contacto
- Horarios de atención
- Enlaces a redes sociales

## 🔒 Seguridad

- Formularios con validación del lado del cliente
- No se almacenan datos sensibles en frontend
- HTTPS obligatorio en producción

## 📈 Analytics

Para implementar Google Analytics:

1. Agregar script de GA4 en `index.html`
2. Configurar eventos en botones CTA
3. Trackear conversiones de formularios

## 🤝 Contribución

1. Fork el proyecto
2. Crear rama feature (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE` para más detalles.

## 📞 Soporte

- **Email**: soporte@jegasolutions.com
- **Teléfono**: +57 (305) 123-4567
- **Ubicación**: Medellín, Colombia

---

**JEGASolutions** - Soluciones que nacen del corazón ❤️
