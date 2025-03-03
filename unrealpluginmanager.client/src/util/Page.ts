/**
 * Represents paginated data for a collection of items.
 * This interface is generic and allows specifying the type of items it contains.
 */
export interface Page<T> {
  /**
   * The current page number of the result set.
   * @type {number}
   * @memberof Page
   */
  pageNumber: number;

  /**
   * The total number of pages in the result set.
   * @type {number}
   * @memberof Page
   */
  totalPages: number;

  /**
   * The number of items on each page.
   * @type {number}
   * @memberof Page
   */
  pageSize: number;

  /**
   * The total number of items available in the result set.
   * @type {number}
   * @memberof Page
   */
  count: number;

  /**
   * An array of items on the current page.
   * @type {Array<PluginOverview>}
   * @memberof Page
   */
  items: Array<T>;
}