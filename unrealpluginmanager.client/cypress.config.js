import {defineConfig} from "cypress";

export default defineConfig({
  component: {
    devServer: {
      baseUrl: "http://localhost:1234",
      framework: "react",
      bundler: "vite",
      viteConfig: {
        server: {
          https: false,
        },
      },
    },
  },

  env: {
    theme: "dark",
  }
});
