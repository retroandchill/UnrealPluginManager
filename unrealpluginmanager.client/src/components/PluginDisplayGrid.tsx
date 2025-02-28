import {Component} from 'react';
import {Page, debounce} from "../util";
import {PluginOverview} from "../api";
import {pluginsApi} from "../config/Globals";
import {PluginButton} from "./PluginButton.tsx";
import InfiniteScroll from 'react-infinite-scroll-component';
import TextField from "@mui/material/TextField";
/**
 * AppState interface represents the state of the application.
 */
interface PluginGridState {
    plugins: PluginOverview[];
    
    lastPage?: Page<PluginOverview>
    searchTerm?: string;
}

export class PluginDisplayGrid extends Component<{}, PluginGridState> {

    /**
     * Constructor for initializing the component with props and setting the initial state.
     *
     * @param {Object} props - The properties passed to the component.
     */
    constructor(props: {}) {
        super(props);
        this.state = {
            plugins: [],
        };
        this.populatePluginList();
    }
    
    render() {
        return <div>
            <TextField
                id="outlined-basic"
                variant="outlined"
                label="Search"
                onChange={(e) => this.debouncedUpdateSearchTerm(e.target.value)}
            />
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
                    {this.state.plugins.map(plugin => <PluginButton key={plugin.id} plugin={plugin} onClick={(_) => {}}/>)}
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
     *
     * @type {Function}
     */
    private debouncedUpdateSearchTerm = debounce(this.updateSearchTerm.bind(this), 500);
    
    private async updateSearchTerm(newSearchTerm: string) {
        const response = await pluginsApi.getPlugins({
            match: newSearchTerm ? `${newSearchTerm}*` : undefined,
            page: 1,
            size: 25
        });
        
        this.setState({
            plugins: response.items,
            lastPage: response,
            searchTerm: this.state.searchTerm,
        });
    }

    private async populatePluginList() {
        const response = await pluginsApi.getPlugins({
            match: this.state.searchTerm ? `${this.state.searchTerm}*` : undefined,
            page: this.state.lastPage ? this.state.lastPage.pageNumber + 1 : 1,
            size: 25
        })
        this.setState({
            plugins: this.state.plugins.concat(response.items),
            lastPage: response
        })
    }
}