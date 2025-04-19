import {createRoot} from 'react-dom/client'
import App from './App.tsx'
import {createBrowserRouter} from "react-router-dom";
import CssBaseline from '@mui/material/CssBaseline';
import {createTheme, ThemeProvider} from '@mui/material/styles';
import {theme} from "@/Theme.ts";

const appTheme = createTheme(theme);
createRoot(document.getElementById('root')!).render(
    <ThemeProvider theme={appTheme}>
      <CssBaseline/>
      <App routerFactory={createBrowserRouter}/>
    </ThemeProvider>
)
