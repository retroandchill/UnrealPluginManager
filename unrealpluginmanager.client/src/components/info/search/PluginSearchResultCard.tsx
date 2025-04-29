import {Box, Card, CardContent, Chip, Grid, Link, Typography} from '@mui/material';
import {PluginVersionInfo} from "@/api";
import {apiResourcesPath} from "@/config/Globals.ts";

interface PluginSearchResultCardProps {
  plugin: PluginVersionInfo;
}

export function PluginSearchResultCard({plugin}: Readonly<PluginSearchResultCardProps>) {
  const route = `/plugin/${plugin.pluginId}`

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
          <Link
              href={route}
              sx={{
                cursor: 'pointer',
                textDecoration: 'none',
                color: 'inherit'
              }}
          >
            <Grid container spacing={2}>
              <Grid size={2}>
              
                <Box
                    component="img"
                    src={plugin.icon ? `${apiResourcesPath}/${plugin.icon.storedFilename}` : "Icon128.png"}
                    alt="Plugin Icon"
                    sx={{
                      height: '100%',
                      maxWidth: '128px',
                      display: 'block'
                    }}
                />
            </Grid>
              <Grid size={4} sx={{alignSelf: 'flex-start'}}>
                <Typography variant="h4" gutterBottom>
                  {plugin.friendlyName}
                </Typography>
                <Chip
                    label={`Version ${plugin.version}`}
                    color="primary"
                    sx={{mb: 2}}
                />
                <Typography variant="body2" color="text.secondary">
                  <strong>Author:</strong> {plugin.authorName ?? 'Unknown'}
                </Typography>
            </Grid>
              <Grid size={6} sx={{alignSelf: 'flex-start'}}>
                <Typography variant="body2" gutterBottom>
                  {plugin.description}
                </Typography>
              </Grid>
          </Grid>

          </Link>
        </CardContent>
      </Card>
  );
}