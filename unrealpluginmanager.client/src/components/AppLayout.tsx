import {Box} from '@mui/material';

import {Outlet} from "react-router";
import {Footer} from "@/components/Footer.tsx";

/**
 * A React component that defines the layout structure of an application. It consists of a primary content area
 * and a footer section. The primary content area includes a flexible container to render nested routes using
 * React Router's `Outlet`. The footer section provides support links, GitHub link, and a list of terms and policies.
 *
 * @return The rendered layout component featuring a main content area and footer with various links.
 */
export function AppLayout() {
  return (
      <Box display="flex" flexGrow={1} flexDirection="column">
        <Box display='flex' flexGrow={1} flexDirection="column">
          <Outlet/>
        </Box>
        <Footer/>
      </Box>
  );
}