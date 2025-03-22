import {createRoot} from 'react-dom/client'
import App from './App.tsx'
import {createBrowserRouter} from "react-router-dom";
import CssBaseline from '@mui/material/CssBaseline';
import {createTheme, ThemeProvider} from '@mui/material/styles';

const darkTheme = createTheme({
  palette: {
    mode: 'dark',
  },
});


createRoot(document.getElementById('root')!).render(
    <ThemeProvider theme={darkTheme}>
      <CssBaseline/>
      <App routerFactory={createBrowserRouter}/>
    </ThemeProvider>
)
