import {defineConfig} from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path, {dirname} from 'path';
import child_process from 'child_process';
import {env} from 'process';
import istanbul from 'vite-plugin-istanbul';
import svgr from "vite-plugin-svgr";
import {fileURLToPath} from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

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

const isCypress = process.env.CYPRESS === 'true';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    plugin(),
    svgr(),
    istanbul({
      include: 'src/**/*.{ts,tsx}',
      exclude: ['node_modules', 'test/*', 'cypress/*'],
      extension: ['.ts', '.tsx'],
      cypress: isCypress,
      requireEnv: false,
      forceBuildInstrument: isCypress
    }),
  ],
  build: {
    sourcemap: true, // IMPORTANT: Enable source maps for accurate coverage
  },
  optimizeDeps: {
    entries: ['cypress/**/*'],
    include: ['react-router', 'oidc-client-ts', 'uuid']
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    proxy: {
      '^/api': {
        target,
        secure: false
      }
    },
    port: 60493,
    https: isCypress ? undefined : {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    },
    hmr: !isCypress
  },
  assetsInclude: ['/src/assets/**/*']
})
