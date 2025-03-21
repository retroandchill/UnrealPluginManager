﻿import {useEffect, useState} from 'react';
import {debounce, Page, PluginCallback} from "@/util";
import {PluginVersionInfo} from "@/api";
import {pluginsApi} from "@/config";
import PluginButton from "./PluginButton.tsx";
import InfiniteScroll from 'react-infinite-scroll-component';
import TextField from "@mui/material/TextField";

/**
 * Interface representing the properties for the PluginGrid component.
 *
 * @property {PluginCallback} [onPluginClick] - Optional callback function triggered when a plugin is clicked.
 */
interface PluginGridProps {
  /**
   * Callback function triggered when a plugin item is clicked.
   * The function is optional and can be used to define custom behavior
   * upon a plugin click event.
   *
   * @type {PluginCallback | undefined}
   */
  onPluginClick?: PluginCallback;
}

function PluginDisplayGrid(props: Readonly<PluginGridProps>) {
    const [plugins, setPlugins] = useState<PluginVersionInfo[]>([]);
    const [lastPage, setLastPage] = useState<Page<PluginVersionInfo> | undefined>(undefined);
  const [searchTerm, setSearchTerm] = useState<string | undefined>(undefined);

  async function updateSearchTerm(newSearchTerm: string) {
      const response = await pluginsApi.getLatestVersions({
      match: newSearchTerm ? `${newSearchTerm}*` : undefined,
      page: 1,
      size: 25
    });
    
    setPlugins(response.items);
    setLastPage(response);
  }

  const debouncedUpdateSearchTerm = debounce(updateSearchTerm, 500);
  
  function onTypeSearchTerm(newSearchTerm: string) {
    setSearchTerm(newSearchTerm);
    debouncedUpdateSearchTerm(newSearchTerm);
  }

  async function populatePluginList() {
      const response = await pluginsApi.getLatestVersions({
      match: searchTerm ? `${searchTerm}*` : undefined,
      page: lastPage ? lastPage.pageNumber + 1 : 1,
      size: 25
    });

    setPlugins(prev => prev.concat(response.items));
    setLastPage(response);
  }
  
  useEffect(() => {
    populatePluginList();
  }, [])

  return <div>
    <div id="search-bar" style={{display: 'flex', justifyContent: 'flex-end', paddingBottom: 10}}>
      <TextField
          id="outlined-basic"
          variant="outlined"
          label="Search"
          onChange={(e) => onTypeSearchTerm(e.target.value)}
      />
    </div>
    <InfiniteScroll
        dataLength={plugins.length}
        next={() => populatePluginList()}
        hasMore={!lastPage || lastPage.pageNumber < lastPage.totalPages}
        loader={<h4>Loading...</h4>}
        scrollableTarget="scrollableDiv"
        endMessage={
          <p style={{textAlign: 'center'}}>
            <b>Yay! You have seen it all</b>
          </p>
        }>

      <table className="table table-striped" aria-labelledby="tableLabel">
        <tbody>
        {plugins.map(plugin => <PluginButton key={plugin.pluginId} plugin={plugin}
                                             onClick={props.onPluginClick}/>)}
        </tbody>
      </table>
    </InfiniteScroll>
  </div>
}

export default PluginDisplayGrid;