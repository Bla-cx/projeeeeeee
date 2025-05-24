// Enhanced DataTable Helper - Version 2.0
// Simplified and more reliable DataTable initialization

(function() {
    'use strict';
    
    // Global namespace
    window.BILETINIAYIR = window.BILETINIAYIR || {};
    window.BILETINIAYIR.dataTableManager = {
        initialized: false,
        tables: new Set(),
          // Initialize all DataTables when ready
        init: function() {
            if (this.initialized) {
                console.log('🔄 DataTable manager already initialized');
                return;
            }
            
            console.log('🚀 Initializing DataTable Manager');
            this.waitForDependencies();
        },
        
        // Wait for jQuery and DataTables to be available
        waitForDependencies: function() {
            const self = this;
              if (typeof $ === 'undefined' || typeof $.fn.DataTable === 'undefined') {
                console.log('⏳ Waiting for jQuery/DataTables...');
                setTimeout(() => self.waitForDependencies(), 100);
                return;
            }
            
            console.log('✅ Dependencies ready - jQuery:', $.fn.jquery, 'DataTables:', $.fn.DataTable.version);
            this.setupDefaults();
            this.initializeTables();
            this.initialized = true;
        },
          // Set DataTable defaults - CORS-free version
        setupDefaults: function() {
            $.extend(true, $.fn.DataTable.defaults, {
                "language": window.dataTableTurkish || {
                    "search": "Ara:",
                    "lengthMenu": "_MENU_ kayıt göster",
                    "info": "_TOTAL_ kayıttan _START_ - _END_ arası",
                    "paginate": {
                        "first": "İlk",
                        "last": "Son", 
                        "next": "Sonraki",
                        "previous": "Önceki"
                    },
                    "emptyTable": "Tabloda veri yok",
                    "zeroRecords": "Eşleşen kayıt bulunamadı"
                },
                "pageLength": 10,
                "ordering": true,
                "responsive": true,
                "autoWidth": false,
                "processing": true,
                "deferRender": true
            });
        },
          // Initialize all tables on the page
        initializeTables: function() {
            const self = this;
            const tableSelectors = 'table.datatable, table.dataTable';
            const tables = document.querySelectorAll(tableSelectors);
            
            console.log(`📊 Found ${tables.length} tables to initialize`);
            
            tables.forEach(function(table, index) {
                self.initializeTable(table, index);
            });
        },
        
        // Initialize a single table
        initializeTable: function(table, index) {
            const tableId = table.id || `auto-datatable-${index}`;
            
            // Set ID if not present
            if (!table.id) {
                table.id = tableId;
            }
            
            // Skip if already initialized
            if (this.tables.has(tableId)) {
                console.log(`Table ${tableId} already initialized`);
                return;
            }
            
            try {
                // Check if DataTable is already initialized
                if ($.fn.DataTable.isDataTable(table)) {
                    console.log(`Table ${tableId} already has DataTable`);
                    this.tables.add(tableId);
                    return;
                }
                  console.log(`🔄 Initializing table: ${tableId}`);
                
                // Initialize the DataTable
                $(table).DataTable();
                this.tables.add(tableId);
                
                console.log(`✅ Successfully initialized table: ${tableId}`);
                
            } catch (error) {
                console.error(`Error initializing table ${tableId}:`, error);
            }
        },
        
        // Reinitialize tables (for AJAX content)
        reinitialize: function(selector) {
            console.log('Reinitializing tables:', selector || 'all');
            
            if (selector) {
                // Reinitialize specific tables
                $(selector).each((index, table) => {
                    const tableId = table.id;
                    if (tableId && this.tables.has(tableId)) {
                        this.destroyTable(table);
                    }
                    this.initializeTable(table, index);
                });
            } else {
                // Reinitialize all tables
                this.initializeTables();
            }
        },
        
        // Destroy a specific table
        destroyTable: function(table) {
            const tableId = table.id;
            
            try {
                if ($.fn.DataTable.isDataTable(table)) {
                    console.log(`Destroying table: ${tableId}`);
                    $(table).DataTable().destroy();
                    this.tables.delete(tableId);
                }
            } catch (error) {
                console.error(`Error destroying table ${tableId}:`, error);
            }
        }
    };
      // Auto-initialize when DOM is ready - REMOVED FOR SAFETY
    // document.addEventListener('DOMContentLoaded', function() {
    //     window.BILETINIAYIR.dataTableManager.init();
    // });
    
    // Handle modal tables
    $(document).on('shown.bs.modal', function(e) {
        setTimeout(function() {
            const modal = $(e.target);
            const tables = modal.find('table.datatable, table.dataTable');
            
            if (tables.length > 0) {
                console.log('Initializing modal tables');
                tables.each(function(index) {
                    window.BILETINIAYIR.dataTableManager.initializeTable(this, index);
                });
            }
        }, 100);
    });
      // Handle AJAX loaded content - COMMENTED OUT FOR SAFETY
    // $(document).on('ajaxComplete', function() {
    //     setTimeout(function() {
    //         const newTables = $('table.datatable:not(.dataTable), table.dataTable:not(.dataTable)');
    //         
    //         if (newTables.length > 0) {
    //             console.log('Initializing AJAX loaded tables');
    //             newTables.each(function(index) {
    //                 window.BILETINIAYIR.dataTableManager.initializeTable(this, index);
    //             });
    //         }
    //     }, 200);
    // });
      // Global access functions
    window.initializeAllDataTables = function() {
        window.BILETINIAYIR.dataTableManager.initializeTables();
    };
    
    window.reinitializeDataTables = function(selector) {
        window.BILETINIAYIR.dataTableManager.reinitialize(selector);
    };
    
    // Handle legacy reinitialize-datatables events for backwards compatibility
    $(document).off('reinitialize-datatables').on('reinitialize-datatables', function(e, selector) {
        console.log('🔄 Legacy reinitialize-datatables event triggered for:', selector);
        try {
            if (selector) {
                window.BILETINIAYIR.dataTableManager.reinitialize(selector);
            } else {
                window.BILETINIAYIR.dataTableManager.initializeTables();
            }
        } catch (error) {
            console.error('❌ Error in legacy reinitialize event:', error);
        }
    });
    
})();
