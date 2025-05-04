import React from 'react'
import {PluginPageHeader} from "@/components";
import {PluginVersionInfo} from "@/api";

describe('<PluginPageHeader/>', () => {
  const mockPlugin: PluginVersionInfo = {
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

  it('renders plugin information correctly', () => {
    cy.mount(<PluginPageHeader plugin={mockPlugin}/>);

    // Check if the plugin name is displayed
    cy.contains('Friendly Test Plugin');

    // Check if version chip is present
    cy.contains('Version 1.0.0');

    // Check if the icon is rendered
    cy.get('img[alt="Plugin Icon"]').should('exist');

    // Check if installation command section exists
    cy.contains('Installation Command');
  });

  it('falls back to name when friendlyName is not provided', () => {
    const pluginWithoutFriendlyName = {
      ...mockPlugin,
      friendlyName: undefined
    };

    cy.mount(<PluginPageHeader plugin={pluginWithoutFriendlyName}/>);

    cy.contains(pluginWithoutFriendlyName.name);
  });

  it('uses fallback icon when no icon is provided', () => {
    const pluginWithoutIcon = {
      ...mockPlugin,
      icon: undefined
    };

    cy.mount(<PluginPageHeader plugin={pluginWithoutIcon}/>);

    cy.get('img[alt="Plugin Icon"]')
        .should('have.attr', 'src', 'Icon128.png');
  });
});