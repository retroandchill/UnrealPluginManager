import React, {JSX} from 'react';
import {ApiContextType, ApiProvider, useApi} from "@/components";
import {mount, MountOptions} from 'cypress/react';
import {AuthProvider} from "react-oidc-context";
import {MemoryRouter, MemoryRouterProps} from "react-router";

interface ApiMockProps {
  mocking: (api: ApiContextType) => void;
  children?: JSX.Element;
}

function ApiMock({mocking, children}: ApiMockProps) {
  const api = useApi();
  mocking(api);
  return children;
}

export function mountWithApiMock(component: JSX.Element, mocking: (api: ApiContextType) => void, options?: MountOptions) {
  return mount(<AuthProvider>
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