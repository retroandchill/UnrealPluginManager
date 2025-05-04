import React from 'react';
import {PluginPageSidebar} from "@/components";
import {PluginVersionInfo} from "@/api";

describe('PluginPageSidebar.cy.tsx', () => {
  const mockPlugin: PluginVersionInfo = {
    pluginId: 'test-plugin',
    versionId: '1',
    name: 'Test Plugin',
    version: '1.0.0',
    description: 'A test plugin',
    authorName: 'Test Author',
    authorWebsite: 'https://test-author.com',
    dependencies: []
  };

  it('renders all sections correctly', () => {
    cy.mount(<PluginPageSidebar plugin={mockPlugin}/>);

    // Verify all sections are present
    cy.contains('Weekly Downloads');
    cy.contains('Latest Version');
    cy.contains('Last Updated');
    cy.contains('Author');
    cy.contains('Homepage');

    // Check specific values
    cy.contains('1,234'); // Weekly downloads
    cy.contains('1.0.0'); // Version
    cy.contains('Test Author'); // Author name
  });

  it('handles missing author information correctly', () => {
    const pluginWithoutAuthor = {
      ...mockPlugin,
      authorName: undefined,
      authorWebsite: undefined
    };

    cy.mount(<PluginPageSidebar plugin={pluginWithoutAuthor}/>);

    cy.contains('Unknown'); // Default author name
    cy.contains('Not specified'); // Default website text
  });

  it('renders author website as clickable link', () => {
    cy.mount(<PluginPageSidebar plugin={mockPlugin}/>);

    cy.get('a')
        .should('have.attr', 'href', 'https://test-author.com')
        .and('contain.text', 'https://test-author.com');
  });
});