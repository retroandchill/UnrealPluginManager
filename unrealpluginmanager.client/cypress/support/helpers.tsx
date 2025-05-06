import React, {JSX} from 'react';
import {ApiContextType, ApiProvider, useApi} from "@/components";
import {mount, MountOptions} from 'cypress/react';
import {AuthProvider} from "react-oidc-context";
import {MemoryRouter, MemoryRouterProps} from "react-router";
import {UserManager} from 'oidc-client-ts';

interface ApiMockProps {
  mocking: (api: ApiContextType) => void;
  children?: JSX.Element;
}

function ApiMock({mocking, children}: ApiMockProps) {
  const api = useApi();
  mocking(api);
  return children;
}

interface MountWithApiMockParams {
  component: JSX.Element;
  userManager?: UserManager;
  mocking: (api: ApiContextType) => void;
  options?: MountOptions;
}

export function mountWithApiMock({component, userManager, mocking, options}: MountWithApiMockParams) {
  return mount(<AuthProvider userManager={userManager}>
    <ApiProvider>
      <ApiMock mocking={mocking}>
        {component}
      </ApiMock>
    </ApiProvider>
  </AuthProvider>, options);
}

type ApiMockRouterProps = ApiMockProps & MemoryRouterProps;

export function ApiMockRouter({mocking, children, ...props}: ApiMockRouterProps) {
  return (
      <ApiMock mocking={mocking}>
        <MemoryRouter {...props}>
          {children}
        </MemoryRouter>
      </ApiMock>
  )
}