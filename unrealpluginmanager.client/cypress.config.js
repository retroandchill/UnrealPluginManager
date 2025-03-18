import {defineConfig} from "cypress";
import codeCoverageTask from "@cypress/code-coverage/task";
import useBabelRc from '@cypress/code-coverage/use-babelrc';
import path from 'path';
import * as fs from 'fs';
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

      on("after:spec", async (spec, results) => {
        const specName = path.basename(spec.name, path.extname(spec.name)); // Get spec name (without extension)
        const outputDir = path.join(__dirname, "coverage");
        fs.rename(outputDir, path.join(__dirname, `coverage-${specName}`))
      });
      
      return config;
    },
  },

  env: {
    theme: "dark",
  }
});
