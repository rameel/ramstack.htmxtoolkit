(() => {
    const menu_toggle = document.querySelector("[data-menu-toggle]");
    menu_toggle.addEventListener("click", () => document.body.classList.toggle("sidebar-open"));

    document.querySelectorAll(".nav-link").forEach(link => {
        link.pathname === window.location.pathname && link.classList.add("active");
    });

    document.addEventListener("customEvent", e => append_event("Custom event", e.detail.message));
    document.addEventListener("logEvent", e => append_event("Log event", e.detail.message));

    function append_event(name, message) {
        const el = document.querySelector("#event-log");
        el.insertAdjacentHTML("beforeend", `<p><b>${name}:</b> ${message}</p>`);
    }
})();
