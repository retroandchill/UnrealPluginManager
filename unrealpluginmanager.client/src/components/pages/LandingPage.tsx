import {Box, Button, Card, CardContent, Container, Grid, Typography, useTheme,} from '@mui/material';

import {CloudDownload, Extension, Security, Speed} from '@mui/icons-material';


/**
 * Renders the landing page showcasing the main features and options for the Unreal Plugin Manager application.
 *
 * @return A React component displaying the title, description, and action buttons for navigating to downloads and documentation pages.
 */
export function LandingPage() {
  const theme = useTheme();

  const features = [
    {
      key: "easy-downloads",
      icon: <CloudDownload sx={{fontSize: 40}}/>,
      title: 'Easy Downloads',
      description: 'Download Unreal Engine packages with a single click',
    },
    {
      key: "plugin-management",
      icon: <Extension sx={{fontSize: 40}}/>,
      title: 'Plugin Management',
      description: 'Manage all your Unreal Engine plugins in one place',
    },
    {
      key: "fast-installation",
      icon: <Speed sx={{fontSize: 40}}/>,
      title: 'Fast Installation',
      description: 'Lightning-fast package installation and updates',
    },
    {
      key: "secure-downloads",
      icon: <Security sx={{fontSize: 40}}/>,
      title: 'Secure Downloads',
      description: 'Verified and secure package distribution',
    },
  ];


  return (
      <Box sx={{flexGrow: 1}}>
        {/* Hero Section */}
        <Box
            sx={{
              backgroundColor: theme.palette.primary.main,
              py: 8,
            }}
            textAlign="center"
        >
          <Container>
            <Grid container spacing={4} alignItems="center" justifyContent="center">
              <Grid size={{xs: 12, sm: 6, md: 6}}>
                <Typography variant="h2" component="h1" gutterBottom>
                  Supercharge Your Unreal Engine Development
                </Typography>
                <Typography variant="h5" paragraph>
                  The most powerful package manager for Unreal Engine assets, plugins, and more.
                </Typography>
                <Button
                    variant="contained"
                    size="large"
                    sx={{
                      backgroundColor: 'white',
                      color: theme.palette.primary.main,
                      '&:hover': {
                        backgroundColor: theme.palette.grey[100],
                      },
                      mt: 2,
                    }}
                >
                  Get Started
                </Button>
              </Grid>
            </Grid>
          </Container>
        </Box>

        {/* Features Section */}
        <Container sx={{py: 8}}>
          <Typography variant="h3" component="h2" textAlign="center" gutterBottom>
            Features
          </Typography>
          <Grid container spacing={4} sx={{mt: 4}}>
            {features.map(feature => (
                <Grid size={{xs: 12, sm: 6, md: 3}} key={feature.key}>
                  <Card
                      sx={{
                        height: '100%',
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        textAlign: 'center',
                        p: 2,
                      }}
                  >
                    <Box sx={{color: theme.palette.primary.main, mb: 2}}>
                      {feature.icon}
                    </Box>
                    <CardContent>
                      <Typography variant="h5" component="h3" gutterBottom>
                        {feature.title}
                      </Typography>
                      <Typography variant="body1" color="text.secondary">
                        {feature.description}
                      </Typography>
                    </CardContent>
                  </Card>
                </Grid>
            ))}
          </Grid>
        </Container>

        {/* Call to Action Section */}
        <Box sx={{backgroundColor: theme.palette.primary.main, py: 8}}>
          <Container>
            <Box textAlign="center">
              <Typography variant="h4" component="h2" gutterBottom>
                Ready to streamline your Unreal Engine development?
              </Typography>
              <Button
                  variant="contained"
                  size="large"
                  sx={{
                    mt: 3, backgroundColor: 'white', color: theme.palette.primary.main,
                    '&:hover': {backgroundColor: theme.palette.grey[100]}
                  }}
              >
                Download Now
              </Button>
            </Box>
          </Container>
        </Box>
      </Box>
  );
}