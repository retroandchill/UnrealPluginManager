import React from 'react';
import App from "@/App";
import {ApiContextType} from '@/components/providers/ApiProvider';
import {ApiMockRouter} from "../support/helpers";

describe('<App/>', () => {
  const mockPlugin = {
    pluginId: 'test-plugin',
    versionId: '1',
    name: 'Test Plugin',
    friendlyName: 'Friendly Test Plugin',
    version: '1.0.0',
    description: 'A test plugin',
    icon: {
      storedFilename: 'test-icon.png',
      originalFilename: 'test-icon.png',
    },
    dependencies: []
  };

  const mockReadme = "# Test Plugin\nThis is a test readme";

  const commonStubbing = ({pluginsApi}: ApiContextType) => {
    // Stub API calls
    cy.stub(pluginsApi, 'getPluginReadme').resolves(mockReadme);
    cy.stub(pluginsApi, "getLatestVersions").resolves({
      items: [mockPlugin],
      pageNumber: 1,
      pageSize: 10,
      totalPages: 1,
      count: 1,
    });
  }

  it('renders landing page by default', () => {
    cy.mount(
        <App routerType={ApiMockRouter} routerProps={{mocking: commonStubbing}}/>
    );

    // Check header is present
    cy.get('.MuiAppBar-root').should('exist');

    // Check footer is present
    cy.get('footer').should('exist');
  });

  it('renders search page on search route', () => {
    cy.mount(
        <App routerType={ApiMockRouter} routerProps={{mocking: commonStubbing, initialEntries: ['/search?q=test']}}/>
    );

    // Verify search page elements
    cy.contains('Search Results').should('exist');
  });

  it('renders plugin page and handles API calls', () => {
    const stubbing = ({pluginsApi, ...api}: ApiContextType) => {
      commonStubbing({pluginsApi, ...api});
      cy.stub(pluginsApi, 'getLatestVersion').resolves(mockPlugin);
    }
    cy.mount(
        <App routerType={ApiMockRouter} routerProps={{mocking: stubbing, initialEntries: ['/plugin/test-plugin']}}/>
    );

    // Wait for data to load and verify content
    cy.contains('Friendly Test Plugin').should('exist');
    cy.contains('Version 1.0.0').should('exist');

    // Verify tabs are present
    cy.contains('Readme').should('exist');
    cy.contains('Dependencies').should('exist');
    cy.contains('Download').should('exist');
    cy.contains('Versions').should('exist');
  });

  it('shows loading state while fetching plugin data', () => {
    // Create a delayed promise for the API call

    const stubbing = ({pluginsApi, ...api}: ApiContextType) => {
      commonStubbing({pluginsApi, ...api});
      cy.stub(pluginsApi, 'getLatestVersion').returns(
          new Promise(resolve => {
            setTimeout(() => resolve(mockPlugin), 1000);
          })
      );
    }

    cy.mount(
        <App routerType={ApiMockRouter}
             routerProps={{mocking: stubbing, initialEntries: ['/plugin/test-plugin-different']}}/>
    );

    // Check for loading indicator
    cy.get('.MuiCircularProgress-root').should('exist');

    // Eventually shows content
    cy.contains('Friendly Test Plugin').should('exist');
  });

  it('maintains layout structure across routes', () => {
    cy.mount(
        <App routerType={ApiMockRouter} routerProps={{mocking: commonStubbing, initialEntries: ['/search']}}/>
    );

    // Verify header and footer are present
    cy.get('.MuiAppBar-root').should('exist');
    cy.get('footer').should('exist');
  });

  it('handles tab navigation on plugin page', () => {
    cy.mount(
        <App routerType={ApiMockRouter}
             routerProps={{mocking: commonStubbing, initialEntries: ['/plugin/test-plugin']}}/>
    );

    // Wait for content to load
    cy.contains('Friendly Test Plugin').should('exist');

    // Click different tabs and verify content changes
    cy.contains('Dependencies').click();
    cy.contains('Coming soon!').should('exist');

    cy.contains('Download').click();
    cy.contains('Download Options').should('exist');

    cy.contains('Versions').click();
    cy.contains('Versions').should('exist');
  });
});