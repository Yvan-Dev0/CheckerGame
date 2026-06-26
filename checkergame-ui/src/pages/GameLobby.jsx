import { useState } from "react";

export default function GameLobby() {
  const [roomId, setRoomId] = useState("");

  const handleMatchmaking = () => {
    alert("Joining matchmaking queue...");
    // TODO: Call your SignalR hub method for matchmaking
  };

  const handleJoinByRoomId = () => {
    if (!roomId) {
      alert("Please enter a room ID");
      return;
    }
    alert(`Joining room: ${roomId}`);
    // TODO: Call your SignalR hub method to join by roomId
  };

  const handlePlayVsAI = () => {
    alert("Starting game vs AI...");
    // TODO: Call your SignalR hub method for AI game
  };

  return (
    <div style={{ padding: "2rem" }}>
      <h2>Game Lobby</h2>

      <div style={{ marginBottom: "1rem" }}>
        <button onClick={handleMatchmaking}>Join Matchmaking</button>
      </div>

      <div style={{ marginBottom: "1rem" }}>
        <input
          type="text"
          placeholder="Enter Room ID"
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
