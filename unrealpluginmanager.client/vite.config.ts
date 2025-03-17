import {fileURLToPath, URL} from 'node:url';

import {defineConfig} from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import {env} from 'process';
import istanbul from 'vite-plugin-istanbul';
import viteBabel from "vite-plugin-babel";

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "unrealpluginmanager.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(baseFolder)) {
  fs.mkdirSync(baseFolder, {recursive: true});
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (0 !== child_process.spawnSync('dotnet', [
    'dev-certs',
    'https',
    '--export-path',
    certFilePath,
    '--format',
    'Pem',
    '--no-password',
  ], {stdio: 'inherit',}).status) {
    throw new Error("Could not create certificate.");
  }
}

let target: string;
if (env.ASPNETCORE_HTTPS_PORT) {
  target = `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`;
} else {
  target = env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7231';
}

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    plugin(),
    viteBabel({
      babelConfig: {
        presets: ["@babel/preset-env", "@babel/preset-typescript"],
        plugins: [["istanbul", { useInlineSourceMaps: false }]]
      }
    }),  
    istanbul({
      include: 'src/**/*.{ts,tsx}',
      exclude: ['node_modules', 'test/*', 'cypress/*'],
      extension: ['.ts', '.tsx'],
      /**
       * This allows us to omit the INSTRUMENT_BUILD env variable when running the production build via
       * npm run build.
       * More details below.
       */
      requireEnv: true,
      /**
       * If forceBuildInstrument is set to true, this will add coverage instrumentation to the
       * built dist files and allow the reporter to collect coverage from the (built files).
       * However, when forceBuildInstrument is set to true, it will not collect coverage from
       * running against the dev server: e.g. npm run dev.
       *
       * To allow collecting coverage from running cypress against the dev server as well as the
       * preview server (built files), we use an env variable, INSTRUMENT_BUILD, to set
       * forceBuildInstrument to true when running against the preview server via the
       * instrument-build npm script.
       *
       * When you run `npm run build`, the INSTRUMENT_BUILD env variable is omitted from the npm
       * script which will result in forceBuildInstrument being set to false, ensuring your
       * dist/built files for production do not include coverage instrumentation code.
       */
      forceBuildInstrument: Boolean(process.env.INSTRUMENT_BUILD)
    }),
  ],
  build: {
    sourcemap: true, // IMPORTANT: Enable source maps for accurate coverage
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    proxy: {
      '^/api': {
        target,
        secure: false
      }
    },
    port: 60493,
    https: {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    }
  },
  assetsInclude: ['/src/assets/**/*']
})
