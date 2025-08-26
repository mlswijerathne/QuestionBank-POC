'use client';
import { useAuth } from '../contexts/AuthContext';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

interface ProtectedRouteProps {
  children: React.ReactNode;
  allowedRoles: string[];
}

export default function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const { user, loading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!loading) {
      if (!user) {
        console.log('ProtectedRoute: No user found, redirecting to login');
        router.push('/login');
        return;
      }

      console.log('ProtectedRoute: User role:', user.role, 'Allowed roles:', allowedRoles);
      if (user.role && !allowedRoles.includes(user.role)) {
        console.log('ProtectedRoute: User does not have required role, redirecting to unauthorized');
        router.push('/unauthorized');
        return;
      }
    }
  }, [user, loading, router, allowedRoles]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-gray-900"></div>
      </div>
    );
  }

  if (!user || (user.role && !allowedRoles.includes(user.role))) {
    return null;
  }

  return <>{children}</>;
}
