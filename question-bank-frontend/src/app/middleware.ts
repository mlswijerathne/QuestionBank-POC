import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

// This middleware redirects users based on their authentication status
export function middleware(request: NextRequest) {
  // Extract the path from the URL
  const path = request.nextUrl.pathname

  // Extract the token from the URL if it's in the format /invitation/TOKEN
  if (path.startsWith('/invitation/')) {
    const token = path.replace('/invitation/', '')
    if (token && token.length > 0) {
      // Redirect to the invite page with the token as a query parameter
      const url = new URL('/invite', request.url)
      url.searchParams.set('token', token)
      return NextResponse.redirect(url)
    }
  }

  return NextResponse.next()
}

// Configure the middleware to run on specific paths
export const config = {
  matcher: ['/invitation/:token*'],
}
