// Login page script
const LOGIN_API_URL = '/api/login'; // <-- set to your real auth endpoint, e.g. "https://port-folio-api-one.vercel.app/api/login"

const el = (id) => document.getElementById(id);

function initLoginPage() {
    const requiredIds = ["loginForm", "email", "password", "emailError", "passwordError", "submitBtn", "formMessage", "togglePassword"];
    const missing = requiredIds.filter((id) => !el(id));
    if (missing.length) return; // not on the login page

    el("togglePassword").addEventListener("click", () => {
        const pw = el("password");
        const btn = el("togglePassword");
        const showing = pw.type === "text";
        pw.type = showing ? "password" : "text";
        btn.textContent = showing ? "Show" : "Hide";
    });

    el("loginForm").addEventListener("submit", handleSubmit);
    el("email").addEventListener("input", () => clearFieldError("email"));
    el("password").addEventListener("input", () => clearFieldError("password"));
}

function clearFieldError(field) {
    el(`${field}Error`).textContent = "";
}

function validate() {
    let valid = true;
    const email = el("email").value.trim();
    const password = el("password").value;

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email) {
        el("emailError").textContent = "Email is required.";
        valid = false;
    } else if (!emailPattern.test(email)) {
        el("emailError").textContent = "Enter a valid email address.";
        valid = false;
    } else {
        clearFieldError("email");
    }

    if (!password) {
        el("passwordError").textContent = "Password is required.";
        valid = false;
    } else if (password.length < 6) {
        el("passwordError").textContent = "Password must be at least 6 characters.";
        valid = false;
    } else {
        clearFieldError("password");
    }

    return valid;
}

async function handleSubmit(e) {
    e.preventDefault();
    const messageEl = el("formMessage");
    messageEl.textContent = "";
    messageEl.classList.remove("lp-error", "lp-success");

    if (!validate()) return;

    const email = el("email").value.trim();
    const password = el("password").value;
    const remember = el("remember").checked;

    const submitBtn = el("submitBtn");
    submitBtn.disabled = true;
    submitBtn.textContent = "Signing in…";

    if (!LOGIN_API_URL) {
        // No backend wired up yet — demo mode.
        await new Promise((r) => setTimeout(r, 500));
        messageEl.textContent = "Login endpoint not configured yet (demo mode).";
        messageEl.classList.add("lp-error");
        submitBtn.disabled = false;
        submitBtn.textContent = "Sign In";
        return;
    }

    try {
        const res = await fetch(LOGIN_API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password, remember })
        });

        if (!res.ok) {
            const data = await res.json().catch(() => ({}));
            throw new Error(data.message || `Login failed (HTTP ${res.status})`);
        }

        const data = await res.json();
        messageEl.textContent = "Signed in successfully. Redirecting…";
        messageEl.classList.add("lp-success");

        if (data.token) {
            // Store however your app expects — e.g. a cookie set by the server,
            // or sessionStorage if you don't need it to persist across tabs.
            sessionStorage.setItem("jwtToken", data.token);
        }
        if (data.token && data.token !== "") {
            await fetch('/Admin/SetToken', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    token: data.token
                })
            });

            window.location.href = '/Admin/Portfolio';
        }

    } catch (err) {
        messageEl.textContent = err.message || "Something went wrong. Please try again.";
        messageEl.classList.add("lp-error");
    } finally {
        submitBtn.disabled = false;
        submitBtn.textContent = "Sign In";
    }
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initLoginPage);
} else {
    initLoginPage();
}