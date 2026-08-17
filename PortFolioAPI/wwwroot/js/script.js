let worldMap = null;

function initMap() {
    const mapDiv = document.getElementById('contact-map');
    if (!mapDiv) return;
    if (worldMap) {
        worldMap.invalidateSize();
        return;
    }

    // Create map centered on world view
    worldMap = L.map('contact-map').setView([20.5937, 78.9629], 2);

    // RELIABLE MAP TILES (OpenStreetMap - always works)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
        maxZoom: 6,
        className: 'map-tiles'
    }).addTo(worldMap);

    // Custom glowing marker icon
    const customIcon = L.divIcon({
        html: '<div style="background: #00d4ff; width: 14px; height: 14px; border-radius: 50%; box-shadow: 0 0 14px #00d4ff, 0 0 4px white; border: 2px solid white;"></div>',
        iconSize: [14, 14],
        iconAnchor: [7, 7]
    });

    // Country locations with tooltip labels
    const locations = [
        { latlng: [20.5937, 78.9629], label: '🇮🇳 INDIA' },
        { latlng: [25.2048, 45.0000], label: '🇦🇪 MIDDLE EAST' },
        { latlng: [55.3781, -3.4360], label: '🇬🇧 UNITED KINGDOM' },
        { latlng: [39.8283, -98.5795], label: '🇺🇸 NORTH AMERICA' }
    ];

    locations.forEach(loc => {
        const marker = L.marker(loc.latlng, { icon: customIcon }).addTo(worldMap);

        // PERMANENT VISIBLE TOOLTIP
        marker.bindTooltip(loc.label, {
            permanent: true,
            direction: 'top',
            offset: [0, -12],
            className: 'custom-map-label'
        }).openTooltip();
    });

    // Optional: Add subtle highlight circles around regions
    L.circle([20.5937, 78.9629], { color: '#00d4ff', weight: 1, opacity: 0.3, fillOpacity: 0.05, radius: 700000 }).addTo(worldMap);
    L.circle([25.2048, 45.0000], { color: '#00d4ff', weight: 1, opacity: 0.3, fillOpacity: 0.05, radius: 1000000 }).addTo(worldMap);
    L.circle([55.3781, -3.4360], { color: '#00d4ff', weight: 1, opacity: 0.3, fillOpacity: 0.05, radius: 400000 }).addTo(worldMap);
    L.circle([39.8283, -98.5795], { color: '#00d4ff', weight: 1, opacity: 0.3, fillOpacity: 0.05, radius: 1200000 }).addTo(worldMap);

    // Fix map size after load
    setTimeout(() => worldMap.invalidateSize(), 200);
}

// Initialize map when page loads
window.addEventListener('load', initMap);
window.addEventListener('resize', () => { if (worldMap) worldMap.invalidateSize(); });

function updateClock() {
    const liveClockElement = document.getElementById('liveClock');

    if (!liveClockElement) return; // Exit if element not found

    const now = new Date();

    let hours = now.getHours();
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');

    const ampm = hours >= 12 ? 'PM' : 'AM';

    hours = hours % 12 || 12;
    const formattedHours = String(hours).padStart(2, '0');

    liveClockElement.textContent =
        `🕒 ${formattedHours}:${minutes}:${seconds} ${ampm}`;
}

// Update immediately and then every second
updateClock();
setInterval(updateClock, 1000);

function initMap() {
    const mapDiv = document.getElementById('contact-map');
    if (!mapDiv) return;
    if (worldMap) { worldMap.invalidateSize(); return; }
    worldMap = L.map('contact-map').setView([20.5937, 78.9629], 2.2);
    L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
        attribution: '© OpenStreetMap',
        subdomains: 'abcd',
        maxZoom: 6
    }).addTo(worldMap);
    const pinIcon = L.divIcon({
        html: '<div style="background:#00d4ff; width:12px; height:12px; border-radius:50%; box-shadow:0 0 10px #00d4ff; border:2px solid white;"></div>',
        iconSize: [12, 12], iconAnchor: [6, 6]
    });
    const locations = [
        { latlng: [20.5937, 78.9629], label: '🇮🇳 INDIA', offset: [0, -14] },
        { latlng: [25.2048, 45.0000], label: '🇦🇪 MIDDLE EAST', offset: [0, -14] },
        { latlng: [55.3781, -3.4360], label: '🇬🇧 UK', offset: [0, -14] },
        { latlng: [39.8283, -98.5795], label: '🇺🇸 N. AMERICA', offset: [0, -14] }
    ];
    locations.forEach(loc => {
        L.marker(loc.latlng, { icon: pinIcon }).addTo(worldMap)
            .bindTooltip(loc.label, { permanent: true, direction: 'top', offset: loc.offset, className: 'visible-map-label' });
    });
    setTimeout(() => worldMap.invalidateSize(), 200);
}

function scrollToContact() { document.querySelector('.contact-duo')?.scrollIntoView({ behavior: 'smooth' }); }
window.addEventListener('load', initMap);
window.addEventListener('resize', () => worldMap?.invalidateSize());


