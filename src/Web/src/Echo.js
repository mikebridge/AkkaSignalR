import React, { Component } from 'react';

import update from 'immutability-helper';

import $ from 'jquery';
//require('expose?$!expose?jQuery!jquery');
import "signalr";

class Error extends Component {
    render() {
        const fg = this.props.color || "red";
        if (this.props.message) {
            return (
                <div className={"flex items-center justify-center pa4 bg-light-gray " + fg}>
                    <svg className="w1" data-icon="info" viewBox="0 0 32 32" style={{fill: fg}}>
                        <title>info icon</title>
                        <path
                            d="M16 0 A16 16 0 0 1 16 32 A16 16 0 0 1 16 0 M19 15 L13 15 L13 26 L19 26 z M16 6 A3 3 0 0 0 16 12 A3 3 0 0 0 16 6" />
                    </svg>
                    <span className="lh-title ml3">{this.props.message}</span>
                </div>
            );
        }
        else {
            return <div></div>;
        }
    }
}

const initialState = {
    message: "Hello World",
    error: "",
    success: "",
    connectionid: null,
    messages: []
};

class Config extends Component {
    render() {
        return (
            <div className="bg-light-gray dib pa1 ma1 br3-ns">
                <dl className="pa4 mt0">
                    <dt className="f6 b">Hub</dt>
                    <dd className="ml0">{this.props.hub}</dd>
                    <dt className="f6 b mt2">URL</dt>
                    <dd className="ml0">{this.props.url}</dd>
                    <dt className="f6 b mt2">ConnectionId</dt>
                    <dd className="ml0">{this.props.connectionid || "not connected" }</dd>
                </dl>
            </div>
        );
    }
}

class Message extends Component {
    render() {
        return <li className="lh-copy pv3 ba bl-0 bt-0 br-0 b--dotted b--black-30">{this.props.message}</li>;
    }
}

class MessageList extends Component {

    render() {
        const messages = this.props.messages.map((message, index) => <Message key={index} message={message} />);
        return (
            <div>
                <ul className="list pl0 measure center">
                    { messages }
                </ul>
            </div>
        );
    }
}

class App extends Component {

    constructor(props) {
        super(props);
        this.state = this.state || initialState;
    }

    onSendClick(e) {
        e.preventDefault();
        console.log(`Sending ${this.state.message}`);
        this.invokeHubMethod("sendMessage", this.state.message);
    }

    handleChange(e) {
        const {name, value} = e.target;
        const diff = { [name]: {$set: value} };
        this.setState(update(this.state, diff));
    }

    invokeHubMethod(method, ...args) {
        this.proxy.invoke(method, ...args).done(() => {
            console.log(`Invocation of ${method} succeeded`);
        }).fail((error) => {
            this.setState(Object.assign({}, this.state, {error: error.message}));
        });
    }
    render() {
        const btnTextColor = this.state.connectionid ? "white" : "lightGrey";
        const btnBgColor = this.state.connectionid ? "bg-black-70" : "bg-grey";
        const btnBgHoverColor = this.state.connectionid ? "hover-bg-black" : "light-silver";
        const style = this.state.connectionid ? {} : {cursor: "progress"} ;
        const disabled = !this.state.connectionid;
        return (
            <div>
                <div>&nbsp;</div>
                <Error message={this.state.error} color="red"/>

                <div className="pa4-l">
                    <form className="bg-light-green mw7 center pa4 br3-ns ba b--black-10">
                        <fieldset className="cf bn ma0 pa0">
                            <legend className="pa0 f5 f4-ns mb3 black-80">Send via SignalR</legend>

                            <div className="cf">
                                <label className="clip" htmlFor="echo-text">Echo Text</label>
                                <input className="f6 f5-l input-reset bn fl black-80 bg-white pa3 lh-solid w-100 w-75-m w-80-l br2-ns br--left-ns"
                                       placeholder="Text"
                                       type="text"
                                       name="message"
                                       id="echo-text"
                                       value={this.state.message}
                                       onChange={this.handleChange.bind(this)}
                                        />
                                <input className={`f6 f5-l button-reset fl pv3 tc bn bg-animate ${btnBgColor} ${btnBgHoverColor} ${btnTextColor} pointer w-100 w-25-m w-20-l br2-ns br--right-ns`}
                                       type="submit"
                                       value="Send"
                                       disabled={disabled}
                                       style={style}
                                       onClick={this.onSendClick.bind(this)} />
                            </div>
                        </fieldset>
                    </form>
                </div>
                <MessageList messages={this.state.messages}/>
                <Config {...this.props} connectionid={this.state.connectionid} />
            </div>
        );
    }

    // server-to-client message handlers
    echoMessageHandler(message) {
        console.log(message);
        this.setState(Object.assign({}, this.state, {messages: [...this.state.messages, message]}));
    }

    componentDidMount() {
        console.log(`Mounting SignalRConnector ${this.props.hub} at ${this.props.url}`);
        this.hubConnection = $.hubConnection(this.props.url);
        if (this.props.logging) {
            this.hubConnection.logging = true;
        }

        this.createProxies(this.hubConnection);
        this.connectToHub();
    }

    createProxies(hubConnection) {
        this.proxy = hubConnection.createHubProxy(this.props.hub);
        // when the server calls "echoMessage" on the client,
        // handle it with "this.echoMessageHandler".
        // see: https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/hubs-api-guide-javascript-client
        this.proxy.on("echoMessage", this.echoMessageHandler.bind(this));
    }

    connectToHub() {

        this.hubConnection.start()
            .done(() => {
                this.setState(Object.assign({}, this.state, {
                    connectionid: this.hubConnection.id,
                    success: `Connected to Hub with connectionid ${this.hubConnection.id}`
                }));
            })
            .fail((e) => {
                this.setState(Object.assign({}, this.state, {
                    error : e.message,
                    connectionid: null,
                }));
            });
    }

}

App.propTypes = {
    hub: React.PropTypes.string.isRequired,
    url: React.PropTypes.string.isRequired,
    logging: React.PropTypes.bool
};

export default App;
