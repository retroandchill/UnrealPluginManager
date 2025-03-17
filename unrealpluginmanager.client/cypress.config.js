import {defineConfig} from "cypress";
import codeCoverageTask from "@cypress/code-coverage/task";

export default defineConfig({
  component: {
    devServer: {
      framework: "react",
      bundler: "vite",
      viteConfig: {
        server: {
          https: false,
        },
      },
    },
    setupNodeEvents(on, config) {
      codeCoverageTask(on, config);
      // `on` is used to hook into various events Cypress emits
      // `config` is the resolved Cypress config
      return config;
    },
  },

  env: {
    theme: "dark",
  }
});
