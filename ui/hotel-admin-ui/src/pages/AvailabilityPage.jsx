import { useState } from "react";
import { upsertAvailability } from "../api";

export default function AvailabilityPage() {
    const [hotelId, setHotelId] = useState("hotel-1");
    const [startDate, setStartDate] = useState("2026-04-10");
    const [endDate, setEndDate] = useState("2026-04-12");
    const [roomType, setRoomType] = useState("Standard");
    const [availableCount, setAvailableCount] = useState(10);
    const [basePrice, setBasePrice] = useState(100);

    const [msg, setMsg] = useState("");
    const [err, setErr] = useState("");

    async function onSubmit() {
        setMsg(""); setErr("");
        try {
            await upsertAvailability({
                hotelId,
                startDate,
                endDate,
                roomType,
                availableCount: Number(availableCount),
                basePrice: Number(basePrice)
            });
            setMsg("Availability upsert OK");
        } catch (e) {
            setErr(e.message || "Failed");
        }
    }

    return (
        <div style={{ maxWidth: 900, margin: "40px auto", padding: 16, fontFamily: "system-ui" }}>
            <h1>Admin - Availability</h1>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
                <label>HotelId<input value={hotelId} onChange={(e) => setHotelId(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>Room Type<input value={roomType} onChange={(e) => setRoomType(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>Start Date<input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>End Date<input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>Available Count<input type="number" value={availableCount} onChange={(e) => setAvailableCount(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
                <label>Base Price<input type="number" value={basePrice} onChange={(e) => setBasePrice(e.target.value)} style={{ width: "100%", padding: 8 }} /></label>
            </div>

            <button onClick={onSubmit} style={{ marginTop: 12, padding: "10px 14px" }}>Save</button>
            {msg && <p style={{ color: "limegreen" }}>{msg}</p>}
            {err && <p style={{ color: "crimson" }}>{err}</p>}
        </div>
    );
}
