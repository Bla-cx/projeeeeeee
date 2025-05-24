// Form İşlemleri Yardımcı Fonksiyonları
document.addEventListener('DOMContentLoaded', function() {
    console.log("Form helper loaded - FIXED VERSION");
    
    // FORM DOĞRULAMA VE GÖNDERİM İŞLEMLERİ
    // jQuery Unobtrusive ile çakışmayı önlemek için özel doğrulama kaldırıldı
    // Form gönderimlerinin düzgün çalışması için event.preventDefault() kaldırıldı
    
    // Edit/Delete butonlarını aktifleştir
    document.querySelectorAll('.btn-edit, .edit-link, .btn-update').forEach(function(button) {
        button.addEventListener('click', function(e) {
            console.log("Edit button clicked:", this);
            const url = this.getAttribute('href') || this.getAttribute('data-url');
            if (url) {
                console.log("Redirecting to:", url);
                window.location.href = url;
            }
        });
    });
    
    document.querySelectorAll('.btn-delete, .delete-link, .delete-confirm').forEach(function(button) {
        button.addEventListener('click', function(e) {
            console.log("Delete button clicked:", this);
            if (confirm('Bu öğeyi silmek istediğinizden emin misiniz?')) {
                const form = this.closest('form');
                if (form) {
                    console.log("Form submit:", form);
                    // Doğrudan form gönder - preventDefault YAPMA
                } else {
                    const url = this.getAttribute('href') || this.getAttribute('data-url');
                    if (url) {
                        console.log("Redirecting to:", url);
                        window.location.href = url;
                    }
                }
            } else {
                e.preventDefault();
                return false;
            }
        });
    });
    
    // AJAX FORM GÖNDERİMLERİ İÇİN DEBUG DESTEĞİ
    document.querySelectorAll('form[data-ajax="true"]').forEach(function(form) {
        console.log("Ajax form detected:", form);
        // AJAX form işlemlerini izleme (müdahale etmeden)
    });
}); 