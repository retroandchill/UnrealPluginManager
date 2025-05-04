import {Box, Card, CardContent, Chip, Grid, Paper, Typography} from '@mui/material';
import {CopyBlock, dracula} from 'react-code-blocks';
import {apiResourcesPath} from "@/config";
import {getInstallCommand} from "@/util";
import {PluginVersionInfo} from "@/api";

interface PluginPageHeaderProps {
  plugin: PluginVersionInfo;
}

export function PluginPageHeader({plugin}: Readonly<PluginPageHeaderProps>) {
  return (
      <Card elevation={3}>
        <CardContent>
          <Grid container spacing={4} alignItems="center">
            <Grid size={{xs: 12, sm: 2}}>
              <Box
                  component="img"
                  src={plugin.icon ? `${apiResourcesPath}/${plugin.icon.storedFilename}` : "Icon128.png"}
                  alt="Plugin Icon"
                  sx={{
                    width: '100%',
                    height: 'auto',
                    maxWidth: '128px',
                    display: 'block',
                    margin: 'auto'
                  }}
              />
            </Grid>
            <Grid size={{xs: 12, sm: 6}}>
              <Typography variant="h4" gutterBottom>
                {plugin.friendlyName || plugin.name}
              </Typography>
              <Chip
                  label={`Version ${plugin.version}`}
                  color="primary"
                  sx={{mb: 2}}
              />
            </Grid>
            <Grid size={{xs: 12, sm: 4}}>
              <Paper
                  elevation={2}
                  sx={{
                    p: 2,
                    backgroundColor: 'rgba(0, 0, 0, 0.03)'
                  }}
              >
                <Typography variant="subtitle2" gutterBottom>
                  Installation Command
                </Typography>
                <CopyBlock
                    text={getInstallCommand(plugin)}
                    language="bash"
                    theme={dracula}
                    codeBlock
                    showLineNumbers={false}
                />
              </Paper>
            </Grid>
          </Grid>
        </CardContent>
      </Card>
  );
}