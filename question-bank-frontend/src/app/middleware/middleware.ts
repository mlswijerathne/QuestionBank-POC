// middleware.ts
import { NextRequest, NextResponse } from 'next/server';

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  
  // Define protected routes
  const protectedRoutes = {
    '/admin': ['admin'],
    '/evaluator': ['admin', 'evaluator'],
    '/shared': ['admin', 'evaluator', 'candidate']
  };

  // Handle invitation tokens
  if (pathname.startsWith('/invitation/')) {
    const token = pathname.replace('/invitation/', '');
    if (token && token.length > 0) {
      // Redirect to the invite page with the token as a query parameter
      const url = new URL('/invite', request.url);
      url.searchParams.set('token', token);
      return NextResponse.redirect(url);
    }
  }

  // Check if the current path starts with any protected route
  const matchedRoute = Object.keys(protectedRoutes).find(route => 
    pathname.startsWith(route)
  );

  if (matchedRoute) {
    // In a real implementation, you'd verify the JWT token here
    // For now, we'll let the client-side handle the role checking
    const response = NextResponse.next();
    response.headers.set('x-protected-route', 'true');
    return response;
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    '/((?!api|_next/static|_next/image|favicon.ico).*)',
    '/invitation/:token*'
  ],
};
