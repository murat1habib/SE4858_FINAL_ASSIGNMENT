import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";

import AvailabilityPage from "./pages/AvailabilityPage";
import NotificationsPage from "./pages/NotificationsPage";
import PredictPage from "./pages/PredictPage";

function Layout({ children }) {
    return (
        <div style={{ fontFamily: "system-ui" }}>
            <nav style={{ padding: 12, borderBottom: "1px solid #ddd", display: "flex", gap: 12 }}>
                <Link to="/">Availability</Link>
                <Link to="/notifications">Notifications</Link>
                <Link to="/predict">Predict</Link>
            </nav>
            {children}
        </div>
    );
}

ReactDOM.createRoot(document.getElementById("root")).render(
    <React.StrictMode>
        <BrowserRouter>
            <Layout>
                <Routes>
                    <Route path="/" element={<AvailabilityPage />} />
                    <Route path="/notifications" element={<NotificationsPage />} />
                    <Route path="/predict" element={<PredictPage />} />
                </Routes>
            </Layout>
        </BrowserRouter>
    </React.StrictMode>
);
