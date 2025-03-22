import {getInstallCommand} from '@/util';
import {expect, test} from "vitest";
import {PluginVersionInfo} from "@/api";
import {v7 as uuid7} from "uuid";

test('check command is formatted correctly', () => {
  const testPlugin: PluginVersionInfo = {
    pluginId: uuid7(),
    name: "TestPlugin",
    authorName: "Demo",
    versionId: uuid7(),
    version: "2.0.2",
    dependencies: []
  }

  expect(getInstallCommand(testPlugin)).equals("uepm install TestPlugin");
  expect(getInstallCommand(testPlugin, false)).equals("uepm install TestPlugin");
  expect(getInstallCommand(testPlugin, true)).equals("uepm install TestPlugin --version 2.0.2");
});