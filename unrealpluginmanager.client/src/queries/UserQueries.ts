import {useApi} from "@/components";
import {useQuery} from "@tanstack/react-query";
import {UserOverview} from "@/api";
import {DefaultError} from "@tanstack/query-core";
import {QueryExtraOptions} from "@/util";

export function useActiveUserQuery<TError = DefaultError, TData = UserOverview>(options?: QueryExtraOptions<UserOverview, TError, TData>) {
  const {usersApi} = useApi();

  return useQuery({
    queryKey: ['users', 'active'],
    queryFn: () => usersApi.getActiveUser(),
    ...options
  })
}