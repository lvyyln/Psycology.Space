import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': 'http://localhost:5032'
    }
  },
  build: {
    outDir: '../Psycology.Space/wwwroot',
    emptyOutDir: true
  }
})
