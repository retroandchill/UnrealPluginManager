import {AppLayout, LandingPage, PluginPage, Search, SearchIconWrapper, StyledInputBase} from "@/components";
import {AppBar, Box, Button, Container, Toolbar, Typography} from '@mui/material';
import {useState} from "react";
import {createBrowserRouter, RouterProvider} from "react-router-dom";
import SearchIcon from '@mui/icons-material/Search';
import {QueryClient, QueryClientProvider,} from '@tanstack/react-query'
import {SearchPage} from "@/components/pages/SearchPage.tsx";

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

const queryClient = new QueryClient();

/**
 * The App class is a React component that manages and displays a list of plugins.
 * It fetches the plugin data from a backend server and renders the plugins in a table.
 * The component communicates with an ASP.NET backend and showcases an example
 * integration between JavaScript and ASP.NET.
 */
function App({routerFactory}: Readonly<AppProps>) {
  const [search, setSearch] = useState('');

  const router = routerFactory([
    {
      path: '/',
      element: <AppLayout/>,
      children: [
        {
          path: '/',
          element: <LandingPage/>
        },
        {
          path: '/search',
          element: <SearchPage/>
        },
        {
          path: '/plugin/:id',
          element: <PluginPage/>
        }
      ]
    }
  ]);

  return (
      <QueryClientProvider client={queryClient}>
        <Box minHeight="100vh" display="flex" flexDirection="column">
          <AppBar position="static">
            <Container>
              <Toolbar style={{marginTop: '10px', marginBottom: '10px'}}>
                <Box display='flex' flexGrow={1} alignItems="center">
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
            </Container>
          </AppBar>
          <RouterProvider router={router}/>
        </Box>
      </QueryClientProvider>
  );
}

export default App;