import { useState } from "react";
import { predictPrice } from "../api";

export default function PredictPage() {
    const [destination, setDestination] = useState("Barcelona");
    const [startDate, setStartDate] = useState("2026-04-10");
    const [endDate, setEndDate] = useState("2026-04-12");
    const [roomType, setRoomType] = useState("Standard");
    const [roomCount, setRoomCount] = useState(1);
    const [people, setPeople] = useState(2);

    const [result, setResult] = useState(null);
    const [err, setErr] = useState("");

    async function run() {
        setErr(""); setResult(null);
        try {
            const data = await predictPrice({ destination, startDate, endDate, roomType, roomCount: Number(roomCount), people: Number(people) });
            setResult(data);
        } catch (e) {
            setErr(e.message || "Failed");
        }
    }

    return (
        <div style={{ maxWidth: 900, margin: "40px auto", padding: 16, fontFamily: "system-ui" }}>
            <h1>Admin - Price Predict</h1>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
                <label>Destination<input value={destination} onChange={(e) => setDestination(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>Room Type<input value={roomType} onChange={(e) => setRoomType(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>Start Date<input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>End Date<input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>Room Count<input type="number" value={roomCount} onChange={(e) => setRoomCount(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>People<input type="number" value={people} onChange={(e) => setPeople(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
            </div>

            <button onClick={run} style={{ marginTop: 12, padding: "10px 14px" }}>Predict</button>

            {err && <p style={{ color: "crimson" }}>{err}</p>}
            {result && <pre style={{ marginTop: 12, background: "#111", color: "#0f0", padding: 12 }}>{JSON.stringify(result, null, 2)}</pre>}
        </div>
    );
}
