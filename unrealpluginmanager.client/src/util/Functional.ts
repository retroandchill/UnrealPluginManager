/**
 * Represents a generic function type.
 *
 * @template R - The return type of the function.
 * @template T - A tuple representing the parameter types of the function.
 * @param {...T} args - Represents the parameters accepted by the function.
 * @returns {R} - The result of invoking the function.
 */
export type Func<R, T extends any[]> = (...args: T) => R;