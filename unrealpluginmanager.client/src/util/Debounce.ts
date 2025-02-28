import {Func} from "./Functional.ts";

/**
 * Creates a debounced function that delays the invocation of the provided callback
 * until after the specified delay time has elapsed since the last time the debounced
 * function was invoked.
 *
 * @template T - The type of arguments that the callback function accepts.
 * @param {(...args: T) => void} callback - The function to be debounced.
 * @param {number} delay - The number of milliseconds to delay execution of the callback.
 * @returns {(...args: T) => void} - A new debounced function that, when invoked, will
 * delay the execution of the callback until the specified delay time has passed since
 * the last call.
 */
export const debounce = <T extends any[]>(callback: Func<void, T>, delay: number): Func<void, T> => {
    let timer: NodeJS.Timeout | undefined;
    return (...args: T) => {
        clearTimeout(timer)
        timer = setTimeout(() => callback(...args), delay)
    }
}