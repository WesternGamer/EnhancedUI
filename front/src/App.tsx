import React, {useEffect, useState} from 'react';
import logo from './logo.svg';
import './App.css';
import {Block, terminal} from "./interop/terminal";
import {CefSharp} from "./interop/cefsharp";

function App() {
    const [blocks, setBlocks] = useState<Block[]>([]);

    useEffect(() => {
        async function getBlocks() {
            await CefSharp.BindObjectAsync("terminal");
            setBlocks(await terminal.getBlocks());
        }
        getBlocks().catch(alert);
    });
    return (
            <div className="App">
                <header className="App-header">
                    <img src={logo} className="App-logo" alt="logo"/>
                    <p>
                        Edit <code>src/App.tsx</code> and save to reload.
                    </p>
                    <a
                            className="App-link"
                            href="https://reactjs.org"
                            target="_blank"
                            rel="noopener noreferrer"
                    >
                        Learn React
                    </a>
                    <p>count: {blocks.length}</p>
                    <p>is cef: {CefSharp === undefined}</p>
                    {blocks.map((value, index) => {
                        return (<li>{index} : {value}</li>)
                    })}
                </header>
            </div>
    );
}

export default App;
