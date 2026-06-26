import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7259/gamehub")
    .withAutomaticReconnect()
    .build();

export default connection;