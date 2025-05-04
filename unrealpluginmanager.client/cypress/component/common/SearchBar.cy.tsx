import React from 'react';
import {SearchBar} from "@/components";

describe('<SearchBar />', () => {
  it('should render with default placeholder', () => {
    cy.mount(<SearchBar/>)
    cy.get('input').should('have.attr', 'placeholder', 'Search...')
  });

  it('should render with custom placeholder', () => {
    const customPlaceholder = 'Custom search...'
    cy.mount(<SearchBar placeholder={customPlaceholder}/>)
    cy.get('input').should('have.attr', 'placeholder', customPlaceholder)
  });

  it('should handle input changes', () => {
    const searchText = 'test search'
    cy.mount(<SearchBar/>)
    cy.get('input').type(searchText)
    cy.get('input').should('have.value', searchText)
  });

  it('should trigger search on Enter when text is not empty', () => {
    const searchText = 'test search'
    const onSearch = cy.spy().as('onSearchSpy')
    cy.mount(<SearchBar onSearch={onSearch}/>)

    cy.get('input').type(`${searchText}{enter}`)
    cy.get('@onSearchSpy').should('have.been.calledWith', searchText)
  });

  it('should not trigger search on Enter when text is empty and allowEmptySearch is false', () => {
    const onSearch = cy.spy().as('onSearchSpy')
    cy.mount(<SearchBar onSearch={onSearch} allowEmptySearch={false}/>)

    cy.get('input').type('{enter}')
    cy.get('@onSearchSpy').should('not.have.been.called')
  });

  it('should trigger search on Enter when text is empty and allowEmptySearch is true', () => {
    const onSearch = cy.spy().as('onSearchSpy')
    cy.mount(<SearchBar onSearch={onSearch} allowEmptySearch={true}/>)

    cy.get('input').type('{enter}')
    cy.get('@onSearchSpy').should('have.been.calledWith', '')
  });

  it('should pass custom sx props correctly', () => {
    const customSx = {marginX: '10px'}
    cy.mount(<SearchBar sx={customSx}/>)
    // Verify the custom style is applied to the Search component
    cy.get('[class*="MuiInputBase-root"]').parent().should('have.css', 'margin-left', '10px')
    cy.get('[class*="MuiInputBase-root"]').parent().should('have.css', 'margin-right', '10px')
  });
});