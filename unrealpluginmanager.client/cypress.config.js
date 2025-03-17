import {defineConfig} from "cypress";

export default defineConfig({
  component: {
    devServer: {
      framework: "react",
      bundler: "vite",
      viteConfig: {
        server: {
          https: false,
        }
      }
    },
  },
  env: {
    theme: "dark",
  }
});
