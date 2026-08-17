// Viet hanh dong truot thong tin co ban cua nguoi dung o header
document.addEventListener("DOMContentLoaded", function () {

    const userInfo =
        document.querySelector(".user-info");

    const userMenuButton =
        document.getElementById("userMenuButton");

    if (!userInfo || !userMenuButton) {
        return;
    }


    // Click nút user
    userMenuButton.addEventListener("click", function (event) {

        event.stopPropagation();

        const isOpen =
            userInfo.classList.toggle("open");

        userMenuButton.setAttribute(
            "aria-expanded",
            isOpen.toString()
        );

    });


    // Click ra ngoài -> đóng
    document.addEventListener("click", function (event) {

        if (!userInfo.contains(event.target)) {

            userInfo.classList.remove("open");

            userMenuButton.setAttribute(
                "aria-expanded",
                "false"
            );

        }

    });


    // ESC -> đóng
    document.addEventListener("keydown", function (event) {

        if (event.key === "Escape") {

            userInfo.classList.remove("open");

            userMenuButton.setAttribute(
                "aria-expanded",
                "false"
            );

        }

    });

});