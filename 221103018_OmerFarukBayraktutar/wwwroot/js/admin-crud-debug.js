// Admin CRUD Debug Helper
document.addEventListener('DOMContentLoaded', function() {
    console.log("=== ADMIN CRUD DEBUG BAŞLADI ===");
    
    // 1. Sayfa URL'ini kontrol et
    console.log("Şu anki sayfa:", window.location.pathname);
    
    // 2. Tüm formları listele
    const forms = document.querySelectorAll('form');
    console.log(`Toplam ${forms.length} form bulundu:`);
    forms.forEach((form, index) => {
        console.log(`Form ${index + 1}:`, {
            action: form.action,
            method: form.method,
            id: form.id,
            hasAntiForgeryToken: form.querySelector('input[name="__RequestVerificationToken"]') !== null
        });
    });
    
    // 3. CRUD butonlarını kontrol et
    const crudButtons = {
        ekle: document.querySelectorAll('[href*="/Ekle"], [href*="/Create"], .btn-add, .btn-create'),
        duzenle: document.querySelectorAll('[href*="/Duzenle"], [href*="/Edit"], .btn-edit, .btn-update'),
        sil: document.querySelectorAll('[href*="/Sil"], [href*="/Delete"], .btn-delete, .btn-remove'),
        kaydet: document.querySelectorAll('button[type="submit"], input[type="submit"], .btn-save')
    };
    
    Object.entries(crudButtons).forEach(([action, buttons]) => {
        console.log(`${action.toUpperCase()} butonları: ${buttons.length} adet`);
        buttons.forEach((btn, i) => {
            console.log(`  ${action} ${i + 1}:`, {
                tag: btn.tagName,
                text: btn.textContent?.trim(),
                href: btn.href,
                type: btn.type,
                form: btn.form?.id || 'Form yok'
            });
        });
    });
    
    // 4. Event listener'ları test et
    console.log("\n=== EVENT LISTENER TESTİ ===");
    
    // Test için tüm submit butonlarına listener ekle
    document.querySelectorAll('button[type="submit"], input[type="submit"]').forEach((btn, index) => {
        btn.addEventListener('click', function(e) {
            console.log(`Submit butonu ${index + 1} tıklandı!`, {
                buton: this.textContent?.trim(),
                form: this.form?.action,
                defaultPrevented: e.defaultPrevented
            });
            
            // Form validasyonu kontrol et
            if (this.form && this.form.checkValidity) {
                const isValid = this.form.checkValidity();
                console.log("Form geçerli mi?", isValid);
                if (!isValid) {
                    this.form.reportValidity();
                }
            }
        }, true); // Capture phase'de dinle
    });
    
    // 5. jQuery ve Validation kontrolleri
    console.log("\n=== KÜTÜPHANE KONTROL ===");
    console.log("jQuery yüklü mü?", typeof jQuery !== 'undefined');
    console.log("jQuery Validation yüklü mü?", typeof jQuery !== 'undefined' && typeof jQuery.validator !== 'undefined');
    console.log("Bootstrap yüklü mü?", typeof bootstrap !== 'undefined');
    
    // 6. Network isteklerini izle
    if (window.fetch) {
        const originalFetch = window.fetch;
        window.fetch = function(...args) {
            console.log('Fetch isteği:', args[0]);
            return originalFetch.apply(this, args);
        };
    }
    
    // 7. Form submit olaylarını izle
    document.addEventListener('submit', function(e) {
        console.log('FORM SUBMIT EDİLİYOR!', {
            form: e.target,
            action: e.target.action,
            method: e.target.method,
            defaultPrevented: e.defaultPrevented
        });
    }, true); // Capture phase
    
    console.log("=== DEBUG BİTTİ ===");
    console.log("Butona tıklayın ve console'u kontrol edin!");
});