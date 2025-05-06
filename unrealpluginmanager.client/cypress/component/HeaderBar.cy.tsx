import React from 'react';
import {HeaderBar} from "@/components";
import {BrowserRouter} from "react-router";
import {mountWithApiMock} from "../support/helpers";
import {QueryClient, QueryClientProvider} from '@tanstack/react-query';

describe('<HeaderBar/>', () => {
  beforeEach(() => {
    const queryClient = new QueryClient();
    // Wrap component in BrowserRouter since it uses navigation
    mountWithApiMock(
        <QueryClientProvider client={queryClient}>
          <BrowserRouter>
            <HeaderBar/>
          </BrowserRouter>
        </QueryClientProvider>,
        () => {
        });
  });

  it('renders header bar with all elements', () => {
    // Check if AppBar exists
    cy.get('.MuiAppBar-root').should('exist');

    // Check if Logo exists and is clickable
    cy.get('a[href="/"]')
        .should('exist');

    // Check if SearchBar exists
    cy.get('input').should('exist');

    // Check if Login button exists
    cy.contains('button', 'Login').should('exist');
  });

  it('navigates to search results when search is performed', () => {
    const searchTerm = 'test plugin';

    // Type in search bar and press enter
    cy.get('input')
        .type(`${searchTerm}{enter}`);

    // Verify the URL was updated with search parameters
    cy.url().should('include', `/search?q=${encodeURIComponent(searchTerm)}`);
  });

  it('has logo with correct attributes', () => {
    cy.get('a[href="/"]')
        .should('have.css', 'display', 'flex')
        .should('have.css', 'align-items', 'center');
  });


  it('applies correct styling to toolbar', () => {
    cy.get('.MuiToolbar-root')
        .should('have.css', 'margin-top', '10px')
        .and('have.css', 'margin-bottom', '10px');
  });

  it('has correct layout structure', () => {
    // Check if the flex container exists
    cy.get('.MuiBox-root')
        .should('have.css', 'display', 'flex')
        .and('have.css', 'align-items', 'center');

    // Check if search bar has correct margins
    cy.get('input')
        .parent()
        .parent()
        .should('have.css', 'margin-left', '10px')
        .and('have.css', 'margin-right', '10px');
  });

  it('has primary colored login button', () => {
    cy.get('button')
        .contains('Login')
        .should('have.class', 'MuiButton-containedPrimary');
  });
});