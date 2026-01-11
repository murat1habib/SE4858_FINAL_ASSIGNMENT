import { useEffect, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { bookHotel, getHotelDetail } from "../api";

export default function HotelDetailPage() {
    const { id } = useParams();
    const nav = useNavigate();
    const loc = useLocation();

    const initial = loc.state || {};
    const [startDate, setStartDate] = useState(initial.startDate || "2026-03-10");
    const [endDate, setEndDate] = useState(initial.endDate || "2026-03-12");
    const [roomCount, setRoomCount] = useState(Number(initial.roomCount || 1));

    const [hotel, setHotel] = useState(null);
    const [loading, setLoading] = useState(true);
    const [err, setErr] = useState("");
    const [bookingMsg, setBookingMsg] = useState("");

    useEffect(() => {
        let mounted = true;
        (async () => {
            setErr("");
            setLoading(true);
            try {
                const data = await getHotelDetail(id);
                if (mounted) setHotel(data);
            } catch (e) {
                if (mounted) setErr(e.message || "Failed to load hotel detail");
            } finally {
                if (mounted) setLoading(false);
            }
        })();
        return () => (mounted = false);
    }, [id]);

    async function onBook() {
        setBookingMsg("");
        setErr("");
        try {
            const res = await bookHotel({
                hotelId: id,
                startDate,
                endDate,
                roomType: "Standard",
                roomCount: Number(roomCount),
            });
            setBookingMsg(`Booked! ReservationId=${res.reservationId ?? res.ReservationId ?? "?"} Total=${res.totalPrice ?? res.TotalPrice ?? ""}`);
        } catch (e) {
            setErr(e.message || "Booking failed");
        }
    }

    return (
        <div style={{ fontFamily: "system-ui", maxWidth: 900, margin: "40px auto", padding: 16 }}>
            <button onClick={() => nav("/")} style={{ padding: "6px 10px" }}>
                ← Back
            </button>

            {loading ? <p>Loading...</p> : null}
            {err ? <p style={{ color: "crimson" }}>{err}</p> : null}

            {hotel && (
                <>
                    <h1>{hotel.name ?? hotel.Name}</h1>
                    <div>{hotel.destination ?? hotel.Destination}</div>
                    <p>{hotel.description ?? hotel.Description}</p>

                    <hr style={{ margin: "20px 0" }} />

                    <h3>Booking</h3>
                    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
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

                    <button onClick={onBook} style={{ marginTop: 12, padding: "10px 14px" }}>
                        Book
                    </button>

                    {bookingMsg ? <p style={{ color: "limegreen" }}>{bookingMsg}</p> : null}
                </>
            )}
        </div>
    );
}
