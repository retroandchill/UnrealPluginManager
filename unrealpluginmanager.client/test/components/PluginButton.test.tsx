import {render, screen} from '@testing-library/react'
import {expect, test} from 'vitest'
import {PluginButton} from "../../src/components";
import {PluginOverview} from "../../src/api";
import '@testing-library/jest-dom/vitest';
import {v4 as uuid4} from "uuid";

test("Plugin Button Renders", () => {
  let plugin: PluginOverview = {
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
  }
  render(<PluginButton plugin={plugin} onClick={() => {
  }}/>)

  const title = screen.getByText(/Test Plugin/i);
  expect(title).toBeInTheDocument();

  const version = screen.getAllByText(/latest release|2\.0\.2/i);
  expect(version).toHaveLength(2);

  const author = screen.getAllByText(/author|demo/i);
  expect(author).toHaveLength(2);

})