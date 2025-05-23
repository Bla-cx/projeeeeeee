// Admin CRUD İşlemleri için Özel JavaScript
document.addEventListener('DOMContentLoaded', function() {
    console.log("Admin CRUD handler yüklendi");
    
    // Mevcut event listener'ları temizle
    function cleanupExistingHandlers() {
        // Submit butonlarından eski handler'ları kaldır
        document.querySelectorAll('button[type="submit"]').forEach(function(button) {
            const newButton = button.cloneNode(true);
            button.parentNode.replaceChild(newButton, button);
        });
    }
    
    // Admin sayfası kontrolü
    if (window.location.pathname.includes('/Admin/')) {
        cleanupExistingHandlers();
        setupAdminCrudHandlers();
    }
    
    function setupAdminCrudHandlers() {
        // EKLE butonları
        document.querySelectorAll('.btn-add, .btn-create, [href*="/Ekle"], [href*="/Create"]').forEach(function(button) {
            button.addEventListener('click', function(e) {
                console.log("Ekle butonu tıklandı");
                // Link ise normal davranışına izin ver
                if (this.tagName === 'A') {
                    return true;
                }
                // Form içindeyse submit et
                const form = this.closest('form');
                if (form) {
                    form.submit();
                }
            });
        });
        
        // DÜZENLE butonları
        document.querySelectorAll('.btn-edit, .btn-update, [href*="/Duzenle"], [href*="/Edit"]').forEach(function(button) {
            button.addEventListener('click', function(e) {
                console.log("Düzenle butonu tıklandı");
                // Link ise normal davranışına izin ver
                if (this.tagName === 'A') {
                    return true;
                }
                // Form içindeyse submit et
                const form = this.closest('form');
                if (form) {
                    form.submit();
                }
            });
        });
        
        // KAYDET butonları (form submit)
        document.querySelectorAll('button[type="submit"], input[type="submit"], .btn-save').forEach(function(button) {
            button.addEventListener('click', function(e) {
                console.log("Kaydet butonu tıklandı");
                
                const form = this.closest('form');
                if (form) {
                    // Form doğrulaması
                    if (form.checkValidity && !form.checkValidity()) {
                        form.reportValidity();
                        return false;
                    }
                    
                    // Formu gönder - preventDefault YAPMA!
                    console.log("Form gönderiliyor:", form.action);
                    // Form otomatik olarak gönderilecek
                }
            });
        });
        
        // SİL butonları
        document.querySelectorAll('.btn-delete, .btn-remove, [href*="/Sil"], [href*="/Delete"]').forEach(function(button) {
            button.addEventListener('click', function(e) {
                console.log("Sil butonu tıklandı");
                
                // Onay al
                if (!confirm('Bu kaydı silmek istediğinizden emin misiniz?')) {
                    e.preventDefault();
                    return false;
                }
                
                // Link ise devam et
                if (this.tagName === 'A') {
                    return true;
                }
                
                // Form içindeyse submit et
                const form = this.closest('form');
                if (form) {
                    form.submit();
                }
            });
        });
        
        // Ajax form gönderimi desteği (eğer kullanılıyorsa)
        document.querySelectorAll('form[data-ajax="true"]').forEach(function(form) {
            form.addEventListener('submit', function(e) {
                console.log("Ajax form gönderimi");
                // jQuery Unobtrusive Ajax varsa ona bırak
                if (typeof $.validator !== 'undefined') {
                    return true;
                }
            });
        });
    }
    
    // Debug için form bilgilerini logla
    document.querySelectorAll('form').forEach(function(form, index) {
        console.log(`Form ${index}:`, {
            action: form.action,
            method: form.method,
            id: form.id,
            classes: form.className
        });
    });
});