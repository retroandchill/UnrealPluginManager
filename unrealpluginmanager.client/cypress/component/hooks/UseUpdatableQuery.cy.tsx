// useUpdatableQuery.cy.tsx
import {QueryClient, QueryClientProvider} from '@tanstack/react-query';
import {useUpdatableQuery} from '@/util';
import React, {FC} from 'react';

interface TestComponentProps {
  initialData: string;
  shouldFail?: boolean;
  onMutationError?: (error: Error) => void;
}

const TestComponent: FC<TestComponentProps> = ({initialData, shouldFail = false, onMutationError}) => {
  const {data, updateData, isUpdating, updateError} = useUpdatableQuery({
    queryKey: ['test'],
    queryFn: async () => initialData,
    mutationFn: async (newValue: string) => {
      if (shouldFail) {
        throw new Error('Update failed');
      }
      await new Promise(resolve => setTimeout(resolve, 1000));
      return newValue;
    },
    onMutationError,
  });

  return (
      <div>
        <div data-testid="data">{data}</div>
        <div data-testid="updating">{String(isUpdating)}</div>
        <div data-testid="error">{updateError?.message || 'no error'}</div>
        <button data-testid="update-button"
                onClick={() => {
                  updateData('updated value').catch(() => {
                    // Error handled by hook
                  });
                }}
        >
          Update
        </button>
      </div>
  );
};

describe('useUpdatableQuery', () => {
  beforeEach(() => {
    // Create a fresh QueryClient for each test
    const queryClient = new QueryClient({
      defaultOptions: {
        queries: {retry: false},
        mutations: {retry: false},
      },
    });

    // Mount the component wrapped in necessary providers
    cy.mount(
        <QueryClientProvider client={queryClient}>
          <TestComponent initialData="initial value"/>
        </QueryClientProvider>
    );
  });

  it('should fetch initial data and update cache on mutation', () => {
    cy.get('[data-testid="data"]').should('have.text', 'initial value');
    cy.get('[data-testid="updating"]').should('have.text', 'false');

    // Trigger update
    cy.get('[data-testid="update-button"]').click();

    // Verify updating state
    cy.get('[data-testid="updating"]').should('have.text', 'true');

    // Verify final state after update
    cy.get('[data-testid="data"]').should('have.text', 'updated value');
    cy.get('[data-testid="updating"]').should('have.text', 'false');
  });

  it('should handle mutation errors', () => {
    const onMutationError = cy.spy().as('errorHandler');

    const queryClient = new QueryClient({
      defaultOptions: {
        queries: {retry: false},
        mutations: {retry: false},
      },
    });

    cy.mount(
        <QueryClientProvider client={queryClient}>
          <TestComponent
              initialData="initial value"
              shouldFail={true}
              onMutationError={onMutationError}
          />
        </QueryClientProvider>
    );

    // Check initial state
    cy.get('[data-testid="data"]').should('have.text', 'initial value');

    // Trigger error
    cy.get('[data-testid="update-button"]').click();

    // Verify error state
    cy.get('[data-testid="error"]').should('have.text', 'Update failed');
    cy.get('[data-testid="updating"]').should('have.text', 'false');
    cy.get('[data-testid="data"]').should('have.text', 'initial value');

    // Verify error handler was called
    cy.get('@errorHandler').should('have.been.calledOnce');
  });
});