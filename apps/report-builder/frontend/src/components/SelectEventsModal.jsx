import { useState, useEffect } from "react";
import { X } from "lucide-react";

// Servicio temporal de eventos (puedes reemplazarlo con el servicio real)
const EventService = {
  getEventsByArea: async (areaId) => {
    // TODO: Implementar llamada real al backend
    console.log("Cargando eventos para área:", areaId);
    return [];
  },
};

const SelectEventsModal = ({ isOpen, onClose, onSelectEvents, areaId }) => {
  const [availableEvents, setAvailableEvents] = useState([]);
  const [selectedEventIds, setSelectedEventIds] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchEvents = async () => {
      if (isOpen && areaId) {
        setLoading(true);
        try {
          const events = await EventService.getEventsByArea(areaId);
          setAvailableEvents(events);
        } catch (error) {
          console.error("Error loading events:", error);
          setAvailableEvents([]);
        } finally {
          setLoading(false);
        }
      }
    };

    fetchEvents();
  }, [isOpen, areaId]);

  const handleSelect = (eventId) => {
    setSelectedEventIds((prevSelected) =>
      prevSelected.includes(eventId)
        ? prevSelected.filter((id) => id !== eventId)
        : [...prevSelected, eventId]
    );
  };

  const handleConfirm = () => {
    const selectedEvents = availableEvents.filter((e) =>
      selectedEventIds.includes(e.id)
    );
    onSelectEvents(selectedEvents);
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex justify-center items-center z-50">
      <div className="bg-white p-8 rounded-xl shadow-2xl w-full max-w-lg">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-blue-700">
            Seleccionar Sucesos
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 transition-colors"
          >
            <X className="w-6 h-6" />
          </button>
        </div>

        {/* Lista de eventos */}
        {loading ? (
          <div className="flex items-center justify-center py-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            <span className="ml-3 text-gray-600">Cargando sucesos...</span>
          </div>
        ) : (
          <div className="max-h-96 overflow-y-auto space-y-3 mb-6">
            {availableEvents.length === 0 ? (
              <div className="text-center py-8">
                <p className="text-gray-400">
                  No hay sucesos disponibles para esta área.
                </p>
                <p className="text-sm text-gray-500 mt-2">
                  Puedes crear sucesos manualmente en cada sección.
                </p>
              </div>
            ) : (
              availableEvents.map((event) => (
                <div
                  key={event.id}
                  className="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors cursor-pointer"
                  onClick={() => handleSelect(event.id)}
                >
                  <input
                    type="checkbox"
                    checked={selectedEventIds.includes(event.id)}
                    onChange={() => handleSelect(event.id)}
                    className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
                  />
                  <div className="flex-1">
                    <p className="font-semibold text-gray-900">{event.title}</p>
                    <p className="text-sm text-gray-500">
                      {new Date(event.eventDate).toLocaleDateString()}
                    </p>
                    {event.description && (
                      <p className="text-xs text-gray-600 mt-1">
                        {event.description}
                      </p>
                    )}
                  </div>
                </div>
              ))
            )}
          </div>
        )}

        {/* Botones */}
        <div className="flex justify-end space-x-4">
          <button
            onClick={onClose}
            className="px-6 py-2 text-gray-600 hover:text-gray-800 transition-colors"
          >
            Cancelar
          </button>
          <button
            onClick={handleConfirm}
            disabled={selectedEventIds.length === 0}
            className="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-md disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            Confirmar ({selectedEventIds.length})
          </button>
        </div>
      </div>
    </div>
  );
};

export default SelectEventsModal;

