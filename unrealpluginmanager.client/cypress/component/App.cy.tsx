import React from 'react'
import App from '../../src/App'
import {mount} from 'cypress/react'
import {Page} from "../../src/util";
import {PluginVersionInfo} from "../../src/api";
import {v7 as uuid7} from "uuid";
import {pluginsApi} from "../../src/config/Globals";
import {createMemoryRouter} from "react-router";


describe('<App />', () => {
  const plugins: Page<PluginVersionInfo> = {
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1,
    count: 3,
    items: [
      {
        pluginId: uuid7(),
        name: "Test Plugin",
        authorName: "Demo",
        versionId: uuid7(),
        version: "2.0.2",
        dependencies: []
      },
      {
        pluginId: uuid7(),
        name: "Sample Plugin",
        authorName: "Demo",
        versionId: uuid7(),
        version: "1.0.0",
        dependencies: []
      },
      {
        pluginId: uuid7(),
        name: "Fake Plugin",
        authorName: "Demo",
        versionId: uuid7(),
        version: "1.1.0",
        dependencies: []
      }
    ]
  };

  beforeEach(() => {
    // Stub the API request
    cy.stub(pluginsApi, "getLatestVersions").returns(Promise.resolve(plugins));

    mount(<App routerFactory={createMemoryRouter}/>);

  });

  it('renders', () => {
    cy.contains("Test Plugin").should("exist");
  })
})