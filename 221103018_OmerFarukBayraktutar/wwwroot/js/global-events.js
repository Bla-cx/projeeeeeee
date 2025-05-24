// Site geneli için global click event çözümü
document.addEventListener('DOMContentLoaded', function() {
    console.log("Global events handler loaded - FIXED VERSION!");
    
    // Tüm form gönderimlerini izle ama engelleme!
    document.querySelectorAll('form').forEach(function(form) {
        form.addEventListener('submit', function(e) {
            console.log('Form gönderimi: ', this.action);
            // Form submit engellenmeyecek, sadece log için
        });
    });

    // Tüm butonları izle ve tıklanabilirliği sağla
    document.querySelectorAll('button, .btn').forEach(function(button) {
        // Bilet adedi +/- butonlarını atla
        if (button.id === "biletArtir" || button.id === "biletAzalt" || 
            button.hasAttribute("onclick")) {
            console.log("Bu buton global event'ten muaf tutuldu:", button.id || button.className);
            return;
        }
        
        // Zaten bir click handler'ı varsa bunu bozmamak için
        // yeni bir handler eklemek yerine mevcut handler'ları koruyoruz
        button.addEventListener('click', function(e) {
            console.log('Button tıklandı: ', this.textContent.trim());
            
            // Sadece belirli submit butonları için formları manuel gönder
            if (this.type === 'submit' && !this.form) {
                // Form olmayan submit butonları için
                // En yakın form elementini bul ve gönder
                const nearestForm = this.closest('form');
                if (nearestForm) {
                    nearestForm.submit();
                }
            }
        });
    });

    // Tüm linkleri izle
    document.querySelectorAll('a').forEach(function(link) {
        link.addEventListener('click', function(e) {
            const href = this.getAttribute('href');
            console.log('Link tıklandı: ', href);
            
            // # ile başlayan linkler için
            if (href && href.startsWith('#')) {
                // Boş # link kontrolü
                if (href === '#') {
                    console.log('Boş # linki tespit edildi, işlem yapılmayacak');
                    e.preventDefault();
                    return;
                }
                
                e.preventDefault();
                // Anchor linkleri için manuel scroll
                try {
                    const targetElement = document.querySelector(href);
                    if (targetElement) {
                        targetElement.scrollIntoView({
                            behavior: 'smooth'
                        });
                    } else {
                        console.warn(`'${href}' seçicisi ile eşleşen öğe bulunamadı`);
                    }
                } catch (err) {
                    console.error(`Geçersiz seçici hatası: ${href}`, err);
                }
                return;
            }
            
            // JavaScript: protokolü ile başlayan linkler için özel işlem
            if (href && href.startsWith('javascript:')) {
                e.preventDefault();
                try {
                    // href'teki javascript kodunu çalıştır
                    const jsCode = href.substring('javascript:'.length);
                    eval(jsCode);
                } catch (error) {
                    console.error('JavaScript çalıştırma hatası:', error);
                }
                return;
            }
            
            // Asp.net handler linkleri için
            if (this.dataset.handler || this.classList.contains('asp-action') || this.hasAttribute('asp-action')) {
                console.log('ASP.NET link tespit edildi');
            }
        });
    });
    
    // Modal ve popuplar için ek destek
    const modalOpenButtons = document.querySelectorAll('[data-toggle="modal"], [data-bs-toggle="modal"], .silme-onayi-btn');
    modalOpenButtons.forEach(button => {
        button.addEventListener('click', function() {
            console.log('Modal butonu tıklandı:', this);
            const target = this.getAttribute('data-target') || this.getAttribute('data-bs-target');
            if (target) {
                console.log('Modal hedefi:', target);
                const modal = document.querySelector(target);
                if (modal) {
                    console.log('Modal bulundu, gösteriliyor');
                    // Bootstrap 5 için
                    if (typeof bootstrap !== 'undefined' && typeof bootstrap.Modal === 'function') {
                        const bsModal = new bootstrap.Modal(modal);
                        bsModal.show();
                    } 
                    // Bootstrap 4 için
                    else if (typeof $ !== 'undefined' && typeof $.fn.modal === 'function') {
                        $(modal).modal('show');
                    } 
                    // Manuel modal gösterme
                    else {
                        console.log('Bootstrap veya jQuery bulunamadı, manuel modal gösterme');
                        modal.style.display = 'block';
                        modal.classList.add('show');
                        document.body.classList.add('modal-open');
                    }
                } else {
                    console.error('Modal bulunamadı:', target);
                }
            } else {
                // Silme butonları için özel işlem
                if (this.classList.contains('silme-onayi-btn')) {
                    console.log('Silme onayı butonu tıklandı');
                    const etkinlikId = this.getAttribute('data-etkinlik-id');
                    const silForm = document.getElementById('silForm');
                    const silmeOnayi = document.getElementById('silmeOnayi');
                    
                    if (etkinlikId && silForm) {
                        console.log('Etkinlik ID:', etkinlikId);
                        silForm.action = `/Organizator/Etkinlik/Sil/${etkinlikId}`;
                        
                        if (typeof bootstrap !== 'undefined' && typeof bootstrap.Modal === 'function') {
                            const silmeModal = new bootstrap.Modal(silmeOnayi);
                            silmeModal.show();
                        } else if (typeof $ !== 'undefined' && typeof $.fn.modal === 'function') {
                            $(silmeOnayi).modal('show');
                        } else {
                            console.error('Bootstrap veya jQuery bulunamadı');
                            if (confirm('Bu etkinliği silmek istediğinize emin misiniz? Bu işlem geri alınamaz.')) {
                                silForm.submit();
                            }
                        }
                    } else {
                        console.error('Silme formu bulunamadı veya etkinlikId eksik');
                        alert('Silme işlemi için gerekli bilgiler eksik.');
                    }
                }
            }
        });
    });
    
    // DataTable'ları global-events.js'de başlatmıyoruz
    // Bu işlem datatable-helper.js tarafından yapılıyor
    console.log('DataTable initialization is now handled by datatable-helper.js');
}); 