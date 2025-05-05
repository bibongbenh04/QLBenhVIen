/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  // This is a special configuration for the ASP.NET Core MVC project
  // It tells Next.js to use the ASP.NET Core MVC project as the source
  distDir: ".next",
  // Disable server-side rendering for this project
  target: "serverless",
  eslint: {
    ignoreDuringBuilds: true,
  },
  typescript: {
    ignoreBuildErrors: true,
  },
  images: {
    unoptimized: true,
  },
}

module.exports = nextConfig
