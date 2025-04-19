import {
  PluginDisplayGrid
} from "./components";
import {createBrowserRouter} from "react-router-dom";
import PluginPage from "@/components/PluginPage.tsx";
import AppTheme from "@/components/theme/AppTheme";
import {CssBaseline, Divider} from '@mui/material';
import AppAppBar from "@/components/AppAppBar.tsx";
import Hero from "@/components/Hero.tsx";
import LogoCollection from "@/components/LogoCollection.tsx";
import Features from "@/components/Features.tsx";
import Testimonials from "@/components/Testimonials.tsx";
import Highlights from "@/components/Highlights.tsx";
import Pricing from "@/components/Pricing.tsx";
import FAQ from "@/components/FAQ.tsx";
import Footer from "@/components/Footer.tsx";

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

  disableCustomTheme?: boolean;
}

/**
 * The App class is a React component that manages and displays a list of plugins.
 * It fetches the plugin data from a backend server and renders the plugins in a table.
 * The component communicates with an ASP.NET backend and showcases an example
 * integration between JavaScript and ASP.NET.
 */
function App({routerFactory, ...props}: Readonly<AppProps>) {
  const router = routerFactory([
    {
      path: '/',
        element: <PluginDisplayGrid onPluginClick={(plugin) => router.navigate(`/plugin/${plugin.pluginId}`)}/>
    },
    {
      path: '/plugin/:id',
      element: <PluginPage/>
    }
  ]);

  return (
      <AppTheme {...props}>
        <CssBaseline enableColorScheme/>

        <AppAppBar/>
        <Hero/>
        <div>
          <LogoCollection/>
          <Features/>
          <Divider/>
          <Testimonials/>
          <Divider/>
          <Highlights/>
          <Divider/>
          <Pricing/>
          <Divider/>
          <FAQ/>
          <Divider/>
          <Footer/>
        </div>
      </AppTheme>
  );
}

export default App;