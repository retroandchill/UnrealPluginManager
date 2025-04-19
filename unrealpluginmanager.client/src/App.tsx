import {AppLayout, LandingPage, PluginPage, Search, SearchIconWrapper, StyledInputBase} from "./components";
import {
  AppBar,
  Box,
  Button,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography
} from '@mui/material';
import {useState} from "react";
import {createBrowserRouter, RouterProvider} from "react-router-dom";
import SearchIcon from '@mui/icons-material/Search';
import MenuIcon from '@mui/icons-material/Menu';
import HomeIcon from '@mui/icons-material/Home';
import DownloadIcon from '@mui/icons-material/Download';
import DocumentationIcon from '@mui/icons-material/Article';
import ExtensionIcon from '@mui/icons-material/Extension';

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
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState('');

  const router = routerFactory([
    {
      path: '/',
      element: <AppLayout/>,
      children: [
        {
          path: '/',
          element: <LandingPage/>,
          errorElement: <LandingPage/>
        },
        {
          path: '/plugin/:id',
          element: <PluginPage/>
        }
      ]
    }
  ]);

  return (
      <>
        <Box minHeight="100vh" display="flex" flexDirection="column">
          <AppBar position="static" color="primary">
            <Toolbar style={{marginTop: '10px', marginBottom: '10px'}}>
              <Box display='flex' flexGrow={1} alignItems="center">
                <IconButton
                    size="large"
                    edge="start"
                    color="inherit"
                    aria-label="menu"
                    sx={{mr: 2}}
                    onClick={() => setOpen(true)}
                >
                  <MenuIcon/>
                </IconButton>
                <Typography variant="h4" component="div" marginRight="10px">
                  Unreal Plugin Manager
                </Typography>
                <Search sx={{marginX: '10px'}}>
                  <SearchIconWrapper>
                    <SearchIcon/>
                  </SearchIconWrapper>
                  <StyledInputBase
                      placeholder="Search…"
                      inputProps={{'aria-label': 'search'}}
                      onChange={e => setSearch(e.target.value)}
                      value={search}
                      onKeyDown={e => {
                        if (e.key === 'Enter') {
                          router.navigate(`/search?q=${search}`);
                        }
                      }}
                  />
                </Search>
              </Box>
              <Button variant="contained" color="primary">Login</Button>
            </Toolbar>
          </AppBar>
          <RouterProvider router={router}/>
        </Box>
        <Drawer open={open} onClose={() => setOpen(false)}>
          <List>
            <ListItemButton href="/">
              <ListItemIcon>
                <HomeIcon/>
              </ListItemIcon>
              <ListItemText primary="Home"/>
            </ListItemButton>
            <ListItemButton href="/downloads">
              <ListItemIcon>
                <DownloadIcon/>
              </ListItemIcon>
              <ListItemText primary="Downloads"/>
            </ListItemButton>
            <ListItemButton href="/plugins">
              <ListItemIcon>
                <ExtensionIcon/>
              </ListItemIcon>
              <ListItemText primary="Plugins"/>
            </ListItemButton>
            <ListItemButton href="/docs">
              <ListItemIcon>
                <DocumentationIcon/>
              </ListItemIcon>
              <ListItemText primary="Documentation"/>
            </ListItemButton>
          </List>
        </Drawer>
      </>
  );
}

export default App;