import {render, screen, waitFor} from '@testing-library/react'
import {afterEach, beforeAll, expect, test, vi} from 'vitest'
import '@testing-library/jest-dom/vitest';
import {Page} from "../src/util";
import App from "../src/App";
import {pluginsApi} from "../src/config/Globals";
import {PluginOverview} from "../src/api";
import {v4 as uuid4} from "uuid";

afterEach(() => {
  vi.clearAllMocks();
})

test("Load the main page", async () => {
  let plugins: Page<PluginOverview> = {
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1,
    count: 3,
    items: [
      {
        id: uuid4(),
        name: "Test Plugin",
        authorName: "Demo",
        versions: [
          {
            id: uuid4(),
            version: "1.0.0"
          },
          {
            id: uuid4(),
            version: "1.0.1"
          },
          {
            id: uuid4(),
            version: "2.0.2"
          }
        ]
      },
      {
        id: uuid4(),
        name: "Sample Plugin",
        authorName: "Demo",
        versions: [
          {
            id: uuid4(),
            version: "1.0.0"
          }
        ]
      },
      {
        id: uuid4(),
        name: "Fake Plugin",
        authorName: "Demo",
        versions: [
          {
            id: uuid4(),
            version: "1.0.0"
          },
          {
            id: uuid4(),
            version: "1.1.0"
          }
        ]
      }
    ]
  };

  vi.spyOn(pluginsApi, 'getPlugins')
      .mockImplementation(() => Promise.resolve(plugins));
  render(<App/>);

  const loading = screen.getByText("Loading...");
  expect(loading).toBeInTheDocument();
  await waitFor(() => expect(screen.queryByText('Loading...')).not.toBeInTheDocument());

  const pluginButtons = screen.getAllByText(/(?:Test|Sample|Fake) Plugin/i);
  expect(pluginButtons).toHaveLength(3);
})