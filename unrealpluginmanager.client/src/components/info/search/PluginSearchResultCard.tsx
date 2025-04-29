import {Box, Card, CardContent, Grid, Link, Typography} from '@mui/material';
import {PluginVersionInfo} from "@/api";
import {apiResourcesPath} from "@/config/Globals.ts";

interface PluginSearchResultCardProps {
  plugin: PluginVersionInfo;
}

export function PluginSearchResultCard({plugin}: Readonly<PluginSearchResultCardProps>) {
  const route = `/plugins/${plugin.pluginId}`

  return (
      <Card
          sx={{
            width: '100%',
            mb: 2,
            '&:hover': {
              boxShadow: 6
            }
          }}
      >
        <CardContent>
          <Grid container spacing={2} alignItems="center">
            <Grid size={2}>
              <Link
                  href={route}
                  sx={{cursor: 'pointer'}}
              >
                <Box
                    component="img"
                    src={plugin.icon ? `${apiResourcesPath}/${plugin.icon.storedFilename}` : "Icon128.png"}
                    alt="Plugin Icon"
                    sx={{
                      width: '100%',
                      height: 'auto',
                      maxWidth: '64px',
                      display: 'block'
                    }}
                />
              </Link>
            </Grid>
            <Grid size={10}>
              <Link
                  href={route}
                  sx={{
                    cursor: 'pointer',
                    textDecoration: 'none',
                    color: 'inherit'
                  }}
              >
                <Typography variant="h6" gutterBottom>
                  {plugin.name}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  <strong>Latest Release:</strong> {plugin.version}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  <strong>Author:</strong> {plugin.authorName ?? 'Unknown'}
                </Typography>
              </Link>
            </Grid>
          </Grid>
        </CardContent>
      </Card>
  );
}