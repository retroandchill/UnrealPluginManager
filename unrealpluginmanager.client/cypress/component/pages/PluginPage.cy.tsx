﻿import React from 'react';
import {createMemoryRouter, RouterProvider} from 'react-router';
import {QueryClient, QueryClientProvider,} from '@tanstack/react-query'
import {PluginPage} from "@/components";
import {mountWithApiMock} from "../../support/helpers";

describe('<PluginPage />', () => {
  // Mock API data
  const mockPluginData = {
    id: '123',
    friendlyName: 'Plugin Friendly Name',
    name: 'plugin-name',
    version: '1.0.0',
    icon: {
      storedFilename: 'icon-stored-filename.png',
    },
    versionId: '456',
  };

  const queryClient = new QueryClient();

  const mountWithRouter = (initialPath: string) => {
    const router = createMemoryRouter(
        [
          {
            path: '/plugins/:id',
            element: <PluginPage/>,
          },
        ],
        {
          initialEntries: [initialPath],
        }
    );

    return mountWithApiMock({
      component: <QueryClientProvider client={queryClient}>
      <RouterProvider router={router}/>
      </QueryClientProvider>, mocking: ({pluginsApi}) => {
      cy.stub(pluginsApi, 'getLatestVersion').resolves(mockPluginData);
      cy.stub(pluginsApi, 'getPluginReadme').resolves("Readme Content");
      }
    });
  };

  it('displays a loading indicator while fetching plugin data', () => {
    // Mount the component with a route
    mountWithRouter('/plugins/123');

    // Validate loading state
    cy.contains('Readme Content').should('be.visible');
  });

  it('renders plugin details correctly', () => {
    // Mount the component
    mountWithRouter('/plugins/123');

    // Wait for the mock data
    cy.contains('Loading...').should('not.exist');

    // Validate plugin details
    cy.get('img')
        .should('have.attr', 'src')
        .and('include', 'icon-stored-filename.png');
    cy.contains('Plugin Friendly Name').should('be.visible');
    cy.contains('1.0.0').should('be.visible');
    cy.contains('uepm install plugin-name').should('be.visible');
  });

  it('renders and switches tabs correctly', () => {
    // Mount the component
    mountWithRouter('/plugins/123');

    // Wait for the mock data
    cy.contains('Loading...').should('not.exist');

    // Check default tab (Readme is active)
    cy.contains('Readme Content').should('be.visible');
    cy.contains('Coming soon!').should('not.exist');

    // Switch to Dependencies tab
    cy.contains('Dependencies').click();
    cy.contains('Readme Content').should('not.exist');
    cy.contains('Coming soon!').should('be.visible');

    // Switch to Download tab
    cy.contains('Download').click();
    cy.contains('Coming soon!').should('be.visible');
  });
});
