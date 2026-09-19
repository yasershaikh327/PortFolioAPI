//Visitors Scripts
const API_URL = "/visitors"; // <-- set to your real endpoint, e.g. "https://port-folio-api-one.vercel.app/api/visitors"
const API_URL2 = "/twilio-balance"; 
const PAGE_SIZE = 10;

// Sample data — trimmed, de-identified-enough snapshot mirroring your CSV,
// used whenever the live API isn't set or isn't reachable.
const SAMPLE_DATA = [];

let allRows = [];
let filteredRows = [];
let currentPage = 1;
let usingLiveData = false;

const el = (id) => document.getElementById(id);

// ---------- Entry point (safe on any page) ----------
// This file is shared site-wide, so it must no-op cleanly on pages
// that don't have the Visitor Insights markup instead of throwing.

function initVisitorPage() {
    const requiredIds = [
        "visitorTableBody", "searchInput", "countryFilter", "deviceFilter",
        "prevPage", "nextPage", "statusDot", "statusLabel"
    ];
    const missing = requiredIds.filter((id) => !el(id));
    if (missing.length) {
        // Not on the Visitor Insights page — do nothing.
        return;
    }

    el("searchInput").addEventListener("input", applyFilters);
    el("countryFilter").addEventListener("change", applyFilters);
    el("deviceFilter").addEventListener("change", applyFilters);
    el("prevPage").addEventListener("click", () => { currentPage--; renderTable(); });
    el("nextPage").addEventListener("click", () => { currentPage++; renderTable(); });

    const yearEl = document.getElementById("year");
    if (yearEl) yearEl.textContent = new Date().getFullYear();

    loadData();
}

// ---------- Data loading ----------

async function loadData() {
    if (API_URL) {
        try {
            const res = await fetch(API_URL, { headers: { "Accept": "application/json" } });
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            const data = await res.json();
            if (!Array.isArray(data.data) || data.data.length === 0) throw new Error("Empty response");
            allRows = normalizeRows(data.data);
            usingLiveData = true;
            console.log(data);
        } catch (err) {
            console.warn("Live API unavailable, using sample data:", err.message);
            allRows = normalizeRows(SAMPLE_DATA);
            usingLiveData = false;
        }
    } else {
        allRows = normalizeRows(SAMPLE_DATA);
        usingLiveData = false;
    }

    setStatus();
    populateFilterOptions();
    applyFilters();
    renderStats();
    renderSources();
}

function normalizeRows(rows) {
    return rows
        .map((r) => ({
            id: r.id,
            country_code: r.country_code || "",
            country_name: r.country_name || "Unknown",
            city: r.city || "Unknown",
            timezone: r.timezone || "",
            device_type: r.device_type || "Unknown",
            operating_system: r.operating_system || "Unknown",
            browser: r.browser || "Unknown",
            page_url: r.page_url || "",
            referrer: r.referrer || "Direct Visit",
            visit_time: r.visit_time || ""
        }))
        .sort((a, b) => new Date(b.visit_time) - new Date(a.visit_time));
}

function setStatus() {
    const dot = el("statusDot");
    const label = el("statusLabel");
    dot.classList.remove("vp-live", "vp-demo");
    if (usingLiveData) {
        dot.classList.add("vp-live");
        label.textContent = "Live data from portfolio API";
    } else {
        dot.classList.add("vp-demo");
        label.textContent = API_URL ? "Showing sample data — API not connected" : "Showing sample data — API_URL not set";
    }
}

// ---------- Stats ----------

function renderStats() {
    el("statTotal").textContent = allRows.length.toLocaleString();
    el("statTotalSub").textContent = "all-time visits";

    const countries = new Set(allRows.map((r) => r.country_name).filter((c) => c && c !== "Unknown"));
    el("statCountries").textContent = countries.size;

    const topCountry = topEntry(allRows, "country_name");
    el("statTopCountry").textContent = topCountry ? `Most from ${topCountry}` : "";

    const topDevice = topEntry(allRows, "device_type");
    el("statDevice").textContent = topDevice || "–";

    const topBrowser = topEntry(allRows, "browser");
    el("statTopBrowser").textContent = topBrowser ? `${topBrowser} leads browsers` : "";
}

function topEntry(rows, field) {
    const counts = {};
    rows.forEach((r) => {
        const v = r[field];
        if (!v || v === "Unknown") return;
        counts[v] = (counts[v] || 0) + 1;
    });
    const sorted = Object.entries(counts).sort((a, b) => b[1] - a[1]);
    return sorted.length ? sorted[0][0] : null;
}

function renderSources() {
    const counts = {};
    allRows.forEach((r) => {
        const ref = r.referrer === "Direct Visit" ? "Direct Visit" : hostnameOf(r.referrer);
        counts[ref] = (counts[ref] || 0) + 1;
    });
    const top = Object.entries(counts).sort((a, b) => b[1] - a[1]).slice(0, 8);
    const container = el("sourcesPills");
    if (!container) return;
    container.innerHTML = "";
    if (top.length === 0) {
        container.innerHTML = `<span class="vp-pill">No data yet</span>`;
        return;
    }
    top.forEach(([source, count]) => {
        const pill = document.createElement("span");
        pill.className = "vp-pill";
        pill.textContent = `${source} · ${count}`;
        container.appendChild(pill);
    });
}

