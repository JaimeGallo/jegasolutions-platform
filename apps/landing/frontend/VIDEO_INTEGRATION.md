# 🎥 Integración de Videos Demo - JEGASolutions Landing

## ✅ Implementación Completada

Se ha implementado un sistema de modales de video para mostrar demos de los módulos en la landing page.

## 📍 Componentes Actualizados

### 1. **VideoModal.jsx** (Nuevo)
Componente reutilizable que maneja la visualización de videos en modal.

**Características:**
- ✅ Soporte para videos de YouTube
- ✅ Soporte para videos de Vimeo
- ✅ Soporte para videos locales (MP4, WebM, etc.)
- ✅ Cierre con tecla ESC
- ✅ Cierre al hacer clic fuera del modal
- ✅ Autoplay al abrir
- ✅ Diseño responsive
- ✅ Animaciones suaves con Framer Motion

### 2. **ExtraHoursModule.jsx** (Actualizado)
- Botón "Ver Demo" agregado con icono de Play
- Modal de video integrado
- Color del botón: azul (jega-blue-600)

### 3. **AIReportsModule.jsx** (Actualizado)
- Botón "Ver Demo" agregado con icono de Play
- Modal de video integrado
- Color del botón: morado (purple-600)

## 🔧 Cómo Cambiar las URLs de los Videos

### Ubicación de las URLs:
Las URLs están definidas en cada componente de módulo:

**ExtraHoursModule.jsx (línea ~114):**
```jsx
<VideoModal
  isOpen={isVideoOpen}
  onClose={() => setIsVideoOpen(false)}
  videoUrl="https://www.youtube.com/watch?v=dQw4w9WgXcQ"  // ← CAMBIAR AQUÍ
  title="Demo - Gestión de Horas Extra"
/>
```

**AIReportsModule.jsx (línea ~116):**
```jsx
<VideoModal
  isOpen={isVideoOpen}
  onClose={() => setIsVideoOpen(false)}
  videoUrl="https://www.youtube.com/watch?v=dQw4w9WgXcQ"  // ← CAMBIAR AQUÍ
  title="Demo - Reportes con IA"
/>
```

## 📹 Formatos de Video Soportados

### Opción 1: YouTube (Recomendado)
```jsx
videoUrl="https://www.youtube.com/watch?v=TU_VIDEO_ID"
// o
videoUrl="https://youtu.be/TU_VIDEO_ID"
```

**Ventajas:**
- ✅ No consume espacio en tu servidor
- ✅ Streaming optimizado
- ✅ Subtítulos automáticos disponibles
- ✅ Control de calidad adaptativo

### Opción 2: Vimeo
```jsx
videoUrl="https://vimeo.com/TU_VIDEO_ID"
```

**Ventajas:**
- ✅ Calidad profesional
- ✅ Sin anuncios
- ✅ Controles personalizables

### Opción 3: Video Local
```jsx
videoUrl="/videos/demo-extra-hours.mp4"
```

**Ventajas:**
- ✅ Control total
- ✅ Sin dependencias externas
- ✅ Funciona offline

**Ubicación recomendada:**
- Coloca los videos en: `apps/landing/frontend/public/videos/`
- Luego referencia como: `videoUrl="/videos/nombre-del-video.mp4"`

## 🎬 Recomendaciones para los Videos

### Duración
- **Ideal:** 30-60 segundos
- **Máximo:** 2 minutos

### Contenido Sugerido
**Horas Extra:**
- Registro de horas por empleado
- Aprobación por manager
- Visualización de reportes
- Exportación a Excel

**Reportes con IA:**
- Carga de datos
- Generación automática de narrativa
- Visualización de insights
- Exportación del reporte

### Especificaciones Técnicas
Si subes videos locales:
- **Formato:** MP4 (H.264)
- **Resolución:** 1920x1080 (Full HD)
- **Aspect Ratio:** 16:9
- **Bitrate:** 5-8 Mbps
- **Audio:** AAC, 128-192 kbps

## 🚀 Próximos Pasos

1. **Graba o prepara los videos demo** de cada módulo
2. **Sube a YouTube/Vimeo** (recomendado) o a la carpeta `public/videos/`
3. **Actualiza las URLs** en los componentes como se indica arriba
4. **Prueba la funcionalidad** en el navegador

## 💡 Características del Modal

- **Apertura:** Clic en botón "Ver Demo"
- **Cierre:** 
  - Botón X en la esquina superior derecha
  - Clic fuera del modal
  - Tecla ESC
- **Autoplay:** El video inicia automáticamente al abrir
- **Pausa:** Al cerrar el modal, el video se detiene

## 🎨 Personalización del Botón

Si deseas cambiar el estilo del botón "Ver Demo", edita las clases CSS en cada módulo:

```jsx
className="inline-flex items-center space-x-2 px-6 py-3 bg-jega-blue-600 hover:bg-jega-blue-700 text-white rounded-lg transition-all duration-300 hover:scale-105 shadow-md hover:shadow-lg"
```

## ❓ Preguntas Frecuentes

**P: ¿Puedo usar otros servicios de video?**
R: Sí, pero necesitarás agregar la lógica de embed en el componente VideoModal.jsx

**P: ¿Los videos se cargan automáticamente?**
R: No, solo se cargan cuando el usuario hace clic en "Ver Demo", optimizando la velocidad de carga inicial.

**P: ¿Funciona en móviles?**
R: Sí, el modal es completamente responsive y funciona en todos los dispositivos.

