import {render, screen, waitFor} from '@testing-library/react'
import {afterEach, expect, test, vi} from 'vitest'
import '@testing-library/jest-dom/vitest';
import {Page} from "../src/util";
import App from "../src/App";
import {pluginsApi} from "../src/config/Globals";
import {PluginVersionInfo} from "../src/api";
import {v7 as uuid7} from "uuid";

afterEach(() => {
  vi.clearAllMocks();
})

test("Load the main page", async () => {
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
      .mockImplementation(() => Promise.resolve(plugins));
  render(<App/>);

  const loading = screen.getByText("Loading...");
  expect(loading).toBeInTheDocument();
  await waitFor(() => expect(screen.queryByText('Loading...')).not.toBeInTheDocument());

  const pluginButtons = screen.getAllByText(/(?:Test|Sample|Fake) Plugin/i);
  expect(pluginButtons).toHaveLength(3);
})