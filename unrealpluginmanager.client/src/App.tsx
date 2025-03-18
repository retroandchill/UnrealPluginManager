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

/**
 * Represents the properties required by an application.
 *
 * This interface primarily defines the configuration options necessary for setting up the application's routing logic.
 *
 * Properties:
 * - `routerFactory`: The function responsible for creating the router instance. It should be of the type `createBrowserRouter` from the routing library. This function facilitates navigation and manages the application's routes.
 */
interface AppProps {
  /**
   * A factory function for creating routers. This variable is assigned the `createBrowserRouter`
   * function's type, which is used to configure and create a browser-based router for managing
   * application routes.
   *
   * It provides the ability to define routes, navigation, and history handling within
   * the application using the React Router architecture.
   */
  routerFactory: typeof createBrowserRouter;
}

/**
 * The App class is a React component that manages and displays a list of plugins.
 * It fetches the plugin data from a backend server and renders the plugins in a table.
 * The component communicates with an ASP.NET backend and showcases an example
 * integration between JavaScript and ASP.NET.
 */
function App(props: Readonly<AppProps>) {

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