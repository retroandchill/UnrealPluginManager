import {Button, Grid, Typography} from '@mui/material';

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
          <Button variant="contained" color="primary" component="div" style={{textTransform: 'none'}}>
            <Typography variant="h5" component="div" sx={{marginX: '25px'}}>
              Downloads
            </Typography>
          </Button>
          <Button variant="contained" color="secondary" component="div" style={{textTransform: 'none'}}>
            <Typography variant="h5" component="div" sx={{marginX: '25px'}}>
              Learn More
            </Typography>
          </Button>
        </Grid>
      </Grid>
  );
}