import React from 'react';
import {PluginSearchResultCard} from "@/components";
import {PluginVersionInfo} from "@/api";

describe('<PluginSearchResultCard/>', () => {
  const mockPlugin: PluginVersionInfo = {
    pluginId: 'test-plugin',
    versionId: '1',
    name: 'Test Plugin',
    friendlyName: 'Friendly Test Plugin',
    version: '1.0.0',
    description: 'This is a test plugin description',
    authorName: 'Test Author',
    icon: {
      storedFilename: 'test-icon.png',
      originalFilename: 'test-icon.png',
    },
    dependencies: []
  };

  it('renders plugin card with all information', () => {
    cy.mount(<PluginSearchResultCard plugin={mockPlugin}/>);

    // Check if plugin name is displayed
    cy.contains('Friendly Test Plugin');

    // Check if version chip is present
    cy.contains('Version 1.0.0');

    // Check if author is displayed
    cy.contains('Test Author');

    // Check if description is present
    cy.contains('This is a test plugin description');

    // Verify the link points to the correct route
    cy.get('a')
        .should('have.attr', 'href', `/plugin/${mockPlugin.pluginId}`);
  });

  it('falls back to name when friendlyName is not provided', () => {
    const pluginWithoutFriendlyName = {
      ...mockPlugin,
      friendlyName: undefined
    };

    cy.mount(<PluginSearchResultCard plugin={pluginWithoutFriendlyName}/>);
    cy.contains(pluginWithoutFriendlyName.name);
  });

  it('displays "Unknown" when author is not provided', () => {
    const pluginWithoutAuthor = {
      ...mockPlugin,
      authorName: undefined
    };

    cy.mount(<PluginSearchResultCard plugin={pluginWithoutAuthor}/>);
    cy.contains('Author:').parent().contains('Unknown');
  });

  it('uses fallback icon when no icon is provided', () => {
    const pluginWithoutIcon = {
      ...mockPlugin,
      icon: undefined
    };

    cy.mount(<PluginSearchResultCard plugin={pluginWithoutIcon}/>);

    cy.get('img[alt="Plugin Icon"]')
        .should('have.attr', 'src', 'Icon128.png');
  });

  it('applies hover effect on card', () => {
    cy.mount(<PluginSearchResultCard plugin={mockPlugin}/>);

    cy.get('.MuiCard-root')
        .should('exist')
        .trigger('mouseover')
        .should('have.css', 'box-shadow')
        .and('not.equal', 'none');
  });
});