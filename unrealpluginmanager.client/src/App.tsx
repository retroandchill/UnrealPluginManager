import { Component } from 'react';
import './App.css';
import {PluginDisplayGrid} from "./components";
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';
import { createBrowserRouter, RouterProvider } from "react-router-dom";

/**
 * The App class is a React component that manages and displays a list of plugins.
 * It fetches the plugin data from a backend server and renders the plugins in a table.
 * The component communicates with an ASP.NET backend and showcases an example
 * integration between JavaScript and ASP.NET.
 *
 * @extends Component
 * @template {}, AppState
 */
class App extends Component {

    private readonly router = createBrowserRouter([
        {
            path: '/',
            element: <PluginDisplayGrid onPluginClick={(plugin) => this.router.navigate(`/plugin/${plugin.name}`)}/>
        },
        {
            path: '/plugin/:id',
            element: <div>This is a test!</div>
        }
    ]);
    
    render() {
        return <div>
            <AppBar position="static">
                <Toolbar>
                    <IconButton
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-label="menu"
                        sx={{ mr: 2 }}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography variant="h4" component="div" sx={{ flexGrow: 1 }}>
                        Unreal Plugin Manager
                    </Typography>
                    <Button color="inherit">Login</Button>
                </Toolbar>
            </AppBar>
            <div style={{padding: '2%'}}>
                <RouterProvider router={this.router} />
            </div>
        </div>;
    }
    
}

export default App;