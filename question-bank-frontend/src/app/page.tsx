import Image from "next/image";
import Link from "next/link";

export default function Home() {
  return (
    <div className="font-sans flex flex-col min-h-screen">
      {/* Hero Section */}
      <main className="flex-grow flex flex-col items-center justify-center p-8 text-center">
        <div className="max-w-4xl mx-auto">
          <h1 className="text-4xl md:text-6xl font-bold mb-6">
            Question Bank Platform
          </h1>
          <p className="text-xl md:text-2xl mb-8 text-gray-600 dark:text-gray-300">
            Streamline your assessment creation process with our comprehensive question management system
          </p>
          
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              href="/login"
              className="rounded-md bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 font-medium text-lg transition-colors"
            >
              Sign In
            </Link>
            <Link
              href="/register"
              className="rounded-md border border-gray-300 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800 px-6 py-3 font-medium text-lg transition-colors"
            >
              Register
            </Link>
          </div>
        </div>
        
        {/* Feature Highlights */}
        <div className="mt-20 grid grid-cols-1 md:grid-cols-3 gap-8 w-full max-w-5xl">
          <div className="p-6 border border-gray-200 dark:border-gray-800 rounded-lg">
            <h3 className="font-bold text-xl mb-3">Create Question Banks</h3>
            <p className="text-gray-600 dark:text-gray-400">
              Build and organize question collections for various assessment needs
            </p>
          </div>
          
          <div className="p-6 border border-gray-200 dark:border-gray-800 rounded-lg">
            <h3 className="font-bold text-xl mb-3">Evaluate Candidates</h3>
            <p className="text-gray-600 dark:text-gray-400">
              Streamline your evaluation process with customizable assessments
            </p>
          </div>
          
          <div className="p-6 border border-gray-200 dark:border-gray-800 rounded-lg">
            <h3 className="font-bold text-xl mb-3">Analyze Results</h3>
            <p className="text-gray-600 dark:text-gray-400">
              Gain insights from assessment results with detailed analytics
            </p>
          </div>
        </div>
      </main>
      
      {/* Footer */}
      <footer className="py-8 border-t border-gray-200 dark:border-gray-800">
        <div className="container mx-auto text-center">
          <p className="text-gray-600 dark:text-gray-400">
            &copy; {new Date().getFullYear()} Question Bank Platform. All rights reserved.
          </p>
        </div>
      </footer>
    </div>
  );
}
