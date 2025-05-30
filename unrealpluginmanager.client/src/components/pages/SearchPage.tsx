﻿import {Box, Button, CircularProgress, Container, Divider, Pagination, PaginationItem, Typography} from "@mui/material";
import {useSearchParams} from "react-router";
import {PluginSearchResultCard} from "@/components/info";
import {usePluginVersionsQuery} from "@/queries";

interface SearchPageProps {
  pageSize?: number;
}

export function SearchPage({pageSize = 25}: Readonly<SearchPageProps>) {
  const [searchParams, setSearchParams] = useSearchParams();

  const searchTerm = searchParams.get("q") ?? "";
  const pageNumber = parseInt(searchParams.get("page") ?? "1");

  const searchResult = usePluginVersionsQuery(searchTerm, pageNumber, pageSize);

  if (searchResult.data === undefined) {
    return (
        <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
          <CircularProgress/>
        </Box>
    );
  }

  if (searchResult.data.items.length === 0) {
    return (
        <Container maxWidth="lg">
          <Box display="flex" justifyContent="center" alignItems="center" marginTop={4}>
            <Typography variant="h4" component="div">
              No results found for "<strong>{searchTerm}</strong>"
            </Typography>
          </Box>
        </Container>
    );
  }

  return (
      <Container maxWidth="lg">
        <Box marginY={2}>
          <Typography variant="h2" component="div">
            Search Results
          </Typography>
          <Divider/>
          {searchResult.data.items.map(plugin => <PluginSearchResultCard key={plugin.pluginId} plugin={plugin}/>)}
          <Pagination
              page={pageNumber}
              count={searchResult.data.totalPages}
              onChange={(_, v) => setSearchParams({q: searchTerm, page: `${v}`})}
              renderItem={(item) => (
                  <PaginationItem
                      component={Button}
                      {...item}
                  />
              )}
          />
        </Box>
      </Container>
  );
}