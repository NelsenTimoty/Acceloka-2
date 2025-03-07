"use client";
import { Geist, Geist_Mono } from "next/font/google";
import { Link } from "react-scroll"; // Import react-scroll
import { Toaster } from "react-hot-toast";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body className={`${geistSans.variable} ${geistMono.variable} antialiased`}>
        <div>
          {/* Toast Notification System */}
          <Toaster position="top-right" reverseOrder={false} />
          {/* Navbar with scroll links */}
          <nav className="bg-black text-white p-4 fixed top-0 left-0 w-full shadow-md z-50">
            <div className="container mx-auto flex justify-between">
              <h3>Created By Nelsen Timoty</h3>
              <div className="space-x-4">
                <Link to="tickets" smooth={true} duration={500} className="cursor-pointer hover:text-gray-300">
                  Tickets
                </Link>
                <Link to="booked-tickets" smooth={true} duration={500} className="cursor-pointer hover:text-gray-300">
                  Booked Tickets
                </Link>
              </div>
            </div>
          </nav>

          {/* Page content */}
          <main className="p-6 pt-16">{children}</main>
        </div>
      </body>
    </html>
  );
}

