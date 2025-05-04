import React from 'react';
import {Footer} from "@/components";

describe('Footer.cy.tsx', () => {
  beforeEach(() => {
    cy.mount(<Footer/>);
  });

  it('renders all footer sections', () => {
    // Check section headings
    cy.contains('Support');
    cy.contains('Organization');
    cy.contains('Terms & Policies');
    cy.contains('Connect With Us');
  });

  it('displays all support links', () => {
    const supportLinks = ['Documentation', 'FAQs', 'Community Forums', 'Contact Support'];
    supportLinks.forEach(link => {
      cy.contains(link).should('exist');
    });
  });

  it('displays all organization links', () => {
    const companyLinks = ['About Us', 'Blog', 'Contact'];
    companyLinks.forEach(link => {
      cy.contains(link).should('exist');
    });
  });

  it('displays all legal links', () => {
    const legalLinks = ['Terms of Service', 'Privacy Policy', 'Cookie Policy', 'License Agreement'];
    legalLinks.forEach(link => {
      cy.contains(link).should('exist');
    });
  });

  it('renders social media section with GitHub link', () => {
    cy.get('a[href="https://github.com/retroandchill/UnrealPluginManager"]')
        .should('exist')
        .and('have.attr', 'target', '_blank')
        .and('have.attr', 'rel', 'noopener noreferrer');
  });

  it('displays correct copyright information', () => {
    const currentYear = new Date().getFullYear();
    cy.contains(`© ${currentYear} UE Package Manager. All rights reserved.`);
  });

  it('shows community message', () => {
    cy.contains('Made with ♥️ for the Unreal Engine Community');
  });

  it('applies correct styling to footer container', () => {
    cy.get('footer')
        .should('have.css', 'background-color')
        .and('not.equal', 'rgba(0, 0, 0, 0)');

    cy.get('footer')
        .should('have.css', 'color', 'rgb(255, 255, 255)');
  });

  it('has proper layout structure', () => {
    // Check Grid container exists
    cy.get('.MuiGrid-container').should('exist');

    // Check divider styling
    cy.get('.MuiDivider-root')
        .should('exist')
        .and('have.css', 'margin-top', '32px')
        .and('have.css', 'margin-bottom', '32px');
  });

  it('has proper bottom section layout', () => {
    cy.get('.MuiBox-root')
        .last()
        .should('have.css', 'display', 'flex')
        .and('have.css', 'justify-content', 'space-between');
  });
});