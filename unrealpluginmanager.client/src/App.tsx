import './App.css';
import {PluginDisplayGrid} from "./components";
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';
import {createBrowserRouter} from "react-router-dom";
import {RouterProvider} from "react-router";

interface AppProps {
  routerFactory: typeof createBrowserRouter;
}

/**
 * The App class is a React component that manages and displays a list of plugins.
 * It fetches the plugin data from a backend server and renders the plugins in a table.
 * The component communicates with an ASP.NET backend and showcases an example
 * integration between JavaScript and ASP.NET.
 *
 * @extends Component
 */
function App(props: AppProps) {

  const router = props.routerFactory([
    {
      path: '/',
        element: <PluginDisplayGrid onPluginClick={(plugin) => router.navigate(`/plugin/${plugin.pluginId}`)}/>
    },
    {
      path: '/plugin/:id',
      element: <div>This is a test!</div>
    }
  ]);

  return <div>
    <AppBar position="static">
      <Toolbar>
        <IconButton
            size="large"
            edge="start"
            color="inherit"
            aria-label="menu"
            sx={{mr: 2}}
        >
          <MenuIcon/>
        </IconButton>
        <Typography variant="h4" component="div" sx={{flexGrow: 1}}>
          Unreal Plugin Manager
        </Typography>
        <Button color="inherit">Login</Button>
      </Toolbar>
    </AppBar>
    <div style={{padding: '2%'}}>
      <RouterProvider router={router}/>
    </div>
  </div>;

}

export default App;