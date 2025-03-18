import React from 'react'
import {PluginDisplayGrid} from "@/components";
import {GetLatestVersionsRequest, PluginVersionInfo} from "@/api";
import {v7 as uuid7} from "uuid";
import {mount} from 'cypress/react'
import {Page} from "@/util";
import {pluginsApi} from "@/config/Globals";


describe('<PluginButton />', () => {
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

  function fakeGetLatestVersions(params: GetLatestVersionsRequest) {
    if (params.match != undefined) {
      let filter = params.match.toLowerCase().slice(0, -1);
      let filtered = plugins.items.filter(plugin => plugin.name.toLowerCase().startsWith(filter));
      return Promise.resolve({
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1,
        count: filtered.length,
        items: filtered
      });
    }

    return Promise.resolve(plugins);
  }

  beforeEach(() => {
    // Stub the API request
    cy.stub(pluginsApi, "getLatestVersions").callsFake(fakeGetLatestVersions);

    // Mount the component
    mount(<PluginDisplayGrid onPluginClick={() => {
    }}/>);
  });


  it('loads the plugin grid', () => {
    // Wait for the loading indicator to disappear
    cy.contains("Loading...").should("not.exist");

    // Verify that the plugin buttons are rendered
    cy.get("button").should("have.length", 3);

  })

  it("filters plugins via the search bar", () => {
    // Verify that the search bar is present
    cy.get("input[type='text']").should("exist").as("searchBar");

    // Type "test" into the search bar
    cy.get("@searchBar").type("test");

    // Verify that the grid shows only one plugin after filtering
    cy.get("button")
        .should("have.length", 1)
        .and("contain", "Test Plugin");
  });
})