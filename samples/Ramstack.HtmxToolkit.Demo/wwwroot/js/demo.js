(() => {
    const menu_toggle = document.querySelector("[data-menu-toggle]");
    menu_toggle.addEventListener("click", () => document.body.classList.toggle("sidebar-open"));

    document.querySelectorAll(".nav-link").forEach(link => {
        link.pathname === window.location.pathname && link.classList.add("active");
    });
})();
