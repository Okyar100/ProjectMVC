
// Icazeler checkbox'u - Hamisini Sec.
document.addEventListener("DOMContentLoaded", function () {
    var selectAll = document.getElementById("selectAllPermissions");
    if (selectAll) {
        var checkboxes = document.querySelectorAll(".permission-checkbox");

        selectAll.addEventListener("change", function () {
            checkboxes.forEach(function (cb) { cb.checked = selectAll.checked; });
        });

        checkboxes.forEach(function (cb) {
            cb.addEventListener("change", function () {
                selectAll.checked = Array.from(checkboxes).every(function (c) { return c.checked; });
            });
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    var alert = document.getElementById("successAlert");
    if (alert) {
        setTimeout(function () { alert.style.display = "none"; }, 4500);
    }
});



(function () {
    var htmlEl = document.documentElement;
    var saved = localStorage.getItem("fhn-theme");
    if (saved === "dark") htmlEl.setAttribute("data-theme", "dark");

    document.addEventListener("DOMContentLoaded", function () {
        var btn = document.getElementById("themeToggleBtn");
        var icon = document.getElementById("themeIcon");
        if (!btn) return;

        function updateLabel() {
            var isDark = htmlEl.getAttribute("data-theme") === "dark";
            icon.innerHTML = isDark ? "&#9728;" : "&#127769;";
            btn.title = isDark ? "İşıqlı tema" : "Tünd tema";
        }
        updateLabel();

        btn.addEventListener("click", function () {
            var isDark = htmlEl.getAttribute("data-theme") === "dark";
            if (isDark) {
                htmlEl.removeAttribute("data-theme");
                localStorage.setItem("fhn-theme", "light");
            } else {
                htmlEl.setAttribute("data-theme", "dark");
                localStorage.setItem("fhn-theme", "dark");
            }
            updateLabel();
        });
    });
})();

// Fin kod hemise boyuk herfle yazilsin
document.addEventListener("DOMContentLoaded", function () {
    var finInput = document.getElementById("FinKod");
    if (finInput) {
        finInput.addEventListener("input", function () {
            var cursorPos = finInput.selectionStart;
            finInput.value = finInput.value.toUpperCase();
            finInput.setSelectionRange(cursorPos, cursorPos);
        });
    }
});