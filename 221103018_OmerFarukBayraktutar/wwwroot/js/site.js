// Kredi kartı numarası formatlama
function formatCardNumber(input) {
    let value = input.value.replace(/\D/g, '');
    value = value.replace(/(\d{4})/g, '$1 ').trim();
    input.value = value;
}

// Sepet miktarı güncelleme
function updateCartQuantity(sepetId) {
    const quantity = document.getElementById(`quantity-${sepetId}`).value;
    document.getElementById(`update-form-${sepetId}`).submit();
}

// Sepetten ürün kaldırma onayı
function confirmRemove(sepetId) {
    if (confirm('Bu ürünü sepetten kaldırmak istediğinize emin misiniz?')) {
        document.getElementById(`remove-form-${sepetId}`).submit();
    }
}

// Ödeme formunu gönderme
function submitPayment() {
    document.getElementById('payment-form').submit();
    document.getElementById('payment-button').disabled = true;
    document.getElementById('payment-spinner').style.display = 'inline-block';
}

// Sayfa yüklendiğinde çalıştırılacak fonksiyonlar
document.addEventListener('DOMContentLoaded', function() {
    console.log("site.js yüklendi!");
    
    // Preloader'ı gizle
    setTimeout(function() {
        const preloader = document.querySelector('.preloader');
        if (preloader) {
            preloader.style.opacity = '0';
            setTimeout(function() {
                preloader.style.display = 'none';
            }, 500);
        }
    }, 500);
    
    // Sepet sayısını güncelle
    updateCartCount();
    
    // Scrollda animasyonları aktifleştir
    animateOnScroll();
    
    // Tooltipleri aktifleştir
    setupTooltips();
    
    // Resimleri kontrol et ve hata durumunda fallback göster
    fixBrokenImages();
    
    // Form ve buton işlemlerini düzenle
    setupFormHandlers();
});

// AJAX ile sepet sayısını güncelle
function updateCartCount() {
    // Kullanıcı giriş yapmış olabilir veya olmayabilir - sepet-count elementi varsa çalışır
    const sepetCountElement = document.getElementById('sepet-count');
    if (sepetCountElement) {
        fetch('/Sepet/GetSepetCount')
            .then(response => response.json())
            .then(data => {
                sepetCountElement.textContent = data.toString();
            })
            .catch(error => console.error('Sepet sayısı güncellenirken hata oluştu:', error));
    }
}

// Sayfa içi animasyonlar için
function animateOnScroll() {
    const animElements = document.querySelectorAll('.animate-on-scroll');
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animated');
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1
    });
    
    animElements.forEach(element => {
        observer.observe(element);
    });
}

// Tooltipleri aktifleştir
function setupTooltips() {
    const tooltips = document.querySelectorAll('.custom-tooltip');
    if (tooltips.length > 0) {
        tooltips.forEach(tooltip => {
            tooltip.addEventListener('mouseenter', function() {
                const tooltipText = this.querySelector('.tooltip-text');
                if (tooltipText) {
                    tooltipText.style.visibility = 'visible';
                    tooltipText.style.opacity = '1';
                }
            });
            
            tooltip.addEventListener('mouseleave', function() {
                const tooltipText = this.querySelector('.tooltip-text');
                if (tooltipText) {
                    tooltipText.style.visibility = 'hidden';
                    tooltipText.style.opacity = '0';
                }
            });
        });
    }
}

