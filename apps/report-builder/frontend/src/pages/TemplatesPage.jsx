import { useState } from "react";
import { Link } from "react-router-dom";
import { useQuery } from "react-query";
import { Plus, Edit, Trash2, Eye } from "lucide-react";
import { templateService } from "../services/templateService";
import { toast } from "react-toastify";

const TemplatesPage = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [filterType, setFilterType] = useState("all");

  const {
    data: templates,
    isLoading,
    refetch,
  } = useQuery("templates", templateService.getTemplates);

  const handleDelete = async (id) => {
    if (window.confirm("Are you sure you want to delete this template?")) {
      try {
        await templateService.deleteTemplate(id);
        toast.success("Template deleted successfully");
        refetch();
      } catch (error) {
        toast.error("Failed to delete template");
      }
    }
  };

  const filteredTemplates =
    templates?.filter((template) => {
      const matchesSearch = template.name
        .toLowerCase()
        .includes(searchTerm.toLowerCase());
      const matchesType = filterType === "all" || template.type === filterType;
      return matchesSearch && matchesType;
    }) || [];

  const templateTypes = [
    "all",
    "financial",
    "technical",
    "operational",
    "generic",
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Templates</h1>
          <p className="text-gray-600">Manage your report templates</p>
        </div>
        <Link
          to="/templates/create"
          className="btn btn-primary flex items-center"
        >
          <Plus className="h-4 w-4 mr-2" />
          Create Template
        </Link>
      </div>

      {/* Filters */}
      <div className="card">
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <input
              type="text"
              placeholder="Search templates..."
              className="input"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          <div className="sm:w-48">
            <select
              className="input"
              value={filterType}
              onChange={(e) => setFilterType(e.target.value)}
            >
              {templateTypes.map((type) => (
                <option key={type} value={type}>
                  {type === "all"
                    ? "All Types"
                    : type.charAt(0).toUpperCase() + type.slice(1)}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {/* Templates Grid */}
      {isLoading ? (
        <div className="text-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto"></div>
          <p className="text-gray-600 mt-4">Loading templates...</p>
        </div>
      ) : filteredTemplates.length > 0 ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredTemplates.map((template) => (
            <div
              key={template.id}
              className="card hover:shadow-md transition-shadow"
            >
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">
                    {template.name}
                  </h3>
                  <p className="text-sm text-gray-600">{template.type}</p>
                </div>
                <span className="px-2 py-1 text-xs font-medium bg-primary-100 text-primary-800 rounded-full">
                  v{template.version}
                </span>
              </div>

              <div className="space-y-2 mb-4">
                <p className="text-sm text-gray-600">
                  <strong>Area:</strong>{" "}
                  {template.areaName || "No area assigned"}
                </p>
                <p className="text-sm text-gray-600">
                  <strong>Created:</strong>{" "}
                  {new Date(template.createdAt).toLocaleDateString()}
                </p>
              </div>

              <div className="flex space-x-2">
                <Link
                  to={`/templates/${template.id}/edit`}
                  className="flex-1 btn btn-secondary flex items-center justify-center"
                >
                  <Edit className="h-4 w-4 mr-1" />
                  Edit
                </Link>
                <button
                  onClick={() => handleDelete(template.id)}
                  className="btn btn-danger flex items-center justify-center"
                >
                  <Trash2 className="h-4 w-4" />
                </button>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <div className="text-center py-12">
          <FileText className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">
            No templates found
          </h3>
          <p className="text-gray-600 mb-4">
            {searchTerm || filterType !== "all"
              ? "Try adjusting your search or filters"
              : "Get started by creating your first template"}
          </p>
          {!searchTerm && filterType === "all" && (
            <Link to="/templates/create" className="btn btn-primary">
              Create Template
            </Link>
          )}
        </div>
      )}
    </div>
  );
};

export default TemplatesPage;
