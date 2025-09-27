import { useState } from "react";
import { Link } from "react-router-dom";
import { useQuery } from "react-query";
import {
  Plus,
  Eye,
  Edit,
  Trash2,
  CheckCircle,
  XCircle,
  Clock,
} from "lucide-react";
import { reportService } from "../services/reportService";
import { toast } from "react-toastify";

const ReportsPage = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("all");

  const {
    data: reports,
    isLoading,
    refetch,
  } = useQuery("reports", reportService.getReports);

  const handleDelete = async (id) => {
    if (window.confirm("Are you sure you want to delete this report?")) {
      try {
        await reportService.deleteReport(id);
        toast.success("Report deleted successfully");
        refetch();
      } catch (error) {
        toast.error("Failed to delete report");
      }
    }
  };

  const handleSubmit = async (id) => {
    try {
      await reportService.submitReport(id);
      toast.success("Report submitted successfully");
      refetch();
    } catch (error) {
      toast.error("Failed to submit report");
    }
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case "approved":
        return <CheckCircle className="h-4 w-4 text-green-500" />;
      case "rejected":
        return <XCircle className="h-4 w-4 text-red-500" />;
      default:
        return <Clock className="h-4 w-4 text-yellow-500" />;
    }
  };

  const getStatusColor = (status) => {
    switch (status) {
      case "approved":
        return "bg-green-100 text-green-800";
      case "rejected":
        return "bg-red-100 text-red-800";
      case "submitted":
        return "bg-blue-100 text-blue-800";
      default:
        return "bg-yellow-100 text-yellow-800";
    }
  };

  const filteredReports =
    reports?.filter((report) => {
      const matchesSearch = report.title
        .toLowerCase()
        .includes(searchTerm.toLowerCase());
      const matchesStatus =
        statusFilter === "all" || report.status === statusFilter;
      return matchesSearch && matchesStatus;
    }) || [];

  const statusOptions = [
    { value: "all", label: "All Status" },
    { value: "draft", label: "Draft" },
    { value: "submitted", label: "Submitted" },
    { value: "approved", label: "Approved" },
    { value: "rejected", label: "Rejected" },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Reports</h1>
          <p className="text-gray-600">Manage your report submissions</p>
        </div>
        <Link
          to="/reports/create"
          className="btn btn-primary flex items-center"
        >
          <Plus className="h-4 w-4 mr-2" />
          Create Report
        </Link>
      </div>

      {/* Filters */}
      <div className="card">
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <input
              type="text"
              placeholder="Search reports..."
              className="input"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          <div className="sm:w-48">
            <select
              className="input"
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
            >
              {statusOptions.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {/* Reports List */}
      {isLoading ? (
        <div className="text-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto"></div>
          <p className="text-gray-600 mt-4">Loading reports...</p>
        </div>
      ) : filteredReports.length > 0 ? (
        <div className="space-y-4">
          {filteredReports.map((report) => (
            <div key={report.id} className="card">
              <div className="flex items-center justify-between">
                <div className="flex-1">
                  <div className="flex items-center space-x-3">
                    {getStatusIcon(report.status)}
                    <div>
                      <h3 className="text-lg font-semibold text-gray-900">
                        {report.title}
                      </h3>
                      <p className="text-sm text-gray-600">
                        {report.templateName} • {report.areaName}
                      </p>
                    </div>
                  </div>
                </div>

                <div className="flex items-center space-x-4">
                  <span
                    className={`px-3 py-1 text-sm font-medium rounded-full ${getStatusColor(
                      report.status
                    )}`}
                  >
                    {report.status}
                  </span>

                  <div className="flex space-x-2">
                    <Link
                      to={`/reports/${report.id}`}
                      className="btn btn-secondary flex items-center"
                    >
                      <Eye className="h-4 w-4 mr-1" />
                      View
                    </Link>

                    {report.status === "draft" && (
                      <>
                        <Link
                          to={`/reports/${report.id}/edit`}
                          className="btn btn-secondary flex items-center"
                        >
                          <Edit className="h-4 w-4 mr-1" />
                          Edit
                        </Link>
                        <button
                          onClick={() => handleSubmit(report.id)}
                          className="btn btn-primary"
                        >
                          Submit
                        </button>
                      </>
                    )}

                    <button
                      onClick={() => handleDelete(report.id)}
                      className="btn btn-danger flex items-center"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  </div>
                </div>
              </div>

              <div className="mt-4 text-sm text-gray-600">
                <p>
                  <strong>Period:</strong>{" "}
                  {new Date(report.periodStart).toLocaleDateString()} -{" "}
                  {new Date(report.periodEnd).toLocaleDateString()}
                </p>
                <p>
                  <strong>Created:</strong>{" "}
                  {new Date(report.createdAt).toLocaleDateString()}
                </p>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <div className="text-center py-12">
          <BarChart3 className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">
            No reports found
          </h3>
          <p className="text-gray-600 mb-4">
            {searchTerm || statusFilter !== "all"
              ? "Try adjusting your search or filters"
              : "Get started by creating your first report"}
          </p>
          {!searchTerm && statusFilter === "all" && (
            <Link to="/reports/create" className="btn btn-primary">
              Create Report
            </Link>
          )}
        </div>
      )}
    </div>
  );
};

export default ReportsPage;
