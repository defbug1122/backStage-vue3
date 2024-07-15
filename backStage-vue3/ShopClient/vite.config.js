import { defineConfig } from "vite";
import { createVuePlugin } from "vite-plugin-vue2";
import path from "path";
import dotenv from "dotenv";

// 加載 .env 文件
dotenv.config({ path: `.env.${process.env.NODE_ENV}` });

export default defineConfig({
  base: "./",
  plugins: [createVuePlugin()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src"),
    },
  },
  define: {
    __VUE_PROD_DEVTOOLS__: "false",
    "process.env": {
      VITE_APP_TITLE: process.env.VITE_APP_TITLE,
      VITE_API_URL: process.env.VITE_API_URL,
      VITE_APP_MODE: process.env.VITE_APP_MODE,
    },
  },
  build: {
    rollupOptions: {
      output: {
        entryFileNames: `assets/[name].js`,
        chunkFileNames: `assets/[name].js`,
        assetFileNames: `assets/[name].[ext]`,
      },
    },
  },
  server: {
    proxy: {
      "/api": {
        target: process.env.VITE_API_URL,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
