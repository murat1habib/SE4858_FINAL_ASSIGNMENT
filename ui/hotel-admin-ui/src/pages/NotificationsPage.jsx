import { useState } from "react";
import { listNotifications } from "../api";

export default function NotificationsPage() {
    const [hotelId, setHotelId] = useState("hotel-1");
    const [items, setItems] = useState([]);
    const [err, setErr] = useState("");

    async function load() {
        setErr("");
        try {
            const data = await listNotifications(hotelId);
            setItems(data || []);
        } catch (e) {
            setErr(e.message || "Failed");
        }
    }

    return (
        <div style={{ maxWidth: 900, margin: "40px auto", padding: 16, fontFamily: "system-ui" }}>
            <h1>Admin - Notifications</h1>

            <div style={{ display: "flex", gap: 10, alignItems: "end" }}>
                <label style={{ flex: 1 }}>
                    HotelId
                    <input value={hotelId} onChange={(e) => setHotelId(e.target.value)} style={{ width: "100%", padding: 8 }} />
                </label>
                <button onClick={load} style={{ padding: "10px 14px" }}>Load</button>
            </div>

            {err && <p style={{ color: "crimson" }}>{err}</p>}

            <ul style={{ marginTop: 16 }}>
                {items.map((n, idx) => (
                    <li key={n.id ?? idx} style={{ marginBottom: 10 }}>
                        <b>{n.type}</b> - {n.message}
                        <div style={{ opacity: 0.7 }}>{n.createdAt}</div>
                    </li>
                ))}
            </ul>
        </div>
    );
}
