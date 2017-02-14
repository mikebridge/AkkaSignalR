import React, { Component } from 'react';
import './App.css';
import Echo from './Echo';

class App extends Component {

    render() {
        return (

        <Echo logging={true}
                  hub="echoHub"
                  url="http://localhost:5001"
            />
        );
    }
}

export default App;
