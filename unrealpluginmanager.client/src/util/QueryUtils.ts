import {useMutation, useQuery, useQueryClient, UseQueryOptions, UseQueryResult} from "@tanstack/react-query";
import {useCallback} from "react";
import type {DefaultError, QueryKey} from "@tanstack/query-core";

interface UseQueryWithMutationOptions<TQueryFnData = unknown,
    TError = DefaultError,
    TData = TQueryFnData,
    TVariables = void,
    TQueryKey extends QueryKey = QueryKey> extends UseQueryOptions<TQueryFnData, TError, TData, TQueryKey> {
  mutationFn: (variables: TVariables) => Promise<TQueryFnData>;
  onMutationSuccess?: (data: TData) => void;
  onMutationError?: (error: TError) => void;
}

interface UseQueryWithMutationResult<TData, TVariables, TError extends Error> extends Omit<UseQueryResult<TData>, 'data'> {
  data: TData | undefined;
  updateData: (variables: TVariables) => Promise<void>;
  isUpdating: boolean;
  updateError: TError | null;
}

export function useUpdatableQuery<TQueryFnData, TData = TQueryFnData, TVariables = TQueryFnData, TError extends Error = DefaultError, TQueryKey extends QueryKey = QueryKey>(
    {
      queryKey,
      queryFn,
      select,
      mutationFn,
      ...options
    }: UseQueryWithMutationOptions<TQueryFnData, TError, TData, TVariables, TQueryKey>): UseQueryWithMutationResult<TData, TVariables, TError> {
  const queryClient = useQueryClient();


  // Set up the query
  const {data, ...query} = useQuery({
    queryKey,
    queryFn,
    select,
    ...options
  });

  // Set up the mutation
  const mutation = useMutation({
    mutationFn,
    onSuccess: (newData) => {
      // Update the query cache with the new data
      queryClient.setQueryData<TQueryFnData>(queryKey, newData);
      options.onMutationSuccess?.(select?.(newData) ?? newData as unknown as TData);
    },
    onError: (error: TError) => {
      options.onMutationError?.(error);
    }
  });

  // Wrapper function for mutation
  const updateData = useCallback(async (variables: TVariables) => {
    await mutation.mutateAsync(variables);
  }, [mutation]);

  return {
    ...query,
    data,
    updateData,
    isUpdating: mutation.isPending,
    updateError: mutation.error
  };
}
