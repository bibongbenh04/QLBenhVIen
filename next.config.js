/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  // Use the output property instead of target
  output: "standalone",
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
