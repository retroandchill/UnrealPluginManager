import {createRoot} from 'react-dom/client'
import App from './App.tsx'
import {createBrowserRouter} from "react-router-dom";


createRoot(document.getElementById('root')!).render(
    <App routerFactory={createBrowserRouter}/>
)
