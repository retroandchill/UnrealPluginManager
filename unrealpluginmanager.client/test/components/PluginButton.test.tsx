import {render, screen} from '@testing-library/react'
import {expect, test} from 'vitest'
import {PluginButton} from "../../src/components";
import {PluginVersionInfo} from "../../src/api";
import '@testing-library/jest-dom/vitest';
import {v7 as uuid7} from "uuid";

test("Plugin Button Renders", () => {
    let plugin: PluginVersionInfo = {
        pluginId: uuid7(),
    name: "Test Plugin",
    authorName: "Demo",
        versionId: uuid7(),
        version: "2.0.2",
        dependencies: []
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