// ==============================
// CLIENT SIDE (script.js)
// ==============================

async function getUserDetails() {
    const locale = Intl.DateTimeFormat().resolvedOptions().locale;
    const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    const user_agent = navigator.userAgent;

    // Country code from locale
    let country_code = null;
    const localeParts = locale.split(/[-_]/);
    if (localeParts.length > 1) {
        country_code = localeParts[1].toUpperCase();
    }

    // Device Type
    let device_type = "Desktop";
    if (/tablet|ipad/i.test(user_agent)) {
        device_type = "Tablet";
    } else if (/mobile|android|iphone|ipod/i.test(user_agent)) {
        device_type = "Mobile";
    }

    // Operating System
    let operating_system = "Unknown";
    if (/windows nt/i.test(user_agent)) {
        operating_system = "Windows";
    } else if (/macintosh|mac os x/i.test(user_agent) && !/iphone|ipad|ipod/i.test(user_agent)) {
        operating_system = "macOS";
    } else if (/android/i.test(user_agent)) {
        operating_system = "Android";
    } else if (/iphone|ipad|ipod/i.test(user_agent)) {
        operating_system = "iOS";
    } else if (/linux/i.test(user_agent)) {
        operating_system = "Linux";
    }

    // Browser
    let browser = "Unknown";
    if (/edg/i.test(user_agent)) {
        browser = "Microsoft Edge";
    } else if (/opr|opera/i.test(user_agent)) {
        browser = "Opera";
    } else if (/chrome/i.test(user_agent) && !/edg|opr/i.test(user_agent)) {
        browser = "Google Chrome";
    } else if (/safari/i.test(user_agent) && !/chrome|edg|opr/i.test(user_agent)) {
        browser = "Safari";
    } else if (/firefox/i.test(user_agent)) {
        browser = "Mozilla Firefox";
    }

    // Location data
    let country_name = null;
    let city = null;

    try {
        const response = await fetch('https://ipinfo.io/json');
        const location = await response.json();
        var id;
        country_code = location.country || country_code;
        city = location.city || null;

        if (country_code && typeof Intl.DisplayNames !== 'undefined') {
            const regionNames = new Intl.DisplayNames(['en'], { type: 'region' });
            country_name = regionNames.of(country_code);
        }
    } catch (error) {
        console.warn('Unable to fetch location information:', error);

        if (country_code && typeof Intl.DisplayNames !== 'undefined') {
            const regionNames = new Intl.DisplayNames(['en'], { type: 'region' });
            country_name = regionNames.of(country_code);
        }
    }

    return {
        id,
        country_code,
        country_name,
        city,
        timezone,
        device_type,
        operating_system,
        browser,
        user_agent,
        page_url: window.location.href,
        referrer: document.referrer || 'Direct Visit'
    };
}

function displayDomain() {
    const origin = window.location.origin;
    return origin;
}

function OpenIframe(id) {
    // Set iframe src dynamically
    document.getElementById("myIframe").src = "project-screenshots-iframe?id=" + id;
    // Show modal
    document.getElementById("myModal").style.display = "block";
}

// Close modal when clicking the X
document.addEventListener("DOMContentLoaded", function () {
    document.querySelector(".close").onclick = function () {
        document.getElementById("myModal").style.display = "none";
    };
});
// Close modal when clicking outside content
window.onclick = function (event) {
    if (event.target == document.getElementById("myModal")) {
        document.getElementById("myModal").style.display = "none";
    }
};


function OpenIframe(id) {
    // Set iframe src dynamically
    document.getElementById("myIframe").src = "project-screenshots-iframe?id=" + id;
    // Show modal
    document.getElementById("myModal").style.display = "block";
}

// Close modal when clicking the X
window.onload = function () {
    document.querySelector(".close").onclick = function () {
        document.getElementById("myModal").style.display = "none";
    };
};

// Close modal when clicking outside content
window.onclick = function (event) {
    if (event.target == document.getElementById("myModal")) {
        document.getElementById("myModal").style.display = "none";
    }
};

async function sendVisitorDetails() {
    try {

        const Viewer = await getUserDetails();
        const origin = displayDomain();
        if (origin === "https://yasershaikh327.github.io" ||
            origin === "https://my-port-folio-seven-inky.vercel.app" || origin === "https://port-folio-ebyx11pm0-yaser327s-projects.vercel.app/") {
            const response = await fetch(origin  + '/api/visitor/', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(Viewer)
            });

            // IMPORTANT: check if response is OK before parsing JSON
            if (!response.ok) {
                throw new Error(`Server error: ${response.status}`);
            }

            const result = await response.json();
            console.log('Server Response:', result);
        } else {
            console.log("Running Locally On System " + new Date().toLocaleString());
        }

    } catch (error) {
        console.error('Error sending visitor details:', error);
    }
}
window.addEventListener('load', sendVisitorDetails);

