import {Configuration, PluginsApi, UsersApi} from "@/api";
import {createContext, ReactNode, useContext, useMemo} from "react";
import {useAuth} from "react-oidc-context";

export interface ApiContextType {
  isAuthenticated: boolean;
  pluginsApi: PluginsApi;
  usersApi: UsersApi;
}

const ApiContext = createContext<ApiContextType | undefined>(undefined);

interface ApiProviderProps {
  children?: ReactNode;
}

export function ApiProvider({children}: Readonly<ApiProviderProps>) {
  const {user, isAuthenticated} = useAuth();

  const accessToken = user?.access_token;
  const apiConfig = useMemo(() => {
    return new Configuration({
      basePath: import.meta.env.VITE_API_BASE_URL,
      accessToken: accessToken ? `Bearer ${accessToken}` : undefined,
    })
  }, [accessToken]);

  const apiClients = useMemo(() => {
    return {
      isAuthenticated: isAuthenticated,
      pluginsApi: new PluginsApi(apiConfig),
      usersApi: new UsersApi(apiConfig)
    }
  }, [isAuthenticated, apiConfig]);

  return (
      <ApiContext.Provider value={apiClients}>
        {children}
      </ApiContext.Provider>
  );
}

export function useApi() {
  const context = useContext(ApiContext);

  if (context === undefined) {
    throw new Error('useApi must be used within an ApiProvider');
  }

  return context;

}