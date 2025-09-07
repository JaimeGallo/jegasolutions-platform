# 🚀 JEGASolutions Landing Page

Una landing page moderna y profesional para JEGASolutions, diseñada con React, Tailwind CSS y Framer Motion.

## ✨ Características

- **Diseño Moderno**: Interfaz limpia y profesional con animaciones suaves
- **Componentes Modulares**: Arquitectura bien estructurada y reutilizable
- **Responsive Design**: Optimizada para todos los dispositivos
- **Animaciones**: Transiciones fluidas con Framer Motion
- **SEO Optimizado**: Meta tags y estructura semántica
- **Performance**: Código optimizado y lazy loading

## 🏗️ Estructura del Proyecto

```
src/
├── components/
│   ├── Hero.jsx              # Sección principal con logo y CTA
│   ├── Modules.jsx           # Módulos disponibles y próximos
│   ├── Consulting.jsx        # Servicios de consultoría
│   ├── PricingCalculator.jsx # Calculadora de precios interactiva
│   ├── Contact.jsx           # Formulario de contacto
│   ├── Footer.jsx            # Pie de página
│   └── JEGASolutionsLanding.jsx # Componente principal
├── index.css                 # Estilos globales y Tailwind
└── App.jsx                   # Punto de entrada
```

## 🚀 Tecnologías Utilizadas

- **React 18** - Biblioteca de interfaz de usuario
- **Vite** - Build tool y dev server
- **Tailwind CSS** - Framework de CSS utility-first
- **Framer Motion** - Biblioteca de animaciones
- **Lucide React** - Iconos modernos
- **PostCSS** - Procesador de CSS

## 📦 Instalación

1. **Clonar el repositorio**

   ```bash
   git clone <repository-url>
   cd jegasolutions-landing
   ```

2. **Instalar dependencias**

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

- **Hero**: Cambiar logo, título, descripción y botones CTA
- **Modules**: Agregar/quitar módulos y características
- **Consulting**: Modificar servicios y testimonios
- **PricingCalculator**: Ajustar precios y opciones
- **Contact**: Personalizar formulario e información de contacto

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

```json
{
  "dev": "vite", // Servidor de desarrollo
  "build": "vite build", // Construir para producción
  "preview": "vite preview", // Previsualizar build
  "lint": "eslint . --ext js,jsx --report-unused-disable-directives --max-warnings 0"
}
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
