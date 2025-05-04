import {Box, Button, CircularProgress, Container, Divider, Pagination, PaginationItem, Typography} from "@mui/material";
import {useQuery, useQueryClient} from "@tanstack/react-query";
import {useSearchParams} from "react-router";
import {PluginSearchResultCard} from "@/components/info";
import {useApi} from "@/components";

interface SearchPageProps {
  pageSize?: number;
}

export function SearchPage({pageSize = 25}: Readonly<SearchPageProps>) {
  const [searchParams, setSearchParams] = useSearchParams();
  const queryClient = useQueryClient();
  const {pluginsApi} = useApi();

  const searchTerm = searchParams.get("q") ?? "";
  const pageNumber = parseInt(searchParams.get("page") ?? "1");

  const searchResult = useQuery({
    queryKey: ['search', searchTerm, pageNumber],
    queryFn: () => pluginsApi.getLatestVersions({
      match: `${searchTerm}`,
      page: pageNumber,
      size: pageSize
    })
  }, queryClient);

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