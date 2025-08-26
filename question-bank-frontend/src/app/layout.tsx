import { AuthProvider } from './contexts/AuthContext';
import './globals.css';
import Link from 'next/link';

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className="min-h-screen bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100">
        <AuthProvider>
          <div className="flex flex-col min-h-screen">
            <header className="bg-white dark:bg-gray-800 shadow">
              <nav className="container mx-auto px-4 py-4 flex justify-between items-center">
                <Link href="/" className="font-bold text-xl">
                  Question Bank
                </Link>
                <div className="flex gap-4">
                  <Link href="/login" className="hover:underline">
                    Login
                  </Link>
                  <Link href="/register" className="hover:underline">
                    Register
                  </Link>
                </div>
              </nav>
            </header>
            <div className="flex-grow">
              {children}
            </div>
          </div>
        </AuthProvider>
      </body>
    </html>
  );
}
