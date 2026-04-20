// Dropdown Menu Isolation and Animation Stabilization (Legacy & IPv6 Compatible)
$(document).ready(function() {
    // When a dropdown is shown (e.g., Task Status menu)
    $(document).on('show.bs.dropdown', '.task-card .dropdown', function () {
        $('body').addClass('menu-open');
        $(this).closest('.task-card').addClass('active-card');
    });

    // When the dropdown is hidden
    $(document).on('hidden.bs.dropdown', '.task-card .dropdown', function () {
        $('body').removeClass('menu-open');
        $('.task-card').removeClass('active-card');
    });
});
