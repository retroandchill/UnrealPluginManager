import {defineConfig} from "cypress";
import codeCoverageTask from "@cypress/code-coverage/task";
import useBabelRc from '@cypress/code-coverage/use-babelrc';

const __dirname = import.meta.dirname;

export default defineConfig({
  retries: {
    // Retry failed tests 2 times in run mode (CI)
    runMode: 2,
    // Retry failed tests 1 time in open mode (local)
    openMode: 1,
  },
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
