import React from 'react'
import {LandingPage} from "@/components"
import {createTheme, ThemeProvider} from '@mui/material'

describe('<LandingPage />', () => {
  beforeEach(() => {
    // Create a basic theme for testing
    const theme = createTheme()

    // Mount the component with required theme provider
    cy.mount(
        <ThemeProvider theme={theme}>
          <LandingPage/>
        </ThemeProvider>
    )
  })

  it('should render the hero section correctly', () => {
    // Check main heading
    cy.contains('h1', 'Supercharge Your Unreal Engine Development').should('be.visible')

    // Verify the presence of the "Get Started" button
    cy.contains('button', 'Get Started').should('be.visible')
  })

  it('should display all feature cards', () => {
    // Check features section heading
    cy.contains('h2', 'Features').should('be.visible')

    // Verify all 4 feature cards are present
    cy.get('[data-testid="CloudDownloadIcon"]').should('exist')
    cy.get('[data-testid="ExtensionIcon"]').should('exist')
    cy.get('[data-testid="SpeedIcon"]').should('exist')
    cy.get('[data-testid="SecurityIcon"]').should('exist')

    // Verify feature titles are present
    cy.contains('Easy Downloads').should('be.visible')
    cy.contains('Plugin Management').should('be.visible')
    cy.contains('Fast Installation').should('be.visible')
    cy.contains('Secure Downloads').should('be.visible')
  })

  it('should render the call-to-action section', () => {
    // Check CTA heading
    cy.contains('h2', 'Ready to streamline your Unreal Engine development?').should('be.visible')

    // Verify download button
    cy.contains('button', 'Download Now').should('be.visible')
  })

  it('should have proper layout structure', () => {
    // Check main container exists
    cy.get('.MuiContainer-root').should('exist')

    // Verify grid structure
    cy.get('.MuiGrid-container').should('exist')

    // Check feature cards layout
    cy.get('.MuiCard-root').should('have.length', 4)
  })
})