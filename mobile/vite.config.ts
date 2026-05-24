import { defineConfig } from 'vite'
import uni from '@dcloudio/vite-plugin-uni'

export default defineConfig({
  plugins: [uni()],
  server: {
    port: 8076,
    proxy: {
      '/api': {
        target: 'http://localhost:5242',
        changeOrigin: true
      }
    }
  }
})
