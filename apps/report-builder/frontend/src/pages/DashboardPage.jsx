import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useQuery } from "react-query";
import {
  FileText,
  BarChart3,
  Brain,
  Plus,
  TrendingUp,
  Users,
} from "lucide-react";
import { templateService } from "../services/templateService";
import { reportService } from "../services/reportService";
import { useTenant } from "../contexts/TenantContext";

const DashboardPage = () => {
  const { tenantName } = useTenant();
  const [stats, setStats] = useState({
    totalTemplates: 0,
    totalReports: 0,
    pendingReports: 0,
    aiInsights: 0,
  });

  // Fetch templates
  const { data: templates, isLoading: templatesLoading } = useQuery(
    "templates",
    templateService.getTemplates
  );

  // Fetch reports
  const { data: reports, isLoading: reportsLoading } = useQuery(
    "reports",
    reportService.getReports
  );

  useEffect(() => {
    if (templates && reports) {
      setStats({
        totalTemplates: templates.length,
        totalReports: reports.length,
        pendingReports: reports.filter((r) => r.status === "draft").length,
        aiInsights: 0, // This would come from AI service
      });
    }
  }, [templates, reports]);

  const statCards = [
    {
      name: "Total Templates",
      value: stats.totalTemplates,
      icon: FileText,
      color: "bg-blue-500",
    },
    {
      name: "Total Reports",
      value: stats.totalReports,
      icon: BarChart3,
      color: "bg-green-500",
    },
    {
      name: "Pending Reports",
      value: stats.pendingReports,
      icon: TrendingUp,
      color: "bg-yellow-500",
    },
    {
      name: "AI Insights",
      value: stats.aiInsights,
      icon: Brain,
      color: "bg-purple-500",
    },
  ];

  const quickActions = [
    {
      name: "Create Template",
      description: "Design a new report template",
      href: "/templates/create",
      icon: Plus,
      color: "bg-primary-500",
    },
    {
      name: "View Reports",
      description: "Browse all reports",
      href: "/reports",
      icon: BarChart3,
      color: "bg-green-500",
    },
    {
      name: "AI Analysis",
      description: "Analyze data with AI",
      href: "/ai-analysis",
      icon: Brain,
      color: "bg-purple-500",
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600">
          Welcome to {tenantName}'s Report Builder
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {statCards.map((stat) => (
          <div key={stat.name} className="card">
            <div className="flex items-center">
              <div className={`p-3 rounded-lg ${stat.color}`}>
                <stat.icon className="h-6 w-6 text-white" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">{stat.name}</p>
                <p className="text-2xl font-bold text-gray-900">{stat.value}</p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Quick Actions */}
      <div>
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          Quick Actions
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {quickActions.map((action) => (
            <Link
              key={action.name}
              to={action.href}
              className="card hover:shadow-md transition-shadow cursor-pointer"
            >
              <div className="flex items-center">
                <div className={`p-3 rounded-lg ${action.color}`}>
                  <action.icon className="h-6 w-6 text-white" />
                </div>
                <div className="ml-4">
                  <h3 className="text-lg font-medium text-gray-900">
                    {action.name}
                  </h3>
                  <p className="text-sm text-gray-600">{action.description}</p>
                </div>
              </div>
            </Link>
          ))}
        </div>
      </div>

      {/* Recent Activity */}
      <div>
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          Recent Activity
        </h2>
        <div className="card">
          <div className="space-y-4">
            {reportsLoading ? (
              <div className="text-center py-8">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600 mx-auto"></div>
                <p className="text-gray-600 mt-2">Loading recent activity...</p>
              </div>
            ) : reports && reports.length > 0 ? (
              reports.slice(0, 5).map((report) => (
                <div
                  key={report.id}
                  className="flex items-center justify-between py-2 border-b border-gray-200 last:border-b-0"
                >
                  <div>
                    <p className="font-medium text-gray-900">{report.title}</p>
                    <p className="text-sm text-gray-600">
                      {report.status} •{" "}
                      {new Date(report.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                  <span
                    className={`px-2 py-1 text-xs font-medium rounded-full ${
                      report.status === "approved"
                        ? "bg-green-100 text-green-800"
                        : report.status === "pending"
                        ? "bg-yellow-100 text-yellow-800"
                        : "bg-gray-100 text-gray-800"
                    }`}
                  >
                    {report.status}
                  </span>
                </div>
              ))
            ) : (
              <p className="text-gray-600 text-center py-8">
                No recent activity
              </p>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
