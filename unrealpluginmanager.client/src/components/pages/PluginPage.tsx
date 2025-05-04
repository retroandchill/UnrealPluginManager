import {Box, CircularProgress, Container, Grid, Paper, Tab, Typography,} from '@mui/material';
import {TabContext, TabList, TabPanel} from '@mui/lab';
import {useParams, useSearchParams} from 'react-router';
import {PluginPageHeader, PluginPageSidebar, PluginReadmeDisplay, useApi} from "@/components";
import DescriptionOutlinedIcon from '@mui/icons-material/DescriptionOutlined';
import AccountTreeOutlinedIcon from '@mui/icons-material/AccountTreeOutlined';
import DownloadOutlinedIcon from '@mui/icons-material/DownloadOutlined';
import VersionsOutlinedIcon from '@mui/icons-material/StyleOutlined';
import {useQuery, useQueryClient} from '@tanstack/react-query';


export function PluginPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const pluginId = useParams().id ?? "";
  const queryClient = useQueryClient();
  const {pluginsApi} = useApi();

  const plugin = useQuery({
    queryKey: ['plugin', pluginId],
    queryFn: () => pluginsApi.getLatestVersion({pluginId: pluginId})
  }, queryClient);

  const tabStyle = {
    minHeight: 64,
    '& .MuiTab-iconWrapper': {
      marginRight: 1
    }
  };

  if (plugin.data === undefined) {
    return (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
          <CircularProgress/>
        </Box>
    );
  }

  return (
      <Container maxWidth="lg">
        <Box sx={{py: 4}}>
          <PluginPageHeader plugin={plugin.data}/>

          <Box sx={{mt: 4}}>
            <TabContext value={searchParams.get("currentTab") ?? "readme"}>
              <Paper sx={{mb: 3}}>
                <TabList
                    onChange={(_, v) => setSearchParams({currentTab: v})}
                    aria-label="Plugin information tabs"
                    sx={{
                      borderBottom: 1,
                      borderColor: 'divider',
                      px: 2
                    }}
                >
                  <Tab
                      icon={<DescriptionOutlinedIcon/>}
                      iconPosition="start"
                      label="Readme"
                      value="readme"
                      sx={tabStyle}/>
                  <Tab
                      icon={<AccountTreeOutlinedIcon/>}
                      iconPosition="start"
                      label="Dependencies"
                      value="dependencies"
                      sx={tabStyle}/>
                  <Tab
                      icon={<DownloadOutlinedIcon/>}
                      iconPosition="start"
                      label="Download"
                      value="download"
                      sx={tabStyle}/>
                  <Tab
                      icon={<VersionsOutlinedIcon/>}
                      iconPosition="start"
                      label="Versions"
                      value="versions"
                      sx={tabStyle}/>
                </TabList>
              </Paper>

              <Grid container spacing={3}>
                <Grid size={{xs: 12, md: 8.5}}>
                  <Paper elevation={2} sx={{p: 3}}>
                    <TabPanel value="readme" sx={{p: 0}}>
                      <PluginReadmeDisplay pluginId={pluginId} versionId={plugin.data.versionId}/>
                    </TabPanel>
                    <TabPanel value="dependencies" sx={{p: 0}}>
                      <Typography variant="h6">Dependencies</Typography>
                      <Typography color="text.secondary">Coming soon!</Typography>
                    </TabPanel>
                    <TabPanel value="download" sx={{p: 0}}>
                      <Typography variant="h6">Download Options</Typography>
                      <Typography color="text.secondary">Coming soon!</Typography>
                    </TabPanel>
                    <TabPanel value="versions" sx={{p: 0}}>
                      <Typography variant="h6">Versions</Typography>
                      <Typography color="text.secondary">Coming soon!</Typography>
                    </TabPanel>
                  </Paper>
                </Grid>

                {/* Sidebar */}
                <Grid size={{xs: 12, md: 3.5}}>
                  <PluginPageSidebar plugin={plugin.data}/>
                </Grid>
              </Grid>
            </TabContext>
          </Box>
        </Box>
      </Container>
  );
}