import {Grid, Typography} from '@mui/material';
import {GradientButton} from "@/components";

/**
 * Renders the landing page showcasing the main features and options for the Unreal Plugin Manager application.
 *
 * @return A React component displaying the title, description, and action buttons for navigating to downloads and documentation pages.
 */
export function LandingPage() {
  return (
      <Grid container direction="column" spacing={2} justifyContent="center" alignItems="center">
        <Typography variant="h2" component="div" sx={{flexGrow: 1}}>
          Manage Plugins with Ease
        </Typography>

        <Typography component="div" sx={{flexGrow: 1}}>
          Unreal Plugin Manager is a free and open source tool for working with Unreal Engine plugins, allowing you to
          easily download, build, and install plugins into the engine.
        </Typography>

        <Grid container direction="row" spacing={2} size="grow">
          <GradientButton variant="contained" color="primary" style={{textTransform: 'none'}} href="/downloads">
            <Typography variant="h5" component="div" sx={{marginX: '25px'}}>
              Downloads
            </Typography>
          </GradientButton>
          <GradientButton variant="contained" color="secondary" style={{textTransform: 'none'}}
                          color1={{color: '#6b7ffe'}}
                          color2={{color: '#6bc6fe'}} href="/docs">
            <Typography variant="h5" component="div" sx={{marginX: '25px'}}>
              Learn More
            </Typography>
          </GradientButton>
        </Grid>
      </Grid>
  );
}