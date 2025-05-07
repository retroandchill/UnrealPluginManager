import React, {JSX} from 'react'
import {ApiContextType, UserSigninStatus} from '@/components';
import {mountWithApiMock} from "../../../support/helpers";
import {QueryClient, QueryClientProvider} from '@tanstack/react-query';
import {User, UserManager, WebStorageStateStore} from 'oidc-client-ts';

const testUserManager = new UserManager({
  authority: 'http://test-authority',
  client_id: 'test-client',
  redirect_uri: window.location.origin,
  userStore: new WebStorageStateStore({store: window.sessionStorage}),
  monitorSession: false // Disable session monitoring for tests
});


function mountWithQueryClient(component: JSX.Element, mocking: (api: ApiContextType) => void) {
  const queryClient = new QueryClient();
  mountWithApiMock({
    component: <QueryClientProvider client={queryClient}>
      {component}
    </QueryClientProvider>, userManager: testUserManager, mocking: mocking
  });
}

describe('<UserSigninStatus/>', () => {
  const mockUser = {
    username: 'TestUser',
    id: '1',
    email: 'test@example.com'
  };

  describe('Unauthenticated State', () => {
    beforeEach(() => {
      cy.stub(testUserManager, 'signoutRedirect').as('signoutRedirect');
      cy.stub(testUserManager, 'signinRedirect').as('signinRedirect');

      window.sessionStorage.clear();

    });

    it('should display login button when user is not authenticated', () => {
      mountWithQueryClient(<UserSigninStatus/>, () => {
      });

      cy.get('button').should('contain', 'Login');
    });

    it('should call signinRedirect when login button is clicked', () => {
      mountWithQueryClient(<UserSigninStatus/>, () => {
      });

      cy.get('button').click();
      cy.get('@signinRedirect').should('have.been.called');
    });
  });

  describe('Authenticated State', () => {
    beforeEach(async () => {
      cy.stub(testUserManager, 'signoutRedirect').as('signoutRedirect');
      cy.stub(testUserManager, 'signinRedirect').as('signinRedirect');
      window.sessionStorage.clear();
      const mockUser = new User({
        access_token: "mock_access_token",
        token_type: "Bearer",
        profile: {
          sub: "123",
          name: "Test User",
          email: "test@example.com",
        },
        expires_at: Date.now() + 3600 * 1000, // Token expires in 1 hour
        scope: "openid profile email",
        id_token: "mock_id_token",
      });
      await testUserManager.storeUser(mockUser);
    });

    it('should display user chip when authenticated', () => {
      mountWithQueryClient(<UserSigninStatus/>, ({usersApi}) => {
        console.log(usersApi);
        cy.stub(usersApi, 'getActiveUser').resolves(mockUser);
      });

      cy.get('.MuiChip-root').should('exist');
      cy.get('.MuiChip-label').should('contain', 'TestUser');
      cy.get('.MuiAvatar-root').should('contain', 'T');
    });

    it('should display menu when user chip is clicked', () => {
      mountWithQueryClient(<UserSigninStatus/>, ({usersApi}) => {
        cy.stub(usersApi, 'getActiveUser').resolves(mockUser);
      });

      // Menu should be closed initially
      cy.get('.MuiMenu-paper').should('not.exist');

      // Click the chip to open menu
      cy.get('.MuiChip-root').click();

      // Menu should be visible with logout option
      cy.get('.MuiMenu-paper').should('be.visible');
      cy.get('.MuiMenuItem-root').should('contain', 'Logout');
    });

    it('should call signoutRedirect when logout is clicked', () => {
      mountWithQueryClient(<UserSigninStatus/>, ({usersApi}) => {
        cy.stub(usersApi, 'getActiveUser').resolves(mockUser);
      });

      cy.get('.MuiChip-root').click();
      cy.get('.MuiMenuItem-root').click();
      cy.get('@signoutRedirect').should('have.been.called');
    });

    it('should close menu when clicking outside', () => {
      mountWithQueryClient(<UserSigninStatus/>, ({usersApi}) => {
        cy.stub(usersApi, 'getActiveUser').resolves(mockUser);
      });

      // Open menu
      cy.get('.MuiChip-root').click();
      cy.get('.MuiMenu-paper').should('be.visible');

      // Click outside
      cy.get('body').click(0, 0);
      cy.get('.MuiMenu-paper').should('not.exist');
    });

    it('should show loading state for username when user data is loading', () => {
      mountWithQueryClient(<UserSigninStatus/>, ({usersApi}) => {
        // Don't resolve the promise immediately to simulate loading
        cy.stub(usersApi, 'getActiveUser').returns(new Promise(() => {
        }));
      });

      cy.get('.MuiChip-label').should('contain', 'User');
      cy.get('.MuiAvatar-root').should('contain', 'U');
    });

    it('should handle API errors gracefully', () => {
      mountWithQueryClient(<UserSigninStatus/>, ({usersApi}) => {
        cy.stub(usersApi, 'getActiveUser').rejects(new Error('Failed to fetch user'));
      });

      // Should show default values when API fails
      cy.get('.MuiChip-label').should('contain', 'User');
      cy.get('.MuiAvatar-root').should('contain', 'U');
    });
  });
});