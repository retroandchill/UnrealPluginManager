import {styled} from '@mui/material/styles';
import InputBase from '@mui/material/InputBase';

/**
 * StyledInputBase is a styled version of the Material-UI InputBase component.
 *
 * This component is customized to have a specific color, width, and padding.
 * It also includes responsive behavior based on the screen size using Material-UI's theme breakpoints.
 * The input field dynamically adjusts its width when focused, allowing for a more interactive user experience.
 *
 * Properties of Styling:
 * - Sets the color of the input to inherit from its parent container.
 * - Ensures 100% width for the input field.
 * - Defines specific padding for the input field based on the theme spacing, including space for the search icon.
 * - Contains a responsive width adjustment based on screen size.
 * - On larger screens (as defined by the theme), the input field width expands from 12ch to 20ch upon focus.
 *
 * Dependencies:
 * - Requires Material-UI's InputBase component.
 * - Relies on the Material-UI theme object for spacing, transitions, and breakpoints.
 */
export const StyledInputBase = styled(InputBase)(({theme}) => ({
  color: 'inherit',
  width: '100%',
  '& .MuiInputBase-input': {
    padding: theme.spacing(1, 1, 1, 0),
    // vertical padding + font size from searchIcon
    paddingLeft: `calc(1em + ${theme.spacing(4)})`,
    transition: theme.transitions.create('width'),
    [theme.breakpoints.up('sm')]: {
      width: '12ch',
      '&:focus': {
        width: '20ch',
      },
    },
  },
}));