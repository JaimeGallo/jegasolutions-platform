import { Eye } from "lucide-react";

const PreviewButton = ({ onClick, disabled }) => {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      className={`
        flex items-center px-4 py-2 rounded-md shadow text-sm font-medium transition-colors
        ${
          disabled
            ? "bg-gray-300 text-gray-500 cursor-not-allowed"
            : "bg-green-600 text-white hover:bg-green-700"
        }
      `}
    >
      <Eye className="h-5 w-5 mr-2" />
      Vista Previa
    </button>
  );
};

export default PreviewButton;

