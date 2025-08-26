import { auth } from '../lib/firebase';

// Provide a fallback URL if the environment variable is not set
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5122/api';

async function apiCall(endpoint: string, options: RequestInit = {}) {
  // Force refresh token to ensure we have the latest claims
  let token = null;
  let tokenData = null;
  
  try {
    if (auth.currentUser) {
      // Force token refresh to ensure we have the latest claims
      token = await auth.currentUser.getIdToken(true);
      
      // Get decoded token data
      const tokenResult = await auth.currentUser.getIdTokenResult();
      console.log('Token claims:', tokenResult.claims);
      
      // Show the token we're using
      console.log('Using token:', token.substring(0, 10) + '...');
      
      // Check if the role claim exists
      if (tokenResult.claims.role) {
        console.log(`User role from token: ${tokenResult.claims.role}`);
      } else {
        console.warn('No role claim found in token');
      }
      
      tokenData = tokenResult;
    } else {
      console.warn('No authenticated user found when making API call');
    }
  } catch (error) {
    console.error('Error getting auth token:', error);
  }
  
  const config: RequestInit = {
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
    ...options,
  };

  console.log(`Making API request to ${API_BASE_URL}${endpoint}`, {
    method: options.method || 'GET',
    hasAuthToken: !!token,
    userRole: tokenData?.claims.role || 'none'
  });

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
    
    // Log response details for debugging
    console.log(`API response from ${endpoint}:`, {
      status: response.status,
      statusText: response.statusText,
      ok: response.ok,
      headers: Object.fromEntries(response.headers.entries())
    });
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: `Server error: ${response.status} ${response.statusText}` }));
      throw new Error(errorData.message || `API call failed with status: ${response.status}`);
    }
    
    return response.json();
  } catch (error) {
    console.error(`API call to ${API_BASE_URL}${endpoint} failed:`, error);
    throw error;
  }
}

export const api = {
  registerCompany: async (data: {
    companyName: string;
    description?: string;
    adminEmail: string;
    idToken: string;
  }) => {
    try {
      console.log('Calling company registration endpoint...');
      const result = await apiCall('/company/register', {
        method: 'POST',
        body: JSON.stringify(data),
      });
      console.log('Company registration API response:', result);
      return result;
    } catch (error) {
      console.error('Company registration API error:', error);
      throw error;
    }
  },

  createInvitation: async (data: {
    email: string;
    role: string;
  }) => {
    return apiCall('/invitation/create', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  verifyInvitation: async (token: string) => {
    return apiCall(`/invitation/verify/${token}`);
  },

  acceptInvitation: async (data: {
    token: string;
    idToken: string;
    fullName: string;
  }) => {
    return apiCall('/invitation/accept', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  getProfile: async () => {
    return apiCall('/company/profile');
  },

  getAdminDashboard: async () => {
    return apiCall('/admin/dashboard');
  },

  getEvaluatorDashboard: async () => {
    return apiCall('/evaluator/dashboard');
  },

  getSharedDashboard: async () => {
    return apiCall('/shared/dashboard');
  },
  
  debugToken: async () => {
    return apiCall('/debug/token');
  }
};
