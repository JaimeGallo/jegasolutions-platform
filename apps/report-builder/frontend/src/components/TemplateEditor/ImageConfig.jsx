import { Upload } from "lucide-react";

const ImageConfig = ({ component = {}, onUpdate }) => {
  const handleImageUpload = (event) => {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        const imageData = e.target.result;
        onUpdate("imageData", imageData);
      };
      reader.readAsDataURL(file);
    }
  };

  const positionOptions = [
    { value: "full-width", label: "Ancho completo", description: "Ocupa todo el ancho de la sección" },
    { value: "full-page", label: "Página completa", description: "Ocupa toda la página del informe como fondo" },
    { value: "left", label: "Izquierda", description: "Alineado a la izquierda" },
    { value: "center", label: "Centro", description: "Centrado en la sección" },
    { value: "right", label: "Derecha", description: "Alineado a la derecha" },
    { value: "custom", label: "Personalizado", description: "Posición específica con coordenadas" },
  ];

  const textPositionOptions = [
    { value: "top", label: "Arriba de la imagen" },
    { value: "bottom", label: "Debajo de la imagen" },
    { value: "overlay-top", label: "Superpuesto arriba" },
    { value: "overlay-bottom", label: "Superpuesto abajo" },
    { value: "overlay-center", label: "Superpuesto centro" },
    { value: "none", label: "Sin texto" },
  ];

  return (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Imagen
        </label>
        <input
          type="file"
          id="image-upload"
          accept="image/*"
          onChange={handleImageUpload}
          className="hidden"
        />
        <button
          onClick={() => document.getElementById("image-upload").click()}
          className="w-full px-4 py-3 bg-blue-50 text-blue-700 rounded hover:bg-blue-100 border border-blue-200 flex items-center justify-center transition-colors"
        >
          <Upload className="h-4 w-4 mr-2" />
          {component.imageData ? "Cambiar imagen" : "Seleccionar imagen"}
        </button>
      </div>

      {component.imageData && (
        <div className="p-2 bg-gray-50 border rounded">
          <img
            src={component.imageData}
            alt="Preview"
            className="w-full h-32 object-contain rounded"
          />
        </div>
      )}

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Posición de la imagen
        </label>
        <select
          value={component.position || "full-width"}
          onChange={(e) => onUpdate("position", e.target.value)}
          className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
        >
          {positionOptions.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
        <p className="text-xs text-gray-500 mt-1">
          {positionOptions.find((o) => o.value === (component.position || "full-width"))?.description}
        </p>
      </div>

      {component.position === "custom" && (
        <div className="grid grid-cols-2 gap-2 p-3 bg-gray-50 border rounded">
          <div>
            <label className="block text-xs text-gray-600 mb-1">X (%)</label>
            <input
              type="number"
              min="0"
              max="100"
              value={component.customPosition?.x || 0}
              onChange={(e) =>
                onUpdate("customPosition", {
                  ...component.customPosition,
                  x: parseInt(e.target.value) || 0,
                })
              }
              className="w-full p-1 border rounded text-sm"
            />
          </div>
          <div>
            <label className="block text-xs text-gray-600 mb-1">Y (%)</label>
            <input
              type="number"
              min="0"
              max="100"
              value={component.customPosition?.y || 0}
              onChange={(e) =>
                onUpdate("customPosition", {
                  ...component.customPosition,
                  y: parseInt(e.target.value) || 0,
                })
              }
              className="w-full p-1 border rounded text-sm"
            />
          </div>
          <div>
            <label className="block text-xs text-gray-600 mb-1">Ancho (%)</label>
            <input
              type="number"
              min="10"
              max="100"
              value={component.customPosition?.width || 50}
              onChange={(e) =>
                onUpdate("customPosition", {
                  ...component.customPosition,
                  width: parseInt(e.target.value) || 50,
                })
              }
              className="w-full p-1 border rounded text-sm"
            />
          </div>
          <div>
            <label className="block text-xs text-gray-600 mb-1">Alto (%)</label>
            <input
              type="number"
              min="10"
              max="100"
              value={component.customPosition?.height || 50}
              onChange={(e) =>
                onUpdate("customPosition", {
                  ...component.customPosition,
                  height: parseInt(e.target.value) || 50,
                })
              }
              className="w-full p-1 border rounded text-sm"
            />
          </div>
        </div>
      )}

      <div className="border-t pt-4">
        <h4 className="text-sm font-medium text-gray-700 mb-2">Texto sobre la imagen</h4>
        
        <div className="space-y-3">
          <div>
            <label className="block text-sm text-gray-600 mb-1">
              Posición del texto
            </label>
            <select
              value={component.textPosition || "none"}
              onChange={(e) => onUpdate("textPosition", e.target.value)}
              className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            >
              {textPositionOptions.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>

          {component.textPosition && component.textPosition !== "none" && (
            <>
              <div>
                <label className="block text-sm text-gray-600 mb-1">
                  Texto
                </label>
                <textarea
                  value={component.overlayText || ""}
                  onChange={(e) => onUpdate("overlayText", e.target.value)}
                  className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
                  rows={3}
                  placeholder="Escribe el texto..."
                />
              </div>

              <div className="p-3 bg-gray-50 border rounded space-y-2">
                <h5 className="text-xs font-medium text-gray-700">Estilo del texto</h5>
                
                <div className="grid grid-cols-2 gap-2">
                  <div>
                    <label className="block text-xs text-gray-600 mb-1">Tamaño</label>
                    <select
                      value={component.textStyle?.fontSize || "medium"}
                      onChange={(e) =>
                        onUpdate("textStyle", {
                          ...component.textStyle,
                          fontSize: e.target.value,
                        })
                      }
                      className="w-full p-1 border rounded text-xs"
                    >
                      <option value="small">Pequeño</option>
                      <option value="medium">Mediano</option>
                      <option value="large">Grande</option>
                      <option value="xlarge">Muy grande</option>
                    </select>
                  </div>

                  <div>
                    <label className="block text-xs text-gray-600 mb-1">Color</label>
                    <input
                      type="color"
                      value={component.textStyle?.color || "#000000"}
                      onChange={(e) =>
                        onUpdate("textStyle", {
                          ...component.textStyle,
                          color: e.target.value,
                        })
                      }
                      className="w-full h-8 border rounded"
                    />
                  </div>
                </div>

                {component.textPosition?.startsWith("overlay") && (
                  <>
                    <div>
                      <label className="block text-xs text-gray-600 mb-1">
                        Color de fondo
                      </label>
                      <input
                        type="color"
                        value={component.textStyle?.backgroundColor || "#ffffff"}
                        onChange={(e) =>
                          onUpdate("textStyle", {
                            ...component.textStyle,
                            backgroundColor: e.target.value,
                          })
                        }
                        className="w-full h-8 border rounded"
                      />
                    </div>

                    <div>
                      <label className="block text-xs text-gray-600 mb-1">
                        Opacidad del fondo: {component.textStyle?.backgroundOpacity || 80}%
                      </label>
                      <input
                        type="range"
                        min="0"
                        max="100"
                        value={component.textStyle?.backgroundOpacity || 80}
                        onChange={(e) =>
                          onUpdate("textStyle", {
                            ...component.textStyle,
                            backgroundOpacity: parseInt(e.target.value),
                          })
                        }
                        className="w-full"
                      />
                    </div>
                  </>
                )}
              </div>
            </>
          )}
        </div>
      </div>

      <div className="border-t pt-4 space-y-2">
        <label className="flex items-center text-sm">
          <input
            type="checkbox"
            checked={component.maintainAspectRatio !== false}
            onChange={(e) => onUpdate("maintainAspectRatio", e.target.checked)}
            className="mr-2"
          />
          Mantener relación de aspecto
        </label>

        <label className="flex items-center text-sm">
          <input
            type="checkbox"
            checked={component.addBorder || false}
            onChange={(e) => onUpdate("addBorder", e.target.checked)}
            className="mr-2"
          />
          Agregar borde
        </label>

        {component.addBorder && (
          <div className="ml-6 grid grid-cols-2 gap-2">
            <div>
              <label className="block text-xs text-gray-600 mb-1">Color del borde</label>
              <input
                type="color"
                value={component.borderColor || "#000000"}
                onChange={(e) => onUpdate("borderColor", e.target.value)}
                className="w-full h-8 border rounded"
              />
            </div>
            <div>
              <label className="block text-xs text-gray-600 mb-1">Grosor (px)</label>
              <input
                type="number"
                min="1"
                max="10"
                value={component.borderWidth || 1}
                onChange={(e) => onUpdate("borderWidth", parseInt(e.target.value) || 1)}
                className="w-full p-1 border rounded text-sm"
              />
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ImageConfig;

