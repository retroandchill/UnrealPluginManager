import {useEffect, useState} from 'react';
import {PluginVersionInfo} from "@/api";
import {apiResourcesPath, pluginsApi} from "@/config";
import {Grid2, Stack, Box, Tab} from "@mui/material";
import {TabContext, TabList, TabPanel} from "@mui/lab";
import { useSearchParams } from 'react-router';
import {PluginReadmeDisplay} from "@/components";

/**
 * Represents the properties required for a PluginPage component.
 * This interface defines the structure expected for the data provided
 * to configure or render a page associated with a specific plugin.
 *
 * Properties:
 * - pluginId: A unique identifier for the plugin associated with the page.
 */
interface PluginPageProps {
  /**
   * Represents the unique identifier for a plugin.
   * This ID is utilized internally to differentiate plugins
   * and is typically defined in the plugin's configuration.
   */
  pluginId: string;
}

/**
 * Renders the PluginPage component that displays plugin details including icon, name, version, and several tabs like Readme, Dependencies, and Download.
 * Fetches the latest version of a plugin based on the provided plugin ID.
 *
 * @param props - The props for the PluginPage component.
 * @param props.pluginId - The unique identifier of the plugin to fetch and display information for.
 * @return The rendered PluginPage component with plugin details and tab functionality. Displays a loading indicator until the plugin data is fetched.
 */
function PluginPage({ pluginId } : Readonly<PluginPageProps>) {
  const [plugin, setPlugin] = useState<PluginVersionInfo>();

  const [searchParams, setSearchParams] = useSearchParams();
  
  useEffect(() => {
    pluginsApi.getLatestVersion({ pluginId: pluginId })
        .then(p => setPlugin(p))
  });
  
  return (plugin === undefined ? <div>Loading...</div> :
      <Stack>
        <Grid2 container spacing={3}>
          <Grid2 size="grow">
            <img src={plugin.icon ? `${apiResourcesPath}/${plugin.icon.storedFilename}` : "Icon128.png"}
                 alt="Plugin Icon"/>
          </Grid2>
          <Grid2 size="grow">
            <h2>{plugin.friendlyName ? plugin.friendlyName : plugin.name}</h2>
            <h4>{plugin.version}</h4>
          </Grid2>
        </Grid2>
        <TabContext value={searchParams.get("currentTab") ?? "readme"}>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <TabList onChange={(_, v) => setSearchParams({currentTab: v})} aria-label="lab API tabs example">
              <Tab label="Readme" value="readme" />
              <Tab label="Dependencies" value="dependencies" />
              <Tab label="Dependencies" value="download" />
            </TabList>
          </Box>
          <TabPanel value="readme"><PluginReadmeDisplay pluginId={pluginId} versionId={plugin.versionId} /></TabPanel>
        </TabContext>
      </Stack>
      
  );
}

export default PluginPage;