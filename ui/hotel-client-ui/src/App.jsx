import { useState } from "react";
import { searchHotels } from "./api";
import HotelMap from "./HotelMap";

export default function App() {
    const [destination, setDestination] = useState("Bodrum");
    const [startDate, setStartDate] = useState("2026-03-10");
    const [endDate, setEndDate] = useState("2026-03-12");
    const [people, setPeople] = useState(2);
    const [roomCount, setRoomCount] = useState(1);

    const [loading, setLoading] = useState(false);
    const [err, setErr] = useState("");
    const [items, setItems] = useState([]);

    const [showMap, setShowMap] = useState(false);

    async function onSearch() {
        setErr("");
        setLoading(true);
        try {
            const data = await searchHotels({
                destination,
                startDate,
                endDate,
                people: Number(people),
                roomCount: Number(roomCount),
                page: 1,
                pageSize: 10,
            });

            const nextItems = data.items ?? data.Items ?? data;
            setItems(nextItems);

            // Search baþarýlý olunca haritayý kapat
            setShowMap(false);
        } catch (e) {
            setErr(e.message || "Search failed");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div style={{ fontFamily: "system-ui", maxWidth: 900, margin: "40px auto", padding: 16 }}>
            <h1>Hotel Search</h1>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
                <label>
                    Destination
                    <input value={destination} onChange={(e) => setDestination(e.target.value)} style={{ width: "100%", padding: 8 }} />
                </label>

                <label>
                    People
                    <input type="number" value={people} onChange={(e) => setPeople(e.target.value)} style={{ width: "100%", padding: 8 }} />
                </label>

                <label>
                    Start Date
                    <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} style={{ width: "100%", padding: 8 }} />
                </label>

                <label>
                    End Date
                    <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} style={{ width: "100%", padding: 8 }} />
                </label>

                <label>
                    Room Count
                    <input type="number" value={roomCount} onChange={(e) => setRoomCount(e.target.value)} style={{ width: "100%", padding: 8 }} />
                </label>
            </div>

            <div style={{ marginTop: 12 }}>
                <button onClick={onSearch} disabled={loading} style={{ padding: "10px 14px" }}>
                    {loading ? "Searching..." : "Search"}
                </button>

                <button
                    onClick={() => setShowMap((v) => !v)}
                    disabled={!items?.length}
                    style={{ marginLeft: 10, padding: "10px 14px" }}
                >
                    {showMap ? "Hide Map" : "Show on Map"}
                </button>
            </div>

            {err && <p style={{ color: "crimson" }}>{err}</p>}

            <hr style={{ margin: "20px 0" }} />

            <h2>Results</h2>
            {items?.length === 0 ? (
                <p>No results.</p>
            ) : (
                <ul>
                    {items.map((x, idx) => (
                        <li key={x.id ?? x.hotelId ?? idx} style={{ marginBottom: 10 }}>
                            <b>{x.name ?? x.hotelName ?? x.hotelId}</b> <span>({x.destination})</span>
                            {"price" in x ? <div>Price: {x.price}</div> : null}
                            {"pricePerNight" in x ? <div>Price/Night: {x.pricePerNight}</div> : null}
                        </li>
                    ))}
                </ul>
            )}

            {showMap && <HotelMap items={items} />}
        </div>
    );
}
