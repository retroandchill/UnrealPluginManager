import {render, screen, waitFor} from '@testing-library/react'
import {afterEach, expect, test, vi} from 'vitest'
import '@testing-library/jest-dom/vitest';
import {Page} from "../../src/util";
import App from "../../src/App";
import {pluginsApi} from "../../src/config/Globals";
import {PluginVersionInfo} from "../../src/api";
import {v7 as uuid7} from "uuid";

afterEach(() => {
  vi.clearAllMocks();
})

test("Load and navigate the plugin grid", async () => {
    let plugins: Page<PluginVersionInfo> = {
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

    vi.spyOn(pluginsApi, 'getLatestVersions')
      .mockImplementation((params) => {
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
      });
  render(<App/>);

  const loading = screen.getByText("Loading...");
  expect(loading).toBeInTheDocument();
  await waitFor(() => expect(screen.queryByText('Loading...')).not.toBeInTheDocument());

  const pluginButtons = screen.getAllByText(/(?:Test|Sample|Fake) Plugin/i);
  expect(pluginButtons).toHaveLength(3);

  const searchBar = screen.getByRole('textbox');
  expect(searchBar).toBeInTheDocument()

  const valueSetter = Object.getOwnPropertyDescriptor(searchBar, 'value').set;
  const prototype = Object.getPrototypeOf(searchBar);
  const prototypeValueSetter = Object.getOwnPropertyDescriptor(prototype, 'value').set;
  if (valueSetter && valueSetter !== prototypeValueSetter) {
    prototypeValueSetter.call(searchBar, 'test');
  } else {
    valueSetter.call(searchBar, 'test');
  }
  searchBar.dispatchEvent(new Event('input', {bubbles: true}));

  await waitFor(() => {
    const pluginButtons = screen.getAllByText(/(?:Test|Sample|Fake) Plugin/i);
    return expect(pluginButtons).toHaveLength(1);
  });

  const filteredPluginButtons = screen.getAllByText(/(?:Test|Sample|Fake) Plugin/i);
  expect(filteredPluginButtons).toHaveLength(1);
})