import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from 'react-query';
import {
  FileText,
  BarChart3,
  Brain,
  Plus,
  TrendingUp,
  Users,
} from 'lucide-react';
import { templateService } from '../services/templateService';
import { reportService } from '../services/reportService';
import { useTenant } from '../contexts/TenantContext';

const DashboardPage = () => {
  const { tenantName } = useTenant();
  const [stats, setStats] = useState({
    totalTemplates: 0,
    totalReports: 0,
    pendingReports: 0,
    aiInsights: 0,
  });

  // Fetch templates
  const {
    data: templates,
    isLoading: templatesLoading,
    error: templatesError,
  } = useQuery('templates', templateService.getTemplates, {
    retry: false, // No reintentar en caso de error
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
    refetchOnWindowFocus: false,
    onError: error => {
      console.warn('Templates API error:', error.message);
    },
  });

  // Fetch reports
  const {
    data: reports,
    isLoading: reportsLoading,
    error: reportsError,
  } = useQuery('reports', reportService.getReports, {
    retry: false, // No reintentar en caso de error
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
    refetchOnWindowFocus: false,
    onError: error => {
      console.warn('Reports API error:', error.message);
    },
  });

  useEffect(() => {
    // Handle data even if APIs fail
    const templatesData = templates || [];
    const reportsData = reports || [];

    setStats({
      totalTemplates: templatesData.length,
      totalReports: reportsData.length,
      pendingReports: reportsData.filter(r => r.status === 'draft').length,
      aiInsights: 0, // This would come from AI service
    });
  }, [templates, reports]);

  const statCards = [
    {
      name: 'Total Templates',
      value: stats.totalTemplates,
      icon: FileText,
      color: 'bg-blue-500',
    },
    {
      name: 'Total Reports',
      value: stats.totalReports,
      icon: BarChart3,
      color: 'bg-green-500',
    },
    {
      name: 'Pending Reports',
      value: stats.pendingReports,
      icon: TrendingUp,
      color: 'bg-yellow-500',
    },
    {
      name: 'AI Insights',
      value: stats.aiInsights,
      icon: Brain,
      color: 'bg-purple-500',
    },
  ];

  const quickActions = [
    {
      name: 'Create Template',
      description: 'Design a new report template',
      href: '/templates/create',
      icon: Plus,
      color: 'bg-primary-500',
    },
    {
      name: 'View Reports',
      description: 'Browse all reports',
      href: '/reports',
      icon: BarChart3,
      color: 'bg-green-500',
    },
    {
      name: 'AI Analysis',
      description: 'Analyze data with AI',
      href: '/ai-analysis',
      icon: Brain,
      color: 'bg-purple-500',
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
        {/* Connection Status */}
        {(templatesError || reportsError) && (
          <div className="mt-2 p-3 bg-blue-50 border border-blue-200 rounded-md">
            <div className="flex">
              <div className="flex-shrink-0">
                <svg
                  className="h-5 w-5 text-blue-400"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fillRule="evenodd"
                    d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                    clipRule="evenodd"
                  />
                </svg>
              </div>
              <div className="ml-3">
                <p className="text-sm text-blue-800">
                  <strong>Dashboard Mode:</strong> You're logged in
                  successfully, but some backend services are not available. The
                  dashboard is working in offline mode with basic functionality.
                </p>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {statCards.map(stat => (
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
          {quickActions.map(action => (
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
            ) : reportsError ? (
              <div className="text-center py-8">
                <div className="text-gray-400 mb-2">
                  <svg
                    className="h-12 w-12 mx-auto"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                </div>
                <p className="text-gray-600">Unable to load recent activity</p>
                <p className="text-sm text-gray-500 mt-1">
                  Check your connection and try again
                </p>
              </div>
            ) : reports && reports.length > 0 ? (
              reports.slice(0, 5).map(report => (
                <div
                  key={report.id}
                  className="flex items-center justify-between py-2 border-b border-gray-200 last:border-b-0"
                >
                  <div>
                    <p className="font-medium text-gray-900">{report.title}</p>
                    <p className="text-sm text-gray-600">
                      {report.status} •{' '}
                      {new Date(report.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                  <span
                    className={`px-2 py-1 text-xs font-medium rounded-full ${
                      report.status === 'approved'
                        ? 'bg-green-100 text-green-800'
                        : report.status === 'pending'
                        ? 'bg-yellow-100 text-yellow-800'
                        : 'bg-gray-100 text-gray-800'
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
