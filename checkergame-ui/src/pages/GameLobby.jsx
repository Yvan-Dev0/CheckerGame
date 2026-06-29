import { useEffect, useState } from "react";
import connection from "../signalr";

export default function GameLobby() {
  const [roomId, setRoomId] = useState("");
  const [status, setStatus] = useState("");

  useEffect(() => {
    if (connection.state === "Disconnected") {
    connection.start()
      .then(() => console.log("✅ Connected to SignalR Hub"))
      .catch(err => console.error("❌ Connection failed:", err));
    }

    // Listen for hub broadcasts
    connection.on("WaitingForOpponent", () => {
      setStatus("Waiting for opponent...");
    });

    connection.on("MatchFound", (gameId) => {
      setStatus(`Match found! Game ID: ${gameId}`);
    });

    connection.on("MatchFoundAI", (gameId) => {
      setStatus(`AI match started! Game ID: ${gameId}`);
    });

    connection.on("PlayerJoined", (connectionId) => {
      console.log("Player joined:", connectionId);
    });

    connection.on("GameUpdated", (update) => {
      console.log("Game updated:", update);
    });

    return () => {
      connection.off("WaitingForOpponent");
      connection.off("MatchFound");
      connection.off("MatchFoundAI");
      connection.off("PlayerJoined");
      connection.off("GameUpdated");
    };
  }, []);

  const handleMatchmaking = async () => {
    await connection.invoke("JoinMatchmaking");
  };

  const handleJoinByRoomId = async () => {
    if (!roomId) {
      alert("Please enter a room ID");
      return;
    }
    await connection.invoke("JoinGame", parseInt(roomId));
  };

  const handlePlayVsAI = async () => {
    await connection.invoke("PlayAgainstAI");
  };

  return (
    <div style={{ padding: "2rem" }}>
      <h2>Game Lobby</h2>
      <p>Status: {status}</p>

      <div style={{ marginBottom: "1rem" }}>
        <button onClick={handleMatchmaking}>Join Matchmaking</button>
      </div>

      <div style={{ marginBottom: "1rem" }}>
        <input
          type="text"
          placeholder="Enter Game ID"
          value={roomId}
          onChange={(e) => setRoomId(e.target.value)}
        />
        <button onClick={handleJoinByRoomId}>Join by Room ID</button>
      </div>

      <div>
        <button onClick={handlePlayVsAI}>Play vs AI</button>
      </div>
    </div>
  );
}
