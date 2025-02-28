import {Component} from 'react';
import {Page} from "../util";
import {PluginOverview} from "../api";
import {pluginsApi} from "../config/Globals";
import {PluginButton} from "./PluginButton.tsx";
import InfiniteScroll from 'react-infinite-scroll-component';
/**
 * AppState interface represents the state of the application.
 */
interface PluginGridState {
    plugins: PluginOverview[];
    
    lastPage?: Page<PluginOverview>
}

export class PluginDisplayGrid extends Component<{}, PluginGridState> {
    
    private initialCall: boolean = false;

    /**
     * Constructor for initializing the component with props and setting the initial state.
     *
     * @param {Object} props - The properties passed to the component.
     * @return {void}
     */
    constructor(props: {}) {
        super(props);
        this.state = {
            plugins: [],
        };
    }

    componentDidMount() {
        if (!this.initialCall) {
            this.populatePluginList();
        }
        this.initialCall = true;
    }
    
    render() {
        return <InfiniteScroll
            dataLength={this.state.plugins.length}
            next={this.populatePluginList}
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
    }

    private async populatePluginList() {
        const response = await pluginsApi.getPlugins({
            page: this.state.lastPage ? this.state.lastPage.pageNumber + 1 : 1,
            size: 25
        })
        this.setState({
            plugins: this.state.plugins.concat(response.items),
            lastPage: response
        })
    }
}