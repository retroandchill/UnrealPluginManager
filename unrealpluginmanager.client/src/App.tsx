import {ApiProvider, AppLayout, HeaderBar, LandingPage, PluginPage} from "@/components";
import {Box} from '@mui/material';
import {BrowserRouter, Route, Routes} from "react-router";
import {JSX, ReactNode} from "react";
import {QueryClient, QueryClientProvider,} from '@tanstack/react-query'
import {SearchPage} from "@/components/pages/SearchPage.tsx";
import {AuthProvider} from "react-oidc-context";
import {UserManager, WebStorageStateStore} from "oidc-client-ts";

interface BaseRouterProps {
  basename?: string;
  children?: ReactNode;
  window?: Window;
}

/**
 * Represents the properties required by an application.
 *
 * This interface primarily defines the configuration options necessary for setting up the application's routing logic.
 *
 * Properties:
 * - `routerFactory`: The function responsible for creating the router instance. It should be of the type `createBrowserRouter` from the routing library. This function facilitates navigation and manages the application's routes.
 */
interface AppProps<P extends BaseRouterProps> {
  routerType: (props: P) => JSX.Element;
  routerProps: P;
}

const queryClient = new QueryClient();

const userManager = new UserManager({
  authority: import.meta.env.VITE_AUTHORITY,
  client_id: import.meta.env.VITE_CLIENT_ID,
  redirect_uri: `${window.location.origin}${window.location.pathname}`,
  post_logout_redirect_uri: window.location.origin,
  userStore: new WebStorageStateStore({store: window.sessionStorage}),
  monitorSession: true
});

function onSigninCallback() {
  window.history.replaceState({}, document.title, window.location.pathname);
}

/**
 * The App class is a React component that manages and displays a list of plugins.
 * It fetches the plugin data from a backend server and renders the plugins in a table.
 * The component communicates with an ASP.NET backend and showcases an example
 * integration between JavaScript and ASP.NET.
 */
function App<P extends BaseRouterProps>({routerType: Router = BrowserRouter, routerProps}: Readonly<AppProps<P>>) {
  return (
      <AuthProvider userManager={userManager} onSigninCallback={onSigninCallback}>
        <ApiProvider>
          <QueryClientProvider client={queryClient}>
            <Box minHeight="100vh" display="flex" flexDirection="column">
              <Router {...routerProps}>
                <HeaderBar/>
                <Routes>
                  <Route path="/" element={<AppLayout/>}>
                    <Route index element={<LandingPage/>}/>
                    <Route path="search" element={<SearchPage/>}/>
                    <Route path="plugin/:id" element={<PluginPage/>}/>
                  </Route>
                </Routes>
              </Router>
            </Box>
          </QueryClientProvider>
        </ApiProvider>
      </AuthProvider>
  );
}

export default App;