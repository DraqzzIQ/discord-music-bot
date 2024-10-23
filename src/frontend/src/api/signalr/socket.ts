import {HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel} from '@microsoft/signalr';

const url = process.env.NEXT_PUBLIC_SIGNALR_API_URL + 'bot';
let connection = null as HubConnection | null;
let closeCallbacks: (() => void)[] = [];
let connectedCallbacks: (() => void)[] = [];
let reconnectedCallbacks: (() => void)[] = [];

function createConnection() {
    if(connection)
        console.log('SOCKET: Connection already exists');
    if (connection)
        return;
    
    console.log('SOCKET: connecting to ', url);
    connection = new HubConnectionBuilder()
        .withUrl(url, {
            logger: LogLevel.Information,
            withCredentials: true,
        })
        .withAutomaticReconnect()
        .build();
    
    connection.onclose(async () => {
        closeCallbacks.forEach(cb => cb());
    });
    connection.onreconnected(async () => {
        reconnectedCallbacks.forEach(cb => cb());
    });
}

async function startConnection() {
    if(!connection) {
        createConnection();
    }
    if (!connection?.state || connection?.state === HubConnectionState.Disconnected) {
        console.log('SOCKET: Starting connection');
        await connection?.start().then(() => {
            connectedCallbacks.forEach(cb => cb());
        });
    }
}

async function stopConnection() {
    if (connection && connection.state == HubConnectionState.Connected) {
        console.log('SOCKET: Stopping connection');
        await connection.stop();
    }
}

async function registerOnServerEvents(
    eventName: string,
    callback: (payload: any) => void,
) {
    try {
        await startConnection();
        
        connection?.off(eventName);
        
        connection?.on(eventName, (payload) => {
            callback(payload);
        });
        connection?.onclose(() => stopConnection());
    } catch (error) {
        console.error('SOCKET: ', error);
    }
}

async function invokeMethod(methodName: string, payload: any) {
    await startConnection();
    
    if (connection?.state === HubConnectionState.Connected) {
        if (payload) {
            connection.invoke(methodName, payload).catch(err => console.error('SOCKET: ', err.toString()));
        } else {
            connection.invoke(methodName).catch(err => console.error('SOCKET: ', err.toString()));
        }
    } else {
        console.error('SOCKET: Cannot invoke method ', methodName);
        console.error('SOCKET: Connection is in state ', connection?.state);
    }
}

function registerOnConnectedCallback(callback: () => void) {
    connectedCallbacks.push(callback);
}

function unregisterOnConnectedCallback(callback: () => void) {
    connectedCallbacks = connectedCallbacks.filter(cb => cb !== callback);
}
    
function registerOnCloseCallback(callback: () => void) {
    closeCallbacks.push(callback);
}

function unregisterOnCloseCallback(callback: () => void) {
    closeCallbacks = closeCallbacks.filter(cb => cb !== callback);
}

function registerOnReconnectedCallback(callback: () => void) {
    reconnectedCallbacks.push(callback);
}

function unregisterOnReconnectedCallback(callback: () => void) {
    reconnectedCallbacks = reconnectedCallbacks.filter(cb => cb !== callback);
}

export const socketService = {
    registerOnServerEvents,
    stopConnection,
    invokeMethod,
    registerOnConnectedCallback,
    registerOnCloseCallback,
    unregisterOnConnectedCallback,
    unregisterOnCloseCallback,
    registerOnReconnectedCallback,
    unregisterOnReconnectedCallback,
};