import { useEffect, useState } from "react";
import PropTypes from "prop-types";
import axios from "axios";
import ConfigContext from "./ConfigContext";
import { API_CONFIG } from "../environments/api.config";

export const ConfigProvider = ({ children }) => {
  const [config, setConfig] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchConfig = async () => {
      try {
        const token = localStorage.getItem("token");
        if (!token) {
          throw new Error("No token found in localStorage.");
        }

        const response = await axios.get(`${API_CONFIG.BASE_URL}/api/config`, {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });

        const data = response.data;
        console.log("Raw data from API:", data);
        console.log(
          "weeklyExtraHoursLimit from API:",
          data.weeklyExtraHoursLimit
        );
        console.log(
          "Type of weeklyExtraHoursLimit:",
          typeof data.weeklyExtraHoursLimit
        );

        const transformedData = {
          weeklyExtraHoursLimit: data.weeklyExtraHoursLimit,
          diurnalMultiplier: data.diurnalMultiplier,
          nocturnalMultiplier: data.nocturnalMultiplier,
          diurnalHolidayMultiplier: data.diurnalHolidayMultiplier,
          nocturnalHolidayMultiplier: data.nocturnalHolidayMultiplier,
          diurnalStart: data.diurnalStart,
          diurnalEnd: data.diurnalEnd,
        };

        console.log("Datos transformados:", transformedData);
        console.log(
          "weeklyExtraHoursLimit transformed:",
          transformedData.weeklyExtraHoursLimit
        );
        setConfig(transformedData);
      } catch (error) {
        console.error("Error fetching configuration:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchConfig();
  }, []);

  if (isLoading) {
    return <div>Loading configuration...</div>;
  }

  return (
    <ConfigContext.Provider value={{ config, setConfig }}>
      {children}
    </ConfigContext.Provider>
  );
};

ConfigProvider.propTypes = {
  children: PropTypes.node.isRequired,
};
