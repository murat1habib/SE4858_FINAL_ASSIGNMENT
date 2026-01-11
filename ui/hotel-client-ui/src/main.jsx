import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import "leaflet/dist/leaflet.css";

import SearchPage from "./pages/SearchPage";
import HotelDetailPage from "./pages/HotelDetailPage";

ReactDOM.createRoot(document.getElementById("root")).render(
    <React.StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<SearchPage />} />
                <Route path="/hotels/:id" element={<HotelDetailPage />} />
            </Routes>
        </BrowserRouter>
    </React.StrictMode>
);

