import {Box, Grid, IconButton, Link, Paper, Stack, SxProps, Theme, Typography} from '@mui/material';

import GithubIcon from '@mui/icons-material/Github';
import {Outlet} from "react-router-dom";
import {secondaryColor} from "@/Theme.ts";

/**
 * A React component that defines the layout structure of an application. It consists of a primary content area
 * and a footer section. The primary content area includes a flexible container to render nested routes using
 * React Router's `Outlet`. The footer section provides support links, GitHub link, and a list of terms and policies.
 *
 * @return The rendered layout component featuring a main content area and footer with various links.
 */
export function AppLayout() {
  const linkStyle: SxProps<Theme> = {
    color: 'inherit',
    textDecoration: 'none',
    '&:hover': {
      color: secondaryColor,
      textDecoration: 'underline',
    },
  };

  return (
      <Box display="flex" flexGrow={1} flexDirection="column">
        <Box display='flex' flexGrow={1} flexDirection="column" padding="2%">
          <Outlet/>
        </Box>
        <Paper elevation={2}>
          <Box display="flex" flexGrow={1} alignItems="center" justifyContent="center" marginY="10px">
            <Grid container flexGrow={1} justifyContent="center" maxWidth="60%">
              <Stack>
                <IconButton
                    size="large"
                    edge="start"
                    color="inherit"
                    aria-label="github"
                    sx={{mr: 2}}
                    href="https://github.com/retroandchill/UnrealPluginManager"
                >
                  <GithubIcon sx={{fontSize: 64}}/>
                </IconButton>
              </Stack>
              <Stack spacing={2} alignItems="flex-start" marginLeft="2%" marginRight="20%">
                <Typography variant="h4" component="div">
                  Support
                </Typography>
                <Stack>
                  <Link href="/docs" sx={linkStyle}>
                    Documentation
                  </Link>
                  <Link href="/contact" sx={linkStyle}>
                    Contact Us
                  </Link>
                </Stack>
              </Stack>
              <Stack spacing={2} alignItems="flex-start">
                <Typography variant="h4" component="div">
                  Terms & Policies
                </Typography>
                <Stack>
                  <Link href="/policies" sx={linkStyle}>
                    Policies
                  </Link>
                  <Link href="/terms-of-use" sx={linkStyle}>
                    Terms of Use
                  </Link>
                  <Link href="/code-of-conduct" sx={linkStyle}>
                    Code of Conduct
                  </Link>
                  <Link href="/privacy" sx={linkStyle}>
                    Privacy
                  </Link>
                </Stack>
              </Stack>
            </Grid>
          </Box>
        </Paper>
      </Box>
  );
}