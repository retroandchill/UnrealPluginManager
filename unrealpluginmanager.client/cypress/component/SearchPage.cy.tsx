import React from 'react'
import {mount} from 'cypress/react'
import {Page} from "@/util";
import {PluginVersionInfo} from "@/api";
import {v7 as uuid7} from "uuid";
import {pluginsApi} from "@/config/Globals";
import {createMemoryRouter, RouterProvider} from "react-router";
import {SearchPage} from "@/components/pages/SearchPage";


describe('<SearchPage />', () => {
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

  const mountWithRouter = (initialPath: string) => {
    const router = createMemoryRouter(
        [
          {
            path: '/search',
            element: <SearchPage/>,
          },
        ],
        {
          initialEntries: [initialPath],
        }
    );

    return mount(<RouterProvider router={router}/>);
  };

  beforeEach(() => {
    // Stub the API request
    cy.stub(pluginsApi, "getLatestVersions").returns(Promise.resolve(plugins));

    // Mount the component with a route
    mountWithRouter('/search?q=*plugin');
  });

  it('renders', () => {
    cy.contains("Test Plugin").should("exist");
  })
})