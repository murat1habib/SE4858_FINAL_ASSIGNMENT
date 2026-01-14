const BASE =
    import.meta.env.VITE_API_BASE_URL ||
    "https://hotelbooking-gateway-unique-d4h8fffjgxa8fqet.francecentral-01.azurewebsites.net";

async function http(method, path, body, extraHeaders) {
    const res = await fetch(`${BASE}${path}`, {
        method,
        headers: { "Content-Type": "application/json", ...(extraHeaders || {}) },
        body: body ? JSON.stringify(body) : undefined,
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(text || `HTTP ${res.status}`);
    }

    return res.status === 204 ? null : res.json();
}

export async function searchHotels(payload, isLoggedIn) {
    const headers = isLoggedIn ? { Authorization: "Bearer demo" } : undefined;
    return http("POST", "/hotel/api/v1/hotels/search", payload, headers);
}

export async function getHotelDetail(hotelId) {
    return http("GET", `/hotel/api/v1/hotels/${hotelId}`);
}

export async function bookHotel(payload) {
    return http("POST", "/hotel/api/v1/bookings", payload);
}
