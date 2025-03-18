import {defineConfig} from "cypress";
import codeCoverageTask from "@cypress/code-coverage/task";
import useBabelRc from '@cypress/code-coverage/use-babelrc';
import path from 'path';
const __dirname = import.meta.dirname;

export default defineConfig({
  component: {
    devServer: {
      framework: "react",
      bundler: "vite",
    },
    setupNodeEvents(on, config) {
      codeCoverageTask(on, config);
      on('file:preprocessor', useBabelRc)
      return config;
    },
  },

  env: {
    theme: "dark",
  }
});
