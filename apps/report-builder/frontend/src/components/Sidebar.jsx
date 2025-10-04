import { NavLink } from "react-router-dom";
import {
  Home,
  FileText,
  BarChart3,
  Brain,
  Settings,
  LogOut,
  FolderKanban,
  CheckSquare,
  Upload,
  Layers,
} from "lucide-react";
import { useAuth } from "../contexts/AuthContext";
import { useTenant } from "../contexts/TenantContext";

const Sidebar = () => {
  const { logout } = useAuth();
  const { tenantName } = useTenant();

  const navigation = [
    { name: "Dashboard", href: "/", icon: Home },
    { name: "Templates", href: "/templates", icon: FileText },
    { name: "Hybrid Builder", href: "/hybrid-builder", icon: Layers },
    { name: "Reports", href: "/reports", icon: BarChart3 },
    { name: "Consolidated", href: "/consolidated-templates", icon: FolderKanban },
    { name: "My Tasks", href: "/my-tasks", icon: CheckSquare },
    { name: "Excel Upload", href: "/excel-uploads", icon: Upload },
    { name: "AI Analysis", href: "/ai-analysis", icon: Brain },
  ];

  return (
    <div className="w-64 bg-white shadow-sm border-r border-gray-200 flex flex-col">
      {/* Logo and Tenant Info */}
      <div className="p-6 border-b border-gray-200">
        <h1 className="text-xl font-bold text-gray-900">Report Builder</h1>
        <p className="text-sm text-gray-600 mt-1">{tenantName}</p>
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-4 py-6 space-y-2">
        {navigation.map((item) => (
          <NavLink
            key={item.name}
            to={item.href}
            className={({ isActive }) =>
              `flex items-center px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                isActive
                  ? "bg-primary-100 text-primary-700"
                  : "text-gray-700 hover:bg-gray-100"
              }`
            }
          >
            <item.icon className="mr-3 h-5 w-5" />
            {item.name}
          </NavLink>
        ))}
      </nav>

      {/* Settings and Logout */}
      <div className="p-4 border-t border-gray-200 space-y-2">
        <button className="flex items-center w-full px-3 py-2 text-sm font-medium text-gray-700 rounded-lg hover:bg-gray-100">
          <Settings className="mr-3 h-5 w-5" />
          Settings
        </button>
        <button
          onClick={logout}
          className="flex items-center w-full px-3 py-2 text-sm font-medium text-red-700 rounded-lg hover:bg-red-50"
        >
          <LogOut className="mr-3 h-5 w-5" />
          Logout
        </button>
      </div>
    </div>
  );
};

export default Sidebar;
