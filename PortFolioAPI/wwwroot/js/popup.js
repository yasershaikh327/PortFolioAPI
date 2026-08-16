document.addEventListener("DOMContentLoaded", function () {
    const slides = document.getElementById("slides");
    const nav = document.getElementById("navigation");
    let currentIndex = 0;

    function getParameterByName(name) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(name);
    }

    const idValue = getParameterByName("id");
    let totalImages = 0;

    // Configure image counts per project
    if (idValue === "expenses_tracker") {
        totalImages = 13;
    } else if (idValue === "sales_dashboard") {
        totalImages = 3;
    } else if (idValue === "inventory_manager") {
        totalImages = 4;
    }

    if (idValue && totalImages > 0) {
        for (let i = 1; i <= totalImages; i++) {
            const img = document.createElement("img");
            img.src = `project_screenshots/${idValue}/${i}.png`;
            img.alt = `${idValue} screenshot ${i}`;
            slides.appendChild(img);
        }

        const images = slides.querySelectorAll("img");

        images.forEach((_, index) => {
            const btn = document.createElement("button");
            btn.textContent = index + 1;
            btn.addEventListener("click", () => showSlide(index));
            nav.appendChild(btn);
        });

        function showSlide(index) {
            currentIndex = index;
            slides.style.transform = `translateX(-${index * 100}%)`;
            updateButtons();
        }

        function updateButtons() {
            const buttons = nav.querySelectorAll("button");
            buttons.forEach((btn, idx) => {
                btn.classList.toggle("active", idx === currentIndex);
            });
        }

        // ✅ Prev button
        document.querySelector(".prev").addEventListener("click", function () {
            currentIndex = (currentIndex - 1 + images.length) % images.length;
            showSlide(currentIndex);
        });

        // ✅ Next button
        document.querySelector(".next").addEventListener("click", function () {
            currentIndex = (currentIndex + 1) % images.length;
            showSlide(currentIndex);
        });

        // Initialize
        showSlide(0);
    } else {
        console.log("No valid ID found in URL or no images configured.");
    }
});