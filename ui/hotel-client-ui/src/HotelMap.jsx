import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";

// Leaflet marker icon fix (Vite’da default icon bazen görünmez)
import markerIcon2x from "leaflet/dist/images/marker-icon-2x.png";
import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";

delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
    iconRetinaUrl: markerIcon2x,
    iconUrl: markerIcon,
    shadowUrl: markerShadow,
});

export default function HotelMap({ items }) {
    if (!items?.length) return null;

    const first = items[0];
    const center = [first.lat ?? first.Lat ?? 39.0, first.lng ?? first.Lng ?? 35.0];

    return (
        <div style={{ marginTop: 16 }}>
            <h3>Map</h3>
            <MapContainer center={center} zoom={12} style={{ height: 360, width: "100%" }}>
                <TileLayer
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    attribution="&copy; OpenStreetMap contributors"
                />

                {items.map((h, idx) => {
                    const lat = h.lat ?? h.Lat;
                    const lng = h.lng ?? h.Lng;
                    if (lat == null || lng == null) return null;

                    const name = h.name ?? h.hotelName ?? h.hotelId ?? `Hotel ${idx + 1}`;
                    const dest = h.destination ?? "";
                    const price = h.pricePerNight ?? h.price ?? null;

                    return (
                        <Marker key={h.id ?? h.hotelId ?? idx} position={[lat, lng]}>
                            <Popup>
                                <b>{name}</b>
                                <div>{dest}</div>
                                {price != null ? <div>Price: {price}</div> : null}
                            </Popup>
                        </Marker>
                    );
                })}
            </MapContainer>
        </div>
    );
}
