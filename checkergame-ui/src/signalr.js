import * as signalR from "@microsoft/signalr";

let connection = null;
let reconnectAttempts = 0;
const MAX_RECONNECT_ATTEMPTS = 5;

const getToken = () => {
    const token = localStorage.getItem('token');
    if (!token) {
        console.warn('No token found in localStorage');
        return null;
    }
    return token;
};

const createConnection = () => {
    const token = getToken();
    const url = "https://localhost:7259/gamehub";

    console.log('Creating SignalR connection with token:', token ? 'Token present' : 'No token');

    return new signalR.HubConnectionBuilder()
        .withUrl(url, {
            accessTokenFactory: () => {
                const currentToken = getToken();
                console.log('Access token requested:', currentToken ? 'Token available' : 'No token');
                return currentToken || '';
            },
            transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect({
            nextRetryDelayInMilliseconds: (retryContext) => {
                if (retryContext.previousRetryCount >= MAX_RECONNECT_ATTEMPTS) {
                    console.log('Max reconnect attempts reached');
                    return null;
                }
                
                const delay = Math.min(30000, 2000 * Math.pow(2, retryContext.previousRetryCount));
                console.log(`Reconnect attempt ${retryContext.previousRetryCount + 1} in ${delay}ms`);
                return delay;
            }
        })
        .withKeepAliveInterval(15000)
        .build();
};

export const getConnection = () => {
    if (!connection) {
        connection = createConnection();
        
        // Handle connection events
        connection.onreconnecting((error) => {
            console.warn('SignalR reconnecting...', error);
        });
        
        connection.onreconnected((connectionId) => {
            console.log('SignalR reconnected with ID:', connectionId);
            reconnectAttempts = 0;
        });
        
        connection.onclose((error) => {
            console.error('SignalR connection closed:', error);
            reconnectAttempts = 0;
        });
    }
    return connection;
};

// Helper function to start connection
export const startConnection = async () => {
    const conn = getConnection();
    try {
        if (conn.state === signalR.HubConnectionState.Disconnected) {
            const token = getToken();
            if (!token) {
                console.warn('Cannot start connection: No token available. Please login first.');
                return false;
            }
            await conn.start();
            console.log('✅ SignalR connected successfully');
            return true;
        }
        return conn.state === signalR.HubConnectionState.Connected;
    } catch (error) {
        console.error('❌ Failed to start SignalR connection:', error);
        return false;
    }
};

// Helper function to stop connection
export const stopConnection = async () => {
    if (connection) {
        try {
            await connection.stop();
            console.log('SignalR connection stopped');
        } catch (error) {
            console.error('Error stopping SignalR connection:', error);
        }
    }
};

// Export the connection as default for backward compatibility
const defaultConnection = getConnection();
export default defaultConnection;