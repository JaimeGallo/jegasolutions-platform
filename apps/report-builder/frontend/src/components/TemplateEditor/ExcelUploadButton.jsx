import { useRef } from "react";
import { Upload } from "lucide-react";

const ExcelUploadButton = ({ sectionIndex, handleFileUpload }) => {
  const fileInputRef = useRef(null);

  const triggerFileInput = () => {
    fileInputRef.current.click();
  };

  const onFileChange = (event) => {
    handleFileUpload(sectionIndex, event);
  };

  return (
    <div className="mt-4">
      <input
        type="file"
        ref={fileInputRef}
        onChange={onFileChange}
        accept=".xlsx,.xls,.csv"
        style={{ display: "none" }}
      />
      <button
        onClick={triggerFileInput}
        className="w-full px-3 py-2 flex items-center justify-center bg-green-50 text-green-600 rounded-md hover:bg-green-100 border border-green-200 transition-colors"
      >
        <Upload className="h-4 w-4 mr-2" />
        Subir datos Excel
      </button>
    </div>
  );
};

export default ExcelUploadButton;

