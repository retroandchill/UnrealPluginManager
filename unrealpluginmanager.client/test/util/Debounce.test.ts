import {debounce} from '@/util';
import {expect, test} from "vitest";

test('debouncer only fires once', () => {
  let callCount = 0;

  const debouncedFunction = debounce(() => {
    callCount++;
  }, 300);

  debouncedFunction(); // Invoke the debounced function
  debouncedFunction(); // Rapid consecutive call
  debouncedFunction(); // Another rapid consecutive call

  // Use a timer to check if the debounced function was called only once after 300ms
  return new Promise((resolve) => {
    setTimeout(() => {
      expect(callCount).toBe(1); // Ensure the function was only called once
      resolve(null);
    }, 500); // Wait longer than the debounce delay to ensure the function has been invoked
  });
});