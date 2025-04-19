import {alpha, styled} from '@mui/material/styles';

/**
 * A styled `div` component used for creating a search container with specific styles.
 * It adjusts its appearance based on the theme's properties, offering a customizable
 * look that changes on hover or when responsive breakpoints are met.
 *
 * Properties:
 * - `position`: Sets the element's position as 'relative'.
 * - `borderRadius`: Applies the theme's border radius to the container.
 * - `backgroundColor`: Semi-transparent white by default, becomes more opaque on hover.
 * - `marginLeft`: Adjusts left margin to zero for smaller viewports, with a small margin for larger viewports.
 * - `width`: Occupies full width by default and changes to 'auto' on larger screens using theme breakpoints.
 *
 * Theme Dependencies:
 * - The `theme.shape.borderRadius` is used for border radius.
 * - The `theme.palette.common.white` is used for background color.
 * - The `theme.spacing` method is used for spacing adjustments on larger breakpoints.
 * - The `theme.breakpoints.up` method is utilized for responsive styling.
 */
export const Search = styled('div')(({theme}) => ({
  position: 'relative',
  borderRadius: theme.shape.borderRadius,
  backgroundColor: alpha(theme.palette.common.white, 0.15),
  '&:hover': {
    backgroundColor: alpha(theme.palette.common.white, 0.25),
  },
  marginLeft: 0,
  width: '100%',
  [theme.breakpoints.up('sm')]: {
    marginLeft: theme.spacing(1),
    width: 'auto',
  },
}));