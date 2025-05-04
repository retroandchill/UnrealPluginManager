import {ThemeOptions} from '@mui/material/styles';

/**
 * Represents the primary color used in the application.
 * This color is typically employed as the base color
 * for themes, branding, or prominent UI elements.
 * The value is specified in hexadecimal color code format.
 */
export const primaryColor = '#3f51b5';
/**
 * Represents the secondary color used in the application.
 * The value is a hexadecimal color code.
 */
export const secondaryColor = '#f50057';

/**
 * A configuration object defining the theme for the application.
 */
export const theme: ThemeOptions = {
  palette: {
    mode: 'dark',
    primary: {
      main: primaryColor,
    },
    secondary: {
      main: secondaryColor,
    }
  },
  spacing: 8,
  shape: {
    borderRadius: 4,
  }
};

export const linkStyle = {
  textDecoration: 'none',
  '&:hover': {textDecoration: 'underline'},
};