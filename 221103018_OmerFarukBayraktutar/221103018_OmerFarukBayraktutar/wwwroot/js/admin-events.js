// Admin Panel Event Handling JavaScript
document.addEventListener('DOMContentLoaded', function() {
    console.log("Admin events script loaded");
    
    // Override default click behavior to debug any issues
    document.body.addEventListener('click', function(e) {
        console.log("Click detected on:", e.target);
        
        // Check if click was on a link
        if (e.target.tagName === 'A' || e.target.closest('a')) {
            console.log("Link clicked:", e.target.href || e.target.closest('a').href);
        }
        
        // Check if click was on a button
        if (e.target.tagName === 'BUTTON' || e.target.closest('button')) {
            console.log("Button clicked:", e.target.textContent || e.target.closest('button').textContent);
        }
    }, true); // Use capture phase to ensure this runs first
    
    // Add manual event listeners for sidebar items
    const sidebarItems = document.querySelectorAll('#sidebar ul li a');
    sidebarItems.forEach(function(item) {
        item.addEventListener('click', function(e) {
            console.log("Sidebar item clicked via direct handler:", this.href);
        });
    });
    
    // Add manual event listener for sidebar toggle
    const sidebarToggle = document.getElementById('sidebarCollapse');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function(e) {
            console.log("Sidebar toggle clicked via direct handler");
            const sidebar = document.getElementById('sidebar');
            if (sidebar) {
                sidebar.classList.toggle('active');
                console.log("Sidebar toggle class applied");
            } else {
                console.error("Sidebar element not found");
            }
        });
    } else {
        console.error("Sidebar toggle button not found");
    }
}); 