// Bozuk resimleri düzelt
function fixBrokenImages() {
    console.log("Resim kontrolleri başlıyor...");
    // Tüm resim elementlerini seç
    const images = document.querySelectorAll('img');
    
    if (!images || images.length === 0) {
        console.warn("Resim elementi bulunamadı");
        return;
    }
    
    // Her resim için hata kontrolü yap
    images.forEach(img => {
        // Orijinal src'yi sakla
        const originalSrc = img.getAttribute('src') || '';
        
        if (!originalSrc) {
            console.warn("Boş src attribute'u olan resim bulundu");
            return;
        }
        
        // Resim yüklenme hatası olduğunda yedek resim göster
        img.onerror = function() {
            console.warn(`Resim yüklenemedi: ${originalSrc}`);
            
            try {
                // Resim türüne göre uygun placeholder belirle
                if (originalSrc.includes('etkinlikler')) {
                    this.src = '/img/defaults/default-event.svg';
                } else if (originalSrc.includes('kullanicilar') || originalSrc.includes('users')) {
                    this.src = '/img/defaults/default-user.svg';
                } else if (originalSrc.includes('logo')) {
                    this.src = '/img/defaults/logo.svg';
                } else if (originalSrc.includes('slider')) {
                    this.src = '/img/defaults/default-slider-concert.svg';
                } else if (originalSrc.includes('category')) {
                    this.src = '/img/defaults/default-category-concert.svg';
                } else {
                    this.src = '/img/defaults/default-event.svg';
                }
            } catch (err) {
                console.error("Resim yedekleme hatası:", err);
                this.src = '/img/defaults/default-event.svg';
            }
            
            // Hata sonrası tekrar hata oluşmaması için
            this.onerror = null;
        };
        
        // Mevcut src'yi kontrol et (halihazırda yüklenmiş olabilir)
        if (img.complete && img.naturalHeight === 0) {
            img.onerror();
        }
    });
}

// Form ve buton işlemlerini düzenle
function setupFormHandlers() {
    console.log("Form ve buton işlemleri ayarlanıyor...");
    
    // Submit butonları için özel işleyici
    document.querySelectorAll('button[type="submit"]').forEach(function(button) {
        button.addEventListener('click', function(e) {
            // Önce olay yayılımını engelle
            e.stopPropagation();
            
            console.log("Submit butonu tıklandı:", button.textContent.trim());
            
            // Butonun formunu bul
            const form = this.closest('form');
            if (form) {
                console.log("Form bulundu:", form.action || form.id || "Adsız form");
                // Normal submit davranışı zaten çalışacak, ek bir şey yapmaya gerek yok
            }
        });
    });
    
    // Organizator alanındaki silme butonları için özel işleyici
    document.querySelectorAll('.silme-onayi-btn').forEach(function(button) {
        button.addEventListener('click', function(e) {
            // Önce olay yayılımını engelle
            e.preventDefault();
            e.stopPropagation();
            
            console.log("site.js: Silme butonu tıklandı");
            const etkinlikId = this.getAttribute('data-etkinlik-id');
            if (!etkinlikId) {
                console.error("Etkinlik ID bulunamadı");
                alert("Etkinlik ID bilgisi eksik!");
                return;
            }
            
            console.log("site.js: Etkinlik ID:", etkinlikId);
            
            // Silme formunu bul
            const silForm = document.getElementById('silForm');
            if (!silForm) {
                console.error("Silme formu bulunamadı");
                alert("Silme formu bulunamadı!");
                return;
            }
            
            // Form action'ını ayarla
            silForm.action = '/Organizator/Etkinlik/Sil/' + etkinlikId;
            console.log("Form action:", silForm.action);
            
            // Modalı göster
            const silmeModal = document.getElementById('silmeOnayi');
            if (silmeModal) {
                try {
                    if (typeof bootstrap !== 'undefined') {
                        const modal = new bootstrap.Modal(silmeModal);
                        modal.show();
                    } else if (typeof $ !== 'undefined' && typeof $.fn.modal === 'function') {
                        $(silmeModal).modal('show');
                    } else {
                        // Fallback - manuel modal göster
                        silmeModal.style.display = 'block';
                        silmeModal.classList.add('show');
                        document.body.classList.add('modal-open');
                        
                        // Manual modal kapatma
                        silmeModal.querySelectorAll('.btn-close, .btn-secondary, [data-bs-dismiss="modal"]').forEach(function(closeBtn) {
                            closeBtn.addEventListener('click', function() {
                                silmeModal.style.display = 'none';
                                silmeModal.classList.remove('show');
                                document.body.classList.remove('modal-open');
                            });
                        });
                    }
                } catch (err) {
                    console.error("Modal gösterme hatası:", err);
                    if (confirm("Bu etkinliği silmek istediğinize emin misiniz?")) {
                        silForm.submit();
                    }
                }
            } else {
                console.error("Modal bulunamadı");
                if (confirm("Bu etkinliği silmek istediğinize emin misiniz?")) {
                    silForm.submit();
                }
            }
        });
    });
}