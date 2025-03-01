import {Component} from 'react';
import {Page, debounce} from "../util";
import {PluginOverview} from "../api";
import {pluginsApi} from "../config/Globals";
import {PluginButton, PluginCallback} from "./PluginButton.tsx";
import InfiniteScroll from 'react-infinite-scroll-component';
import TextField from "@mui/material/TextField";

/**
 * Interface representing the properties for the PluginGrid component.
 *
 * @property {PluginCallback} [onPluginClick] - Optional callback function triggered when a plugin is clicked.
 */
interface PluginGridProps {
    /**
     * Callback function triggered when a plugin item is clicked.
     * The function is optional and can be used to define custom behavior
     * upon a plugin click event.
     *
     * @type {PluginCallback | undefined}
     */
    onPluginClick?: PluginCallback;
}

/**
 * Represents the state of a plugin grid, including information about plugins,
 * paging details, and a search term used for filtering.
 */
interface PluginGridState {
    /**
     * Represents an array of PluginOverview objects, where each object contains
     * detailed information about a specific plugin. This variable is typically
     * used to manage and access plugins loaded into the application.
     *
     * @type {PluginOverview[]}
     */
    plugins: PluginOverview[];

    /**
     * Represents the last page of a paginated collection containing PluginOverview objects.
     *
     * This optional variable can hold an instance of a `Page` object, which includes details
     * about the last page in a series of paged data results, such as items, metadata, and navigation information.
     */
    lastPage?: Page<PluginOverview>
    
    /**
     * Represents the term or phrase used for searching within a dataset or collection.
     * Typically utilized in filtering or querying operations to match relevant results.
     * This property is optional and may not always be provided.
     */
    searchTerm?: string;
}

/**
 * Class representing a grid display for plugins with infinite scrolling and search functionality.
 *
 * React component that displays a list of plugins in a grid-like table. Supports searching
 * and infinite scrolling features, enabling efficient loading and filtering of plugin data.
 *
 * Extends the base `Component` class from React and manages states for plugin data,
 * pagination, and search term.
 *
 * Methods included in the class:
 * - Constructor: Initializes the component and its state.
 * - componentDidMount: Handles any setup required after the component is mounted on the DOM.
 * - render: Renders the component's JSX based on the current state and props.
 * - debouncedUpdateSearchTerm: Debounced function to handle search term updates.
 * - updateSearchTerm: Fetches filtered plugins based on the provided search term.
 * - populatePluginList: Loads additional plugins for infinite scrolling.
 *
 * State:
 * - plugins: An array of plugins currently displayed in the grid.
 * - lastPage: Information about the pagination state of the loaded data.
 * - searchTerm: The current search term entered in the search bar.
 *
 * Props:
 * - onPluginClick: Callback function triggered when a plugin button is clicked.
 */
export class PluginDisplayGrid extends Component<PluginGridProps, PluginGridState> {
    
    private initialLoad = false;

    /**
     * Constructor for initializing the component with props and setting the initial state.
     *
     * @param {Object} props - The properties passed to the component.
     */
    constructor(props: PluginGridProps = {}) {
        super(props);
        this.state = {
            plugins: [],
        };
        
    }
    
    componentDidMount() {
        if (!this.initialLoad) {
            this.initialLoad = true;
            this.populatePluginList();
        }
    }
    
    render() {
        return <div>
            <div id="search-bar" style={{display: 'flex', justifyContent: 'flex-end', paddingBottom: 10}}>
                <TextField
                    id="outlined-basic"
                    variant="outlined"
                    label="Search"
                    onChange={(e) => this.debouncedUpdateSearchTerm(e.target.value)}
                />
            </div>
            <InfiniteScroll
                dataLength={this.state.plugins.length}
                next={() => this.populatePluginList()}
                hasMore={!this.state.lastPage || this.state.lastPage.pageNumber < this.state.lastPage.totalPages}
                loader={<h4>Loading...</h4>}
                scrollableTarget="scrollableDiv"
                endMessage={
                    <p style={{textAlign: 'center'}}>
                        <b>Yay! You have seen it all</b>
                    </p>
                }>

                <table className="table table-striped" aria-labelledby="tableLabel">
                    <tbody>
                    {this.state.plugins.map(plugin => <PluginButton key={plugin.id} plugin={plugin} 
                                                                    onClick={this.props.onPluginClick}/>)}
                    </tbody>
                </table>
            </InfiniteScroll>
        </div>
    }
    
    /**
     * A debounced version of the `updateSearchTerm` method that delays its execution
     * until 500 milliseconds have passed since the last invocation. This helps to
     * limit the frequency of function calls, particularly useful in scenarios where
     * the `updateSearchTerm` method is triggered by user input events, such as typing
     * in a search field. The debounced function ensures better performance by reducing
     * the number of executions, improving application efficiency.
     *
     * The function is bound to the current context to ensure it maintains access to
     * the instance's properties and methods.
     */
    private readonly debouncedUpdateSearchTerm = debounce(this.updateSearchTerm.bind(this), 500);
    
    private async updateSearchTerm(newSearchTerm: string) {
        const response = await pluginsApi.getPlugins({
            match: newSearchTerm ? `${newSearchTerm}*` : undefined,
            page: 1,
            size: 25
        });
        
        this.setState(prev => ({
            plugins: response.items,
            lastPage: response,
            searchTerm: prev.searchTerm,
        }));
    }

    private async populatePluginList() {
        const response = await pluginsApi.getPlugins({
            match: this.state.searchTerm ? `${this.state.searchTerm}*` : undefined,
            page: this.state.lastPage ? this.state.lastPage.pageNumber + 1 : 1,
            size: 25
        });
        this.setState(prev => ({
            plugins: prev.plugins.concat(response.items),
            lastPage: response
        }));
    }
}