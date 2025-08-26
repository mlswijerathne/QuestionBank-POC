'use client';
import { useState, useEffect } from 'react';
import ProtectedRoute from '../components/ProtectedRoute';
import { useAuth } from '../contexts/AuthContext';
import { api } from '../services/api';

export default function EvaluatorPage() {
  const [dashboardData, setDashboardData] = useState<any>(null);
  const { signOut } = useAuth();

  useEffect(() => {
    const loadDashboard = async () => {
      try {
        const data = await api.getEvaluatorDashboard();
        setDashboardData(data);
      } catch (err) {
        console.error('Failed to load dashboard:', err);
      }
    };
    loadDashboard();
  }, []);

  return (
    <ProtectedRoute allowedRoles={['evaluator', 'admin']}>
      <div className="min-h-screen bg-gray-100">
        <nav className="bg-white shadow">
          <div className="max-w-7xl mx-auto px-4">
            <div className="flex justify-between items-center py-6">
              <h1 className="text-xl font-semibold text-gray-900">Evaluator Dashboard</h1>
              <button
                onClick={signOut}
                className="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
              >
                Sign Out
              </button>
            </div>
          </div>
        </nav>

        <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
          <div className="px-4 py-6 sm:px-0">
            {dashboardData && (
              <div className="bg-white overflow-hidden shadow rounded-lg">
                <div className="px-4 py-5 sm:p-6">
                  <h2 className="text-lg font-medium text-gray-900 mb-4">
                    {dashboardData.message}
                  </h2>
                  <p className="text-gray-600">Role: {dashboardData.role}</p>
                  <div className="mt-4">
                    <h3 className="font-medium text-gray-900">Available Features:</h3>
                    <ul className="mt-2 list-disc list-inside">
                      {dashboardData.features?.map((feature: string, index: number) => (
                        <li key={index} className="text-gray-600">{feature}</li>
                      ))}
                    </ul>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
