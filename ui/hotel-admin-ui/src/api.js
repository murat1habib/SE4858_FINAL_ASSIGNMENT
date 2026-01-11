const BASE = import.meta.env.VITE_API_BASE_URL;

async function http(method, path, body) {
    const res = await fetch(`${BASE}${path}`, {
        method,
        headers: {
            "Content-Type": "application/json",
            // Admin simulate (þimdilik)
            Authorization: "Bearer admin-demo",
        },
        body: body ? JSON.stringify(body) : undefined,
    });

    if (!res.ok) throw new Error((await res.text()) || `HTTP ${res.status}`);
    return res.status === 204 ? null : res.json();
}

export function upsertAvailability(payload) {
    return http("POST", "/hotel/api/v1/admin/availability", payload);
}

export function listNotifications(hotelId) {
    const q = hotelId ? `?hotelId=${encodeURIComponent(hotelId)}` : "";
    return http("GET", `/notify/api/v1/notifications${q}`);
}

export function predictPrice(payload) {
    return http("POST", "/hotel/api/v1/pricing/predict", payload);
}
