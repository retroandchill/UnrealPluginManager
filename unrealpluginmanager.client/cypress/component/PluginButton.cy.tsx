import React from 'react'
import {PluginButton} from "../../src/components";
import {PluginVersionInfo} from "../../src/api";
import {v7 as uuid7} from "uuid";
import {mount} from 'cypress/react'


describe('<PluginButton />', () => {
  it('renders', () => {
    let plugin: PluginVersionInfo = {
      pluginId: uuid7(),
      name: "Test Plugin",
      authorName: "Demo",
      versionId: uuid7(),
      version: "2.0.2",
      dependencies: []
    };
    mount(<PluginButton plugin={plugin} onClick={() => {
    }}/>)
    cy.get('button').should('exist');
    cy.get('button').should('contain', 'Test Plugin');
    cy.get('button').should('contain', '2.0.2');
    cy.get('button').should('contain', 'Demo');
  })
})