function hostnameOf(url) {
    try {
        return new URL(url).hostname.replace(/^www\./, "");
    } catch {
        return url || "Direct Visit";
    }
}

// ---------- Filters ----------

function populateFilterOptions() {
    const countrySel = el("countryFilter");
    const deviceSel = el("deviceFilter");

    const countries = [...new Set(allRows.map((r) => r.country_name))].sort();
    const devices = [...new Set(allRows.map((r) => r.device_type))].sort();

    countrySel.innerHTML = `<option value="">All Countries</option>` +
        countries.map((c) => `<option value="${c}">${c}</option>`).join("");

    deviceSel.innerHTML = `<option value="">All Devices</option>` +
        devices.map((d) => `<option value="${d}">${d}</option>`).join("");
}

function applyFilters() {
    const search = el("searchInput").value.trim().toLowerCase();
    const country = el("countryFilter").value;
    const device = el("deviceFilter").value;

    filteredRows = allRows.filter((r) => {
        if (country && r.country_name !== country) return false;
        if (device && r.device_type !== device) return false;
        if (search) {
            const haystack = `${r.city} ${r.country_name} ${r.page_url} ${r.referrer} ${r.browser} ${r.operating_system}`.toLowerCase();
            if (!haystack.includes(search)) return false;
        }
        return true;
    });

    currentPage = 1;
    renderTable();
}

// ---------- Table ----------

function renderTable() {
    const tbody = el("visitorTableBody");
    const total = filteredRows.length;
    const totalPages = Math.max(1, Math.ceil(total / PAGE_SIZE));
    currentPage = Math.min(currentPage, totalPages);

    const rowCountLabel = el("rowCountLabel");
    if (rowCountLabel) {
        rowCountLabel.innerHTML = `<span class="vp-status-dot" style="background:var(--accent)"></span>${total.toLocaleString()} visit${total === 1 ? "" : "s"}`;
    }

    if (total === 0) {
        tbody.innerHTML = `<tr><td colspan="6"><div class="vp-empty">No visits match these filters.</div></td></tr>`;
    } else {
        const start = (currentPage - 1) * PAGE_SIZE;
        const pageRows = filteredRows.slice(start, start + PAGE_SIZE);

        tbody.innerHTML = pageRows.map((r) => `
      <tr>
        <td>
          <div class="vp-cell-primary">${escapeHtml(r.city)}</div>
          <div>${escapeHtml(r.country_name)}${r.country_code ? " · " + escapeHtml(r.country_code) : ""}</div>
        </td>
        <td><span class="vp-badge"><span class="vp-badge-dot vp-${r.device_type.toLowerCase()}"></span>${escapeHtml(r.device_type)}</span></td>
        <td>${escapeHtml(r.browser)}<div>${escapeHtml(r.operating_system)}</div></td>
        <td><a class="vp-cell-link" href="${escapeAttr(r.page_url)}" target="_blank" rel="noopener">${escapeHtml(shorten(r.page_url, 34))}</a></td>
        <td>${r.referrer === "Direct Visit" ? "Direct Visit" : escapeHtml(shorten(hostnameOf(r.referrer), 28))}</td>
        <td>${formatTime(r.visit_time)}</td>
      </tr>
    `).join("");
    }

    el("pageLabel").textContent = `Page ${currentPage} of ${totalPages}`;
    el("prevPage").disabled = currentPage <= 1;
    el("nextPage").disabled = currentPage >= totalPages;
}

function shorten(str, max) {
    if (!str) return "–";
    return str.length > max ? str.slice(0, max - 1) + "…" : str;
}

function formatTime(ts) {
    const d = new Date(ts);
    if (isNaN(d.getTime())) return ts || "–";
    return d.toLocaleString(undefined, {
        year: "numeric", month: "short", day: "numeric",
        hour: "2-digit", minute: "2-digit"
    });
}

function escapeHtml(str) {
    const div = document.createElement("div");
    div.textContent = str ?? "";
    return div.innerHTML;
}

function escapeAttr(str) {
    return (str || "").replace(/"/g, "&quot;");
}

// ---------- Boot ----------

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initVisitorPage);
} else {
    initVisitorPage();
}

// async function getTwilioBalance() {
//     try {
//         const res = await fetch("/api/Notification/twilio-balance", {
//             headers: { "Accept": "application/json" }
//         });

//         if (!res.ok) throw new Error(`HTTP ${res.status}`);

//         const { balance } = await res.json();
//         //console.log("Twilio Balance:", balance);
//         document.getElementById("twilio-balance").style="display: inline-block;color: red;font-weight: bolder;";
//         document.getElementById("twilio-balance").textContent="Twilio Balance: " + balance;
//     } catch (err) {
//         console.error("Failed to fetch Twilio balance:", err.message);
//     }
// }

// getTwilioBalance();
