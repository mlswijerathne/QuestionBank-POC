'use client';
import { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { auth } from '../lib/firebase';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const { signInWithEmail, signInWithGoogle } = useAuth();
  const router = useRouter();

  const handleEmailLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      console.log('Attempting sign in...');
      await signInWithEmail(email, password);
      console.log('Sign in successful, waiting for user data...');
      
      // Wait a moment for Firebase to update auth state
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      if (!auth.currentUser) {
        console.error('User not available after login');
        throw new Error('Authentication failed');
      }
      
      // Force token refresh to get the latest claims
      await auth.currentUser.getIdToken(true);
      
      // Get the user role and redirect accordingly
      const idTokenResult = await auth.currentUser.getIdTokenResult();
      console.log('Token claims:', idTokenResult?.claims);
      const role = idTokenResult?.claims?.role as string;
      console.log('Detected role:', role);
      
      // Cancel any existing navigation to dashboard
      if (window.location.pathname === '/dashboard') {
        window.history.replaceState({}, '', '/');
      }
      
      if (role === 'admin') {
        console.log('Redirecting to admin page');
        router.replace('/admin');
      } else if (role === 'evaluator') {
        console.log('Redirecting to evaluator page');
        router.replace('/evaluator');
      } else if (role) {
        console.log('Redirecting to shared page');
        router.replace('/shared');
      } else {
        console.log('No role found, redirecting to homepage');
        // Default route for users without a specific role
        router.replace('/');
      }
    } catch (err: any) {
      console.error('Login error:', err);
      // If Firebase error, show its code/message for debugging
      const message = err?.code ? `${err.code}: ${err.message}` : (err?.message || 'Failed to sign in');
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  const handleGoogleLogin = async () => {
    setLoading(true);
    setError('');

    try {
      await signInWithGoogle();
      console.log('Google sign in successful, waiting for user data...');
      
      // Wait a moment for Firebase to update auth state
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      if (!auth.currentUser) {
        console.error('User not available after Google login');
        throw new Error('Authentication failed');
      }
      
      // Force token refresh to get the latest claims
      await auth.currentUser.getIdToken(true);
      
      // Get the user role and redirect accordingly
      const idTokenResult = await auth.currentUser.getIdTokenResult();
      console.log('Token claims:', idTokenResult?.claims);
      const role = idTokenResult?.claims?.role as string;
      console.log('Detected role:', role);
      
      // Cancel any existing navigation to dashboard
      if (window.location.pathname === '/dashboard') {
        window.history.replaceState({}, '', '/');
      }
      
      if (role === 'admin') {
        console.log('Redirecting to admin page');
        router.replace('/admin');
      } else if (role === 'evaluator') {
        console.log('Redirecting to evaluator page');
        router.replace('/evaluator');
      } else if (role) {
        console.log('Redirecting to shared page');
        router.replace('/shared');
      } else {
        console.log('No role found, redirecting to homepage');
        // Default route for users without a specific role
        router.replace('/');
      }
    } catch (err: any) {
      console.error('Google login error:', err);
      const message = err?.code ? `${err.code}: ${err.message}` : (err?.message || 'Failed to sign in with Google');
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Sign in to your account
        </h2>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <form className="space-y-6" onSubmit={handleEmailLogin}>
            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                Email address
              </label>
              <div className="mt-1">
                <input
                  id="email"
                  name="email"
                  type="email"
                  required
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                />
              </div>
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                Password
              </label>
              <div className="mt-1">
                <input
                  id="password"
                  name="password"
                  type="password"
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                />
              </div>
            </div>

            <div>
              <button
                type="submit"
                disabled={loading}
                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
              >
                {loading ? 'Signing in...' : 'Sign in'}
              </button>
            </div>
          </form>

          <div className="mt-6">
            <div className="relative">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-gray-300" />
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-2 bg-white text-gray-500">Or continue with</span>
              </div>
            </div>

            <div className="mt-6">
              <button
                onClick={handleGoogleLogin}
                disabled={loading}
                className="w-full inline-flex justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
              >
                <svg className="w-5 h-5" viewBox="0 0 24 24">
                  <path fill="currentColor" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/>
                  <path fill="currentColor" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
                  <path fill="currentColor" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
                  <path fill="currentColor" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
                </svg>
                <span className="ml-2">Sign in with Google</span>
              </button>
            </div>
          </div>

          <div className="mt-6 text-center">
            <Link href="/register" className="text-indigo-600 hover:text-indigo-500">
              Don't have an account? Register here
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
