// DataTable Helper - COMPLETELY REWRITTEN FOR RELIABILITY
// This script ensures DataTables are only initialized once

// Global tracker object
window.BILETINIAYIR = window.BILETINIAYIR || {};
window.BILETINIAYIR.initializedTables = {};

// Wait for document ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM fully loaded - Starting DataTable initialization process');
    initializeDataTablesWhenReady();
});

// Main initialization function with retry mechanism
function initializeDataTablesWhenReady() {
    console.log('Checking if dependencies are ready for DataTables initialization');
    
    // Wait for jQuery and DataTables to be available
    if (typeof $ === 'undefined' || typeof $.fn.DataTable === 'undefined') {
        console.log('jQuery or DataTables not loaded yet, waiting 100ms...');
        setTimeout(initializeDataTablesWhenReady, 100);
        return;
    }
    
    console.log('Dependencies loaded! Starting DataTables initialization');
    console.log('jQuery version:', $.fn.jquery);
    console.log('DataTables version:', $.fn.DataTable.version);
    
    // Now initialize all tables
    initializeAllDataTables();
}

// Core initialization function
function initializeAllDataTables() {
    // Set default options first
    $.extend(true, $.fn.DataTable.defaults, {
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Turkish.json"
        },
        "pageLength": 10,
        "ordering": true,
        "responsive": true,
        "autoWidth": false
    });
    
    // IMPORTANT: First destroy ALL existing DataTables to prevent "Cannot reinitialize" errors
    console.log('Pre-initialization cleanup: Destroying any existing DataTables');
    $('table.datatable, table.dataTable').each(function() {
        var tableId = this.id || 'unnamed-table';
        try {
            if ($.fn.DataTable.isDataTable(this)) {
                console.log('Destroying existing DataTable:', tableId);
                $(this).DataTable().destroy();
                delete window.BILETINIAYIR.initializedTables[tableId];
                console.log('Successfully destroyed DataTable:', tableId);
            }
        } catch (e) {
            console.error('Error destroying DataTable', tableId, ':', e);
        }
    });
    
    // Reset tracking object
    window.BILETINIAYIR.initializedTables = {};
    
    // Now initialize all tables
    var tables = document.querySelectorAll('table.datatable, table.dataTable');
    console.log('Found', tables.length, 'tables to initialize');
    
    tables.forEach(function(table) {
        var tableId = table.id || ('datatable-' + Math.random().toString(36).substring(2, 10));
        
        // If table has no ID, set one for tracking
        if (!table.id) {
            table.id = tableId;
        }
        
        try {
            // Skip if somehow already initialized (should not happen after destroy above)
            if (window.BILETINIAYIR.initializedTables[tableId]) {
                console.log('Table already tracked as initialized:', tableId);
                return;
            }
            
            if ($.fn.DataTable.isDataTable(table)) {
                console.log('Table already initialized according to DataTables:', tableId);
                window.BILETINIAYIR.initializedTables[tableId] = true;
                return;
            }
            
            // Initialize the table
            console.log('Initializing table:', tableId);
            $(table).DataTable();
            window.BILETINIAYIR.initializedTables[tableId] = true;
            console.log('Successfully initialized table:', tableId);
        } catch (err) {
            console.error('Error initializing table', tableId, ':', err);
        }
    });
}

// Make the function globally accessible so it can be called from other scripts
window.initializeAllDataTables = initializeAllDataTables;

// Global function to safely reinitialize a specific table or all tables
window.safeReinitializeDataTable = function(selector) {
    console.log('Safe reinitialization requested for:', selector || 'all tables');
    
    // If selector is provided, destroy just those tables
    if (selector) {
        $(selector).each(function() {
            var table = this;
            var tableId = table.id || 'unnamed-table';
            
            try {
                if ($.fn.DataTable.isDataTable(table)) {
                    console.log('Destroying table for reinitialization:', tableId);
                    $(table).DataTable().destroy();
                    delete window.BILETINIAYIR.initializedTables[tableId];
                }
            } catch (err) {
                console.error('Error destroying table', tableId, ':', err);
            }
        });
        
        // Now initialize those specific tables
        $(selector).each(function() {
            var table = this;
            var tableId = table.id || ('datatable-' + Math.random().toString(36).substring(2, 10));
            
            if (!table.id) {
                table.id = tableId;
            }
            
            try {
                console.log('Reinitializing table:', tableId);
                $(table).DataTable();
                window.BILETINIAYIR.initializedTables[tableId] = true;
                console.log('Successfully reinitialized table:', tableId);
            } catch (err) {
                console.error('Error reinitializing table', tableId, ':', err);
            }
        });
    } 
    // Otherwise reinitialize all tables
    else {
        initializeAllDataTables();
    }
};

// Custom event for reinitialization
$(document).off('reinitialize-datatables').on('reinitialize-datatables', function(e, selector) {
    console.log('DataTable reinitialization event received');
    window.safeReinitializeDataTable(selector);
});

// Handle tables in modals
$(document).off('shown.bs.modal').on('shown.bs.modal', function(e) {
    var modal = $(e.target);
    var tables = modal.find('table.datatable, table.dataTable');
    
    if (tables.length > 0) {
        console.log('Tables found in modal, initializing');
        tables.each(function() {
            var tableId = this.id || ('modal-datatable-' + Math.random().toString(36).substring(2, 10));
            if (!this.id) {
                this.id = tableId;
            }
            
            try {
                if ($.fn.DataTable.isDataTable(this)) {
                    console.log('Modal table already initialized:', tableId);
                    return;
                }
                
                console.log('Initializing modal table:', tableId);
                $(this).DataTable();
                window.BILETINIAYIR.initializedTables[tableId] = true;
            } catch (err) {
                console.error('Error initializing modal table', tableId, ':', err);
            }
        });
    }
});

// Handle tables loaded via AJAX
$(document).off('ajaxComplete').on('ajaxComplete', function() {
    setTimeout(function() {
        $('table.datatable:not(.dataTable), table.dataTable:not(.dataTable)').each(function() {
            var tableId = this.id || ('ajax-datatable-' + Math.random().toString(36).substring(2, 10));
            if (!this.id) {
                this.id = tableId;
            }
            
            try {
                if (window.BILETINIAYIR.initializedTables[tableId] || $.fn.DataTable.isDataTable(this)) {
                    console.log('AJAX loaded table already initialized:', tableId);
                    return;
                }
                
                console.log('Initializing AJAX loaded table:', tableId);
                $(this).DataTable();
                window.BILETINIAYIR.initializedTables[tableId] = true;
            } catch (err) {
                console.error('Error initializing AJAX loaded table', tableId, ':', err);
            }
        });
    }, 100);
}); 