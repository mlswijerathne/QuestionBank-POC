'use client';
import { useState, useEffect } from 'react';
import ProtectedRoute from '../components/ProtectedRoute';
import { useAuth } from '../contexts/AuthContext';
import { api } from '../services/api';
import { auth } from '../lib/firebase';

export default function AdminPage() {
  const [dashboardData, setDashboardData] = useState<any>(null);
  const [inviteForm, setInviteForm] = useState({ email: '', role: 'evaluator' });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const { user, signOut } = useAuth();

  useEffect(() => {
    const loadDashboard = async () => {
      try {
        // Wait for user to be loaded before making API call
        if (!user) return;
        
        // Force refresh token to make sure we have the latest claims
        const token = await auth.currentUser?.getIdToken(true);
        const tokenResult = await auth.currentUser?.getIdTokenResult();
        
        // Debug output to check claims
        console.log("Token claims:", tokenResult?.claims);
        
        // First call the debug endpoint to check token and authentication
        console.log("Calling debug endpoint to check token...");
        try {
          const debugInfo = await api.debugToken();
          console.log("Debug token info:", debugInfo);
        } catch (debugError) {
          console.error("Debug token endpoint error:", debugError);
        }
        
        // Now try to get the actual dashboard data
        const data = await api.getAdminDashboard();
        setDashboardData(data);
      } catch (err) {
        console.error('Failed to load dashboard:', err);
      }
    };
    
    if (user) {
      loadDashboard();
    }
  }, [user]);

  const handleInvite = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage('');

    try {
      const result = await api.createInvitation(inviteForm);
      setMessage(`Invitation sent! Token: ${result.invitationToken}`);
      setInviteForm({ email: '', role: 'evaluator' });
    } catch (err: any) {
      setMessage(`Error: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <ProtectedRoute allowedRoles={['admin']}>
      <div className="min-h-screen bg-gray-100">
        <nav className="bg-white shadow">
          <div className="max-w-7xl mx-auto px-4">
            <div className="flex justify-between items-center py-6">
              <h1 className="text-xl font-semibold text-gray-900">Admin Dashboard</h1>
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
              <div className="bg-white overflow-hidden shadow rounded-lg mb-6">
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

            <div className="bg-white overflow-hidden shadow rounded-lg">
              <div className="px-4 py-5 sm:p-6">
                <h2 className="text-lg font-medium text-gray-900 mb-4">
                  Invite Users
                </h2>

                {message && (
                  <div className={`mb-4 p-4 rounded ${message.startsWith('Error') ? 'bg-red-50 text-red-700' : 'bg-green-50 text-green-700'}`}>
                    {message}
                  </div>
                )}

                <form onSubmit={handleInvite} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Email
                    </label>
                    <input
                      type="email"
                      required
                      value={inviteForm.email}
                      onChange={(e) => setInviteForm(prev => ({ ...prev, email: e.target.value }))}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Role
                    </label>
                    <select
                      value={inviteForm.role}
                      onChange={(e) => setInviteForm(prev => ({ ...prev, role: e.target.value }))}
                      className="mt-1 block w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                    >
                      <option value="evaluator">Evaluator</option>
                      <option value="candidate">Candidate</option>
                    </select>
                  </div>

                  <button
                    type="submit"
                    disabled={loading}
                    className="bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700 disabled:opacity-50"
                  >
                    {loading ? 'Sending...' : 'Send Invitation'}
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
