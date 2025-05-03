import {AppLayout, HeaderBar, LandingPage, PluginPage} from "@/components";
import {Box} from '@mui/material';
import {createBrowserRouter, RouterProvider} from "react-router-dom";
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
          <HeaderBar onSearch={search => router.navigate(`/search?q=${search}`)}/>
          <RouterProvider router={router}/>
        </Box>
      </QueryClientProvider>
  );
}

export default App;