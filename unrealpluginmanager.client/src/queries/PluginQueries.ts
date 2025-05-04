import {useApi} from "@/components";
import {useQuery} from "@tanstack/react-query";
import {useUpdatableQuery} from "@/util";

export function useLatestPluginVersionQuery(pluginId: string) {
  const {pluginsApi} = useApi();

  return useQuery({
    queryKey: ['plugins', pluginId, 'latest'],
    queryFn: () => pluginsApi.getLatestVersion({pluginId})
  });
}

export function usePluginVersionsQuery(searchTerm: string, pageNumber: number = 1, pageSize: number = 25) {
  const {pluginsApi} = useApi();

  return useQuery({
    queryKey: ['plugins', 'latest', searchTerm, pageNumber, pageSize],
    queryFn: () => pluginsApi.getLatestVersions({
      match: `${searchTerm}`,
      page: pageNumber,
      size: pageSize
    })
  });
}

export function usePluginReadmeQuery(pluginId: string, versionId: string) {
  const {pluginsApi} = useApi();

  return useUpdatableQuery({
    queryKey: ['plugins', pluginId, versionId, 'readme'],
    queryFn: () => pluginsApi.getPluginReadme({pluginId, versionId}),
    mutationFn: (readme) => pluginsApi.updatePluginReadme({pluginId, versionId, body: readme})
  });
}