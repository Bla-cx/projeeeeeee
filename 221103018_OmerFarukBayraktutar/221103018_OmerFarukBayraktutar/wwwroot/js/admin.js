// Admin Panel JavaScript
document.addEventListener('DOMContentLoaded', function() {
    console.log("Admin panel JavaScript loaded - NO DataTable initialization here"); 

    // Only handle non-DataTables functionality here
    // =============================================
    
    // Make sure jQuery is loaded
    if (typeof jQuery !== 'undefined') {
        console.log("jQuery version loaded in admin.js:", jQuery.fn.jquery);
        
        // We will NOT attempt to initialize any DataTables
        // datatable-helper.js is solely responsible for that
        
        // Toggle sidebar - Use both jQuery and native JS methods for redundancy
        $('#sidebarCollapse').on('click', function(e) {
            e.preventDefault();
            $('#sidebar').toggleClass('active');
            console.log("Sidebar toggle clicked via jQuery"); // Debugging log
        });
        
        // Tooltip initialization
        if (typeof $().tooltip === 'function') {
            $('[data-toggle="tooltip"]').tooltip();
        }

        // Ensure navigation links work correctly
        $('#sidebar ul li a').each(function() {
            // Exclude logout and toggle links
            if (!$(this).hasClass('logout-link') && $(this).attr('href') !== 'javascript:void(0);') {
                $(this).off('click').on('click', function(e) {
                    // Let the link work normally
                    console.log("Navigation link clicked:", $(this).attr('href'));
                });
            }
        });

        // CRUD form submissions
        $('form.crud-form').off('submit').on('submit', function(e) {
            console.log("Form submission detected:", $(this).attr('action'));
        });

        // Confirm delete action
        $('.delete-confirm').off('click').on('click', function(e) {
            if (!confirm('Bu öğeyi silmek istediğinizden emin misiniz?')) {
                e.preventDefault();
                return false;
            }
            console.log("Delete confirmed for element");
        });
        
        // Make sure all buttons are clickable
        $('button:not([type="submit"]), .btn:not([type="submit"])').off('click').on('click', function(e) {
            if ($(this).data('bs-toggle') || $(this).data('toggle')) {
                // Let Bootstrap handle toggle buttons
                return true;
            }
            console.log("Button clicked:", $(this).text()); // Debugging log
        });
    } else {
        console.error("jQuery is not loaded! Admin panel functionality will be limited.");
        
        // Pure JavaScript fallback for sidebar toggle
        document.getElementById('sidebarCollapse')?.addEventListener('click', function(e) {
            e.preventDefault();
            document.getElementById('sidebar')?.classList.toggle('active');
            console.log("Sidebar toggle clicked via native JS fallback"); // Debugging log
        });
        
        // Handle delete confirmations in vanilla JS
        document.querySelectorAll('.delete-confirm').forEach(elem => {
            elem.addEventListener('click', function(e) {
                if (!confirm('Bu öğeyi silmek istediğinizden emin misiniz?')) {
                    e.preventDefault();
                    return false;
                }
            });
        });
    }
    
    // Fix for sidebar menu button not working
    const sidebarToggleBtn = document.getElementById('sidebarCollapse');
    if (sidebarToggleBtn) {
        console.log("Setting up direct sidebar toggle event");
        sidebarToggleBtn.addEventListener('click', function(e) {
            e.preventDefault();
            const sidebar = document.getElementById('sidebar');
            if (sidebar) {
                sidebar.classList.toggle('active');
                console.log("Sidebar toggled directly");
            }
        });
    }
    
    // Fix AJAX links in the application
    document.querySelectorAll('[data-ajax="true"]').forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const url = this.href;
            const target = this.getAttribute('data-ajax-target');
            
            console.log(`AJAX link clicked: ${url} -> ${target}`);
            
            if (url && target) {
                fetch(url)
                    .then(response => response.text())
                    .then(data => {
                        document.querySelector(target).innerHTML = data;
                        console.log("AJAX content loaded successfully");
                        
                        // When content is loaded, use the new global initializer
                        if (window.initializeAllDataTables) {
                            console.log("Initializing new tables after AJAX content loaded");
                            window.initializeAllDataTables();
                        }
                    })
                    .catch(error => {
                        console.error("AJAX request failed:", error);
                    });
            }
        });
    });
}); 