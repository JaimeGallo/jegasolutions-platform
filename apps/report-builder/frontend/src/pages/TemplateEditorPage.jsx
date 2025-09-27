import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useQuery } from "react-query";
import { Save, Eye, Settings } from "lucide-react";
import { templateService } from "../services/templateService";
import { toast } from "react-toastify";

const TemplateEditorPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditing = !!id;

  const [templateData, setTemplateData] = useState({
    name: "",
    areaId: null,
    configuration: {
      metadata: {
        templateType: "generic",
        description: "",
        version: "1.0.0",
        defaultPeriod: "monthly",
        allowedPeriods: ["monthly"],
      },
      sections: [],
      header: {
        titleFormat: "{TemplateName} - {Period}",
        showLogo: true,
        metadataFields: [],
      },
      footer: {
        content: "© {Year} Company Name",
        showPageNumbers: true,
      },
      settings: {
        exportFormats: ["pdf"],
        colorScheme: "default",
      },
    },
  });

  const { data: template, isLoading } = useQuery(
    ["template", id],
    () => templateService.getTemplate(id),
    { enabled: isEditing }
  );

  useEffect(() => {
    if (template) {
      setTemplateData({
        name: template.name,
        areaId: template.areaId,
        configuration: template.configuration,
      });
    }
  }, [template]);

  const handleSave = async () => {
    try {
      if (isEditing) {
        await templateService.updateTemplate(id, templateData);
        toast.success("Template updated successfully");
      } else {
        await templateService.createTemplate(templateData);
        toast.success("Template created successfully");
      }
      navigate("/templates");
    } catch (error) {
      toast.error("Failed to save template");
    }
  };

  const addSection = () => {
    const newSection = {
      sectionId: Date.now().toString(),
      title: "",
      type: "text",
      order: templateData.configuration.sections.length + 1,
      required: true,
      components: [],
      layout: "full-width",
    };

    setTemplateData((prev) => ({
      ...prev,
      configuration: {
        ...prev.configuration,
        sections: [...prev.configuration.sections, newSection],
      },
    }));
  };

  const updateSection = (index, field, value) => {
    setTemplateData((prev) => ({
      ...prev,
      configuration: {
        ...prev.configuration,
        sections: prev.configuration.sections.map((section, i) =>
          i === index ? { ...section, [field]: value } : section
        ),
      },
    }));
  };

  const removeSection = (index) => {
    setTemplateData((prev) => ({
      ...prev,
      configuration: {
        ...prev.configuration,
        sections: prev.configuration.sections.filter((_, i) => i !== index),
      },
    }));
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditing ? "Edit Template" : "Create Template"}
          </h1>
          <p className="text-gray-600">
            {isEditing
              ? "Modify your template configuration"
              : "Design a new report template"}
          </p>
        </div>
        <div className="flex space-x-3">
          <button className="btn btn-secondary flex items-center">
            <Eye className="h-4 w-4 mr-2" />
            Preview
          </button>
          <button
            onClick={handleSave}
            className="btn btn-primary flex items-center"
          >
            <Save className="h-4 w-4 mr-2" />
            Save Template
          </button>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Template Configuration */}
        <div className="lg:col-span-2 space-y-6">
          <div className="card">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Basic Information
            </h3>
            <div className="space-y-4">
              <div>
                <label className="label">Template Name</label>
                <input
                  type="text"
                  className="input"
                  value={templateData.name}
                  onChange={(e) =>
                    setTemplateData((prev) => ({
                      ...prev,
                      name: e.target.value,
                    }))
                  }
                  placeholder="Enter template name"
                />
              </div>
              <div>
                <label className="label">Description</label>
                <textarea
                  className="input"
                  rows="3"
                  value={templateData.configuration.metadata.description}
                  onChange={(e) =>
                    setTemplateData((prev) => ({
                      ...prev,
                      configuration: {
                        ...prev.configuration,
                        metadata: {
                          ...prev.configuration.metadata,
                          description: e.target.value,
                        },
                      },
                    }))
                  }
                  placeholder="Describe the purpose of this template"
                />
              </div>
            </div>
          </div>

          {/* Sections */}
          <div className="card">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-lg font-semibold text-gray-900">
                Report Sections
              </h3>
              <button onClick={addSection} className="btn btn-primary">
                Add Section
              </button>
            </div>

            <div className="space-y-4">
              {templateData.configuration.sections.map((section, index) => (
                <div
                  key={section.sectionId}
                  className="border border-gray-200 rounded-lg p-4"
                >
                  <div className="flex justify-between items-center mb-3">
                    <h4 className="font-medium text-gray-900">
                      Section {index + 1}
                    </h4>
                    <button
                      onClick={() => removeSection(index)}
                      className="text-red-600 hover:text-red-800"
                    >
                      Remove
                    </button>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="label">Title</label>
                      <input
                        type="text"
                        className="input"
                        value={section.title}
                        onChange={(e) =>
                          updateSection(index, "title", e.target.value)
                        }
                        placeholder="Section title"
                      />
                    </div>
                    <div>
                      <label className="label">Type</label>
                      <select
                        className="input"
                        value={section.type}
                        onChange={(e) =>
                          updateSection(index, "type", e.target.value)
                        }
                      >
                        <option value="text">Text</option>
                        <option value="chart">Chart</option>
                        <option value="table">Table</option>
                        <option value="image">Image</option>
                      </select>
                    </div>
                    <div>
                      <label className="label">Order</label>
                      <input
                        type="number"
                        className="input"
                        value={section.order}
                        onChange={(e) =>
                          updateSection(
                            index,
                            "order",
                            parseInt(e.target.value)
                          )
                        }
                      />
                    </div>
                    <div className="flex items-center">
                      <input
                        type="checkbox"
                        id={`required-${index}`}
                        checked={section.required}
                        onChange={(e) =>
                          updateSection(index, "required", e.target.checked)
                        }
                        className="mr-2"
                      />
                      <label
                        htmlFor={`required-${index}`}
                        className="text-sm text-gray-700"
                      >
                        Required section
                      </label>
                    </div>
                  </div>
                </div>
              ))}

              {templateData.configuration.sections.length === 0 && (
                <div className="text-center py-8 text-gray-500">
                  <p>
                    No sections added yet. Click "Add Section" to get started.
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Settings Panel */}
        <div className="space-y-6">
          <div className="card">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Template Settings
            </h3>
            <div className="space-y-4">
              <div>
                <label className="label">Template Type</label>
                <select
                  className="input"
                  value={templateData.configuration.metadata.templateType}
                  onChange={(e) =>
                    setTemplateData((prev) => ({
                      ...prev,
                      configuration: {
                        ...prev.configuration,
                        metadata: {
                          ...prev.configuration.metadata,
                          templateType: e.target.value,
                        },
                      },
                    }))
                  }
                >
                  <option value="generic">Generic</option>
                  <option value="financial">Financial</option>
                  <option value="technical">Technical</option>
                  <option value="operational">Operational</option>
                </select>
              </div>

              <div>
                <label className="label">Export Formats</label>
                <div className="space-y-2">
                  {["pdf", "excel", "csv"].map((format) => (
                    <label key={format} className="flex items-center">
                      <input
                        type="checkbox"
                        checked={templateData.configuration.settings.exportFormats.includes(
                          format
                        )}
                        onChange={(e) => {
                          const formats =
                            templateData.configuration.settings.exportFormats;
                          if (e.target.checked) {
                            setTemplateData((prev) => ({
                              ...prev,
                              configuration: {
                                ...prev.configuration,
                                settings: {
                                  ...prev.configuration.settings,
                                  exportFormats: [...formats, format],
                                },
                              },
                            }));
                          } else {
                            setTemplateData((prev) => ({
                              ...prev,
                              configuration: {
                                ...prev.configuration,
                                settings: {
                                  ...prev.configuration.settings,
                                  exportFormats: formats.filter(
                                    (f) => f !== format
                                  ),
                                },
                              },
                            }));
                          }
                        }}
                        className="mr-2"
                      />
                      <span className="text-sm text-gray-700">
                        {format.toUpperCase()}
                      </span>
                    </label>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TemplateEditorPage;
