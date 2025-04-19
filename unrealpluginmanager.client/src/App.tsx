import {LandingPage} from "./components";
import {AppBar, Button, TextField, Toolbar, Typography} from '@mui/material';
import {createBrowserRouter} from "react-router-dom";
import {RouterProvider} from "react-router";
import PluginPage from "@/components/PluginPage.tsx";

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
function App({routerFactory}: Readonly<AppProps>) {
  const router = routerFactory([
    {
      path: '/',
      element: <LandingPage/>,
    },
    {
      path: '/plugin/:id',
      element: <PluginPage/>
    }
  ]);

  return <>
    <AppBar position="static">
      <Toolbar style={{marginTop: '10px', marginBottom: '10px'}}>
        <Typography variant="h4" component="div" sx={{flexGrow: 1}}>
          Unreal Plugin Manager
        </Typography>
        <TextField id="outlined-search" label="Seach plugins" type="search"
                   style={{width: '20%', marginRight: '10px'}}/>
        <Button variant="outlined" color="primary" style={{marginRight: '10px'}} onClick={() => {
        }}>
          Search
        </Button>
        <Button variant="contained" color="primary">Login</Button>
      </Toolbar>
    </AppBar>
    <div style={{padding: '2%'}}>
      <RouterProvider router={router}/>
    </div>
  </>;
}

export default App;