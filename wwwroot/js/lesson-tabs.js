document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll("[data-lesson-tab]");
    const panels = document.querySelectorAll("[data-lesson-panel]");

    if (!tabs.length || !panels.length) {
        return;
    }

    tabs.forEach(function (tab) {
        tab.addEventListener("click", function () {
            const target = tab.dataset.lessonTab;

            tabs.forEach(function (item) {
                item.classList.toggle("active", item === tab);
            });

            panels.forEach(function (panel) {
                panel.classList.toggle(
                    "active",
                    panel.dataset.lessonPanel === target
                );
            });
        });
    });
});
