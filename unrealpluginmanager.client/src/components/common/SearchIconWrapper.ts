import {styled} from '@mui/material/styles';

/**
 * A styled container component used to wrap a search icon.
 *
 * This wrapper is designed to position and center-align the search icon
 * within its parent container. It utilizes theme spacing for padding
 * and ensures the wrapper does not interfere with pointer events.
 *
 * Styles applied:
 * - Padding controlled via theme spacing.
 * - Full height of the parent container.
 * - Positioned absolutely to ensure proper positioning.
 * - Pointer events are disabled.
 * - Flexbox used for centering the child elements.
 */
export const SearchIconWrapper = styled('div')(({theme}) => ({
  padding: theme.spacing(0, 2),
  height: '100%',
  position: 'absolute',
  pointerEvents: 'none',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
}));