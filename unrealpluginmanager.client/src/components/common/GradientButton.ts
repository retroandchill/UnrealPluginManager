import {styled} from '@mui/material/styles';
import {Button} from '@mui/material';
import {CSSProperties} from "react";

/**
 * Represents a color along with its associated percentage value.
 *
 * This interface is typically used in scenarios where a color is paired
 * with a numerical percentage to denote distribution, intensity, or weight.
 *
 * Optional properties:
 * - `color`: The color value, which can be any valid CSS color format (e.g., hexadecimal, rgba, named colors).
 * - `percent`: A numerical value representing the percentage associated with the color.
 */
interface ColorPercent {
  /**
   * Represents the color of the component's text or other elements.
   * Accepts any valid CSS color value, such as named colors, HEX, RGB, HSL, etc.
   * Optional property, default styling may apply if not specified.
   */
  color?: CSSProperties["color"];

  /**
   * Represents a percentage value.
   * This optional property may be used to store or reference a numeric value
   * that indicates a proportion or fraction of a whole, expressed as a percentage.
   * Accepts values in the range of 0 to 100 (inclusive), where 0 represents 0%
   * and 100 represents 100%.
   */
  percent?: number;
}

/**
 * Represents the props required for rendering a gradient button.
 */
interface GradientButtonProps {
  /**
   * Represents the angle in degrees.
   * This optional property specifies a numeric value for an angle, usually
   * utilized in geometrical, trigonometric, or graphical calculations.
   * If not provided, the angle may default to undefined or require explicit
   * assignment depending on its usage.
   */
  angle?: number;

  /**
   * Represents the primary color with an optional color percentage.
   * The value can be used for applying colors in a specific format
   * or for styling purposes, where `ColorPercent` defines the
   * expected format of the color and its associated percentage.
   */
  color1?: ColorPercent;

  /**
   * Represents an optional secondary color with an associated percentage value.
   * Can be used to define gradient transitions or relative intensity in visual elements.
   * The value should conform to the `ColorPercent` type specification.
   */
  color2?: ColorPercent;
}

/**
 * GradientButton is a styled component extending the Button component with additional gradient-related properties.
 * It creates a button with a customizable linear gradient background based on the provided angle and color stops.
 *
 * @property [angle=45] - The angle of the gradient in degrees. Defaults to 45 degrees if not specified.
 * @property [color1] - The first color stop in the gradient.
 * @property [color1.color='#FE6B8B'] - The color value for the first gradient stop. Defaults to '#FE6B8B' if not specified.
 * @property [color1.percent=30] - The percentage position of the first color stop in the gradient. Defaults to 30% if not specified.
 * @property [color2] - The second color stop in the gradient.
 * @property [color2.color='#FF8E53'] - The color value for the second gradient stop. Defaults to '#FF8E53' if not specified.
 * @property [color2.percent=90] - The percentage position of the second color stop in the gradient. Defaults to 90% if not specified.
 */
export const GradientButton = styled(Button)<GradientButtonProps>(({angle, color1, color2}: GradientButtonProps) => ({
  background: `linear-gradient(${angle ?? 45}deg, ${color1?.color ?? '#FE6B8B'} ${color1?.percent ?? 30}%, ${color2?.color ?? '#FF8E53'} ${color2?.percent ?? 90}%)`,
  border: 0,
  borderRadius: 3,
  boxShadow: '0 3px 5px 2px rgba(255, 105, 135, .3)',
  color: 'white',
  height: 48,
  padding: '0 30px',
}));