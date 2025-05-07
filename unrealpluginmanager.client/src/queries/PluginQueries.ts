import {useApi} from "@/components";
import {useQuery} from "@tanstack/react-query";
import {Page, QueryExtraOptions, UpdatableQueryExtraOptions, useUpdatableQuery} from "@/util";
import {PluginVersionInfo} from "@/api";
import type {DefaultError} from "@tanstack/query-core";

export function useLatestPluginVersionQuery<TError = DefaultError, TData = PluginVersionInfo>(pluginId: string, options?: QueryExtraOptions<PluginVersionInfo, TError, TData>) {
  const {pluginsApi} = useApi();

  return useQuery({
    queryKey: ['plugins', pluginId, 'latest'],
    queryFn: () => pluginsApi.getLatestVersion({pluginId}),
    ...options
  });
}

export function usePluginVersionsQuery<TError = DefaultError, TData = Page<PluginVersionInfo>>(searchTerm: string, pageNumber: number = 1, pageSize: number = 25, options?: QueryExtraOptions<Page<PluginVersionInfo>, TError, TData>) {
  const {pluginsApi} = useApi();

  return useQuery({
    queryKey: ['plugins', 'latest', searchTerm, pageNumber, pageSize],
    queryFn: () => pluginsApi.getLatestVersions({
      match: `${searchTerm}`,
      page: pageNumber,
      size: pageSize
    }),
    ...options
  });
}

export function usePluginReadmeQuery<TError extends Error = DefaultError, TData = string>(pluginId: string, versionId: string, options?: UpdatableQueryExtraOptions<string, TError, TData>) {
  const {pluginsApi} = useApi();

  return useUpdatableQuery({
    queryKey: ['plugins', pluginId, versionId, 'readme'],
    queryFn: () => pluginsApi.getPluginReadme({pluginId, versionId}),
    mutationFn: (readme) => pluginsApi.updatePluginReadme({pluginId, versionId, body: readme}),
    ...options
  });
}