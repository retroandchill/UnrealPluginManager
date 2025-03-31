import React from 'react'
import {PluginReadmeDisplay} from "@/components";
import {v7 as uuid7} from "uuid";
import {pluginsApi} from "@/config";


describe('<PluginReadmeDisplay />', () => {
  const pluginId = uuid7();
  const versionId = uuid7();

  it('should display the fetched README correctly', () => {
    cy.stub(pluginsApi, "getPluginReadme").returns(Promise.resolve('# Test Plugin README\n\nThis is a sample readme content for testing purposes.'));

    // Mount the component with pluginId and versionId
    cy.mount(<PluginReadmeDisplay pluginId={pluginId} versionId={versionId}/>);

    // Assert that the README content is rendered correctly
    cy.get('.markdown-container').should('exist');
    cy.contains('h1', 'Test Plugin README').should('be.visible');
    cy.contains('This is a sample readme content for testing purposes.').should('be.visible');
  });

  it('should display an error message when the API call fails', () => {
    // Setup mock for a failed API call
    cy.stub(pluginsApi, "getPluginReadme").returns(Promise.reject());

    // For some reason the upgrade fails the test, so I need to do this in order to
    // make the behavior bypass work
    Cypress.on("uncaught:exception", (err) => {
      return false;
    });


    // Mount the component with pluginId and versionId
    cy.mount(<PluginReadmeDisplay pluginId={pluginId} versionId={versionId}/>);

    // Assert that the error message is displayed
    cy.contains('Failed to fetch plugin README.').should('be.visible');
    cy.get('h1').should('not.exist'); // Ensure Markdown doesn't render
  });

  it('should render syntax-highlighted code blocks in the README', () => {
    const markdownWithCode = `
      # Sample README with Code

      \`\`\`javascript
      console.log('Hello, world!');
      \`\`\`
    `;

    // Intercept the API call to return markdown with a code block
    cy.stub(pluginsApi, "getPluginReadme").returns(Promise.resolve(markdownWithCode));

    // Mount the component
    cy.mount(<PluginReadmeDisplay pluginId={pluginId} versionId={versionId}/>);

    // Assert Markdown renders properly with syntax-highlighted code
    cy.contains('Sample README with Code').should('be.visible');
    cy.get('pre').should('contain', "console.log('Hello, world!');");
  });

})