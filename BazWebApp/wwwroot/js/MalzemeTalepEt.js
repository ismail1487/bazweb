/**
 * Malzeme Talep Et - JavaScript Dosyası
 * İki ana tab: Malzeme Talep Et ve Depo Hazırlama
 */

$(document).ready(function () {
    // DataTables başlatma
    initializeDataTables();

    // Event handler'ları başlat
    initializeEventHandlers();

    // Select2 başlatma
    initializeSelect2();

    // Proje listesini yükle
    loadProjeList();
    
    // Statü listesini yükle
    loadStatuList();

    // İlk verileri yükle
    loadInitialData();
});

/**
 * DataTables başlatma
 */
function initializeDataTables() {
    // Malzeme Talep Tablosu
    if ($.fn.DataTable.isDataTable('#malzemeTalepTable')) {
        $('#malzemeTalepTable').DataTable().destroy();
    }

    window.malzemeTalepTable = $('#malzemeTalepTable').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.24/i18n/Turkish.json"
        },
        "responsive": true,
        "lengthChange": true,
        "autoWidth": false,
        "pageLength": 10,
        "order": [[8, "desc"]], // Oluşturma tarihine göre sırala
        "data": [], // Başlangıçta boş veri
        "columns": [
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `<input type="checkbox" class="row-checkbox" data-id="${row.malzemeTalep.malzemeTalebiEssizID}" />`;
                }
            },
            {
                "data": null,
                "render": function (data, type, row) {
                    return `${row.malzemeTalep.satSeriNo}/${row.malzemeTalep.satSiraNo}`;
                }
            },
            {
                "data": "malzemeTalep.projeKodu"
            },
            {
                "data": "malzemeTalep.malzemeKodu"
            },
            {
                "data": "malzemeTalep.malzemeIsmi"
            },
            {
                "data": "malzemeTalep.malzemeOrijinalTalepEdilenMiktar",
                "render": function (data, type, row) {
                    return data.toLocaleString('tr-TR');
                }
            },
            {
                "data": "toplamSevkEdilenMiktar",
                "render": function (data, type, row) {
                    return data.toLocaleString('tr-TR');
                }
            },
            {
                "data": "kalanMiktar",
                "render": function (data, type, row) {
                    return data.toLocaleString('tr-TR');
                }
            },
            {
                "data": "malzemeTalep.satOlusturmaTarihi",
                "render": function (data, type, row) {
                    if (!data) return '-';
                    const date = new Date(data);
                    return date.toLocaleDateString('tr-TR') + ' ' + date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                        <button class="btn btn-sm btn-info btn-detay" data-id="${row.malzemeTalep.malzemeTalebiEssizID}" data-type="malzeme" title="Detay">
                            <i class="fas fa-eye"></i>
                        </button>
                    `;
                }
            }
        ]
    });
    
    // Depo Hazırlama Tablosu
    if ($.fn.DataTable.isDataTable('#depoHazirlamaTable')) {
        $('#depoHazirlamaTable').DataTable().destroy();
    }

    window.depoHazirlamaTable = $('#depoHazirlamaTable').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.24/i18n/Turkish.json"
        },
        "responsive": true,
        "lengthChange": true,
        "autoWidth": false,
        "pageLength": 10,
        "order": [[7, "desc"]], // Oluşturma tarihine göre sırala
        "data": [], // Başlangıçta boş veri
        "columns": [
            {
                "data": null,
                "render": function (data, type, row) {
                    return `${row.malzemeTalep.satSeriNo}/${row.malzemeTalep.satSiraNo}`;
                }
            },
            {
                "data": "malzemeTalep.projeKodu"
            },
            {
                "data": "malzemeTalep.malzemeKodu"
            },
            {
                "data": "malzemeTalep.malzemeIsmi"
            },
            {
                "data": "malzemeTalep.malzemeOrijinalTalepEdilenMiktar",
                "render": function (data, type, row) {
                    return data.toLocaleString('tr-TR');
                }
            },
            {
                "data": "toplamSevkEdilenMiktar",
                "render": function (data, type, row) {
                    return data.toLocaleString('tr-TR');
                }
            },
            {
                "data": "kalanMiktar",
                "render": function (data, type, row) {
                    return data.toLocaleString('tr-TR');
                }
            },
            {
                "data": "malzemeTalep.satOlusturmaTarihi",
                "render": function (data, type, row) {
                    if (!data) return '-';
                    const date = new Date(data);
                    return date.toLocaleDateString('tr-TR') + ' ' + date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                        <button class="btn btn-sm btn-info btn-detay" data-id="${row.malzemeTalep.malzemeTalebiEssizID}" data-type="depo" title="Detay">
                            <i class="fas fa-eye"></i>
                        </button>
                    `;
                }
            }
        ]
    });
    
    // Aktif tab'ın tablosunu düzelt
    setTimeout(function() {
        if (window.malzemeTalepTable) {
            window.malzemeTalepTable.columns.adjust();
        }
    }, 100);
}

/**
 * Event handler'ları başlatma
 */
function initializeEventHandlers() {
    // Proje filtresi değişikliği
    $('#projeFilter').on('change', function () {
        refreshMalzemeTalepTable();
    });
    
    // Statü filtresi değişikliği
    $('#statuFilter').on('change', function () {
        refreshMalzemeTalepTable();
    });
    
    // Depo Proje filtresi değişikliği
    $('#depoProjeFilter').on('change', function () {
        refreshDepoHazirlamaTable();
    });

    // Yeni Talep butonu
    $('#btnYeniTalep').on('click', function () {
        yeniTalepEkle();
    });

    // Yeni Depo Kaydı butonu
    $('#btnYeniDepoKayit').on('click', function () {
        yeniDepoKayitEkle();
    });

    // Filtrele butonu - Yorum satırına alındı
    // $('#btnFiltrele').on('click', function () {
    //     refreshMalzemeTalepTable();
    // });

    // Excel'e Aktar butonları
    $('#btnExcelAktarMalzeme').on('click', function () {
        exportToExcel('malzeme');
    });

    $('#btnExcelAktarDepo').on('click', function () {
        exportToExcel('depo');
    });

    // Tab değişikliği olayları
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        const targetTab = $(e.target).attr("href");
        handleTabChange(targetTab);
    });

    // Tümünü seç checkbox - devre dışı
    $('#selectAll').on('change', function () {
        // Tümünü seçme özelliği kapalı
        $(this).prop('checked', false);
        Swal.fire({
            icon: 'info',
            title: 'Bilgi',
            text: 'Sadece tek bir kayıt seçebilirsiniz!'
        });
    });

    // Tekil checkbox'lar değiştiğinde - sadece 1 tane seçilebilir
    $(document).on('change', '.row-checkbox', function () {
        if ($(this).is(':checked')) {
            // Diğer tüm checkbox'ları kaldır
            $('.row-checkbox').not(this).prop('checked', false);
        }
        // SelectAll checkbox'ını her zaman kapalı tut
        $('#selectAll').prop('checked', false);
    });

    // Detay butonu
    $(document).on('click', '.btn-detay', function () {
        const id = $(this).data('id');
        const type = $(this).data('type');
        showDetay(id, type);
    });

    // Düzenle butonu
    $(document).on('click', '.btn-duzenle', function () {
        const id = $(this).data('id');
        const type = $(this).data('type');
        editKayit(id, type);
    });

    // Sil butonu
    $(document).on('click', '.btn-sil', function () {
        const id = $(this).data('id');
        const type = $(this).data('type');
        deleteKayit(id, type);
    });
}

/**
 * Select2 başlatma
 */
function initializeSelect2() {
    $('#projeFilter').select2({
        theme: 'bootstrap4',
        placeholder: 'Tümü',
        allowClear: false
    });
    
    $('#statuFilter').select2({
        theme: 'bootstrap4',
        placeholder: 'Tüm Statüler',
        allowClear: false
    });
    
    $('#depoProjeFilter').select2({
        theme: 'bootstrap4',
        placeholder: 'Tümü',
        allowClear: false
    });
}

/**
 * Proje listesini API'den yükle
 */
function loadProjeList() {
    $.ajax({
        url: '/panel/ProjeGenelBilgilerList',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.isSuccess && response.value) {
                const select = $('#projeFilter');
                const depoSelect = $('#depoProjeFilter');
                
                // API'den gelen tüm verileri ekle (Tümü dahil)
                response.value.forEach(function (proje) {
                    const optionText = proje.tabloID === 0 
                        ? proje.projeAdi 
                        : proje.projeAdi + ' (' + proje.projeKodu + ')';
                    
                    // Malzeme Talep Et için
                    const option = new Option(
                        optionText,
                        proje.projeKodu, // Value olarak projeKodu kullan
                        proje.tabloID === 0, // Tümü seçili olsun
                        proje.tabloID === 0
                    );
                    select.append(option);
                    
                    // Depo Hazırlama için
                    const depoOption = new Option(
                        optionText,
                        proje.projeKodu, // Value olarak projeKodu kullan
                        proje.tabloID === 0, // Tümü seçili olsun
                        proje.tabloID === 0
                    );
                    depoSelect.append(depoOption);
                });
                
                // Select2'yi güncelle ama change event'ini tetikleme
                select.trigger('change.select2');
                depoSelect.trigger('change.select2');
            }
        },
        error: function (error) {
            console.error('Proje listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Proje listesi yüklenemedi!'
            });
        }
    });
}

/**
 * Statü listesini API'den yükle
 */
function loadStatuList() {
    $.ajax({
        url: '/panel/TalepSurecStatuleriList',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.isSuccess && response.value) {
                const select = $('#statuFilter');
                
                // API'den gelen tüm verileri ekle (Tümü dahil)
                response.value.forEach(function (statu) {
                    const option = new Option(
                        statu.statu,
                        statu.tabloID,
                        statu.tabloID === 0, // Tümü seçili olsun
                        statu.tabloID === 0
                    );
                    select.append(option);
                });
                
                select.trigger('change');
            }
        },
        error: function (error) {
            console.error('Statü listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Statü listesi yüklenemedi!'
            });
        }
    });
}

/**
 * İlk verileri yükleme
 */
function loadInitialData() {
    // İlk tab yüklendiğinde verileri getir
    refreshMalzemeTalepTable();
}

/**
 * Tab değişikliği kontrolü
 */
function handleTabChange(tabId) {
    switch (tabId) {
        case '#malzemeTalepTabContent':
            if (window.malzemeTalepTable) {
                window.malzemeTalepTable.columns.adjust().draw();
            }
            refreshMalzemeTalepTable();
            break;
        case '#depoHazirlamaTabContent':
            if (window.depoHazirlamaTable) {
                window.depoHazirlamaTable.columns.adjust().draw();
            }
            refreshDepoHazirlamaTable();
            break;
        default:
    }
}

/**
 * Yeni Talep Ekleme
 */
function yeniTalepEkle() {
    // Seçili satırları kontrol et
    const checkedBoxes = $('.row-checkbox:checked');
    
    if (checkedBoxes.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen bir kayıt seçiniz!'
        });
        return;
    }
    
    if (checkedBoxes.length > 1) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen sadece bir kayıt seçiniz!'
        });
        return;
    }
    
    // Seçili kaydın ID'sini al
    const malzemeTalebiEssizID = checkedBoxes.first().data('id');
    $('#malzemeTalebiEssizID').val(malzemeTalebiEssizID);
    
    // Bildirim tiplerini yükle
    loadBildirimTipleri();
    
    // Formu temizle
    $('#sevkEdilenMiktar').val('');
    $('#surecStatuGirilenNot').val('');
    $('#surecStatuBildirimTipi').val('');
    
    // Modalı aç
    $('#talepEtModal').modal('show');
}

/**
 * Bildirim tiplerini yükle
 */
function loadBildirimTipleri() {
    // Önce selectbox'ı temizle
    const select = $('#surecStatuBildirimTipi');
    select.empty();
    select.append(new Option('Seçiniz', '', false, false));
    
    // TabloID 1 ve 2 için bildirim tiplerini yükle
    Promise.all([
        $.ajax({
            url: '/panel/SurecStatuleriBildirimTipleriList/1',
            type: 'GET',
            dataType: 'json'
        }),
        $.ajax({
            url: '/panel/SurecStatuleriBildirimTipleriList/2',
            type: 'GET',
            dataType: 'json'
        })
    ]).then(function(responses) {
        // Her iki response'u birleştir
        const allTipler = [];
        
        if (responses[0] && responses[0].isSuccess && responses[0].value) {
            allTipler.push(...responses[0].value);
        }
        
        if (responses[1] && responses[1].isSuccess && responses[1].value) {
            allTipler.push(...responses[1].value);
        }
        
        // Selectbox'a ekle
        allTipler.forEach(function(tip) {
            select.append(new Option(tip.bildirimTipiTanimlama, tip.tabloID));
        });
        
    }).catch(function(error) {
        console.error('Bildirim tipleri yüklenirken hata:', error);
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Bildirim tipleri yüklenemedi!'
        });
    });
}

/**
 * Talep kaydet butonu
 */
$(document).on('click', '#btnTalepKaydet', function() {
    // Form validasyonu
    const sevkEdilenMiktar = parseInt($('#sevkEdilenMiktar').val());
    const surecStatuBildirimTipiID = parseInt($('#surecStatuBildirimTipi').val());
    const surecStatuGirilenNot = $('#surecStatuGirilenNot').val() || '';
    const malzemeTalebiEssizID = parseInt($('#malzemeTalebiEssizID').val());
    
    if (!sevkEdilenMiktar || sevkEdilenMiktar <= 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen geçerli bir miktar giriniz!'
        });
        return;
    }
    
    if (!surecStatuBildirimTipiID) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen bildirim tipi seçiniz!'
        });
        return;
    }
    
    // Request data hazırla
    const requestData = {
        malzemeTalebiEssizID: malzemeTalebiEssizID,
        sevkEdilenMiktar: sevkEdilenMiktar,
        sevkTalepEdenKisiID: 1,
        malzemeSevkTalebiYapanDepartmanID: 1,
        malzemeSevkTalebiYapanKisiID: 1,
        surecStatuGirilenNot: surecStatuGirilenNot,
        surecStatuBildirimTipiID: surecStatuBildirimTipiID
    };
    
    $.ajax({
        url: '/panel/MalzemeTalepEt',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function(response) {
            $('#talepEtModal').modal('hide');
            
            if (response.isSuccess) {
                Swal.fire({
                    icon: 'success',
                    title: 'Başarılı',
                    text: 'Malzeme talebi başarıyla kaydedildi!',
                    confirmButtonColor: '#ff5f00'
                }).then(() => {
                    // Checkbox'ları temizle
                    $('.row-checkbox').prop('checked', false);
                    $('#selectAll').prop('checked', false);
                    
                    // Tabloyu mevcut filtreler ile yenile
                    refreshMalzemeTalepTable();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Hata',
                    text: response.errors && response.errors.length > 0 
                        ? response.errors.join(', ') 
                        : 'Talep kaydedilemedi!',
                    confirmButtonColor: '#dc3545'
                });
            }
        },
        error: function(error) {
            console.error('Talep Et - Error Response:', error);
            
            $('#talepEtModal').modal('hide');
            
            let errorMessage = 'Talep kaydedilemedi!';
            if (error.responseJSON && error.responseJSON.errors && error.responseJSON.errors.length > 0) {
                errorMessage = error.responseJSON.errors.join(', ');
            } else if (error.responseText) {
                errorMessage = error.responseText;
            }
            
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: errorMessage,
                confirmButtonColor: '#dc3545'
            });
        }
    });
});

/**
 * Yeni Depo Kaydı Ekleme
 */
function yeniDepoKayitEkle() {
    Swal.fire({
        title: 'Yeni Depo Kaydı',
        html: `
            <div class="form-group text-left">
                <label>Malzeme Adı:</label>
                <input type="text" id="swal-malzemeAdi" class="form-control" placeholder="Malzeme adı">
            </div>
            <div class="form-group text-left">
                <label>Kategori:</label>
                <select id="swal-kategori" class="form-control">
                    <option value="">Seçiniz</option>
                    <option value="Kırtasiye">Kırtasiye</option>
                    <option value="Elektronik">Elektronik</option>
                    <option value="Mobilya">Mobilya</option>
                    <option value="Diğer">Diğer</option>
                </select>
            </div>
            <div class="form-group text-left">
                <label>Depo Yeri:</label>
                <input type="text" id="swal-depoYeri" class="form-control" placeholder="Depo yeri">
            </div>
            <div class="form-group text-left">
                <label>Stok Miktarı:</label>
                <input type="number" id="swal-stok" class="form-control" min="0" placeholder="Stok miktarı">
            </div>
            <div class="form-group text-left">
                <label>Birim:</label>
                <select id="swal-birim" class="form-control">
                    <option value="">Seçiniz</option>
                    <option value="Adet">Adet</option>
                    <option value="Kg">Kg</option>
                    <option value="Lt">Lt</option>
                    <option value="Paket">Paket</option>
                </select>
            </div>
        `,
        showCancelButton: true,
        confirmButtonColor: '#ff5f00',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Kaydet',
        cancelButtonText: 'İptal',
        preConfirm: () => {
            const malzemeAdi = document.getElementById('swal-malzemeAdi').value;
            const kategori = document.getElementById('swal-kategori').value;
            const depoYeri = document.getElementById('swal-depoYeri').value;
            const stok = document.getElementById('swal-stok').value;
            const birim = document.getElementById('swal-birim').value;

            if (!malzemeAdi || !kategori || !depoYeri || !stok || !birim) {
                Swal.showValidationMessage('Tüm alanları doldurunuz!');
                return false;
            }

            return { malzemeAdi, kategori, depoYeri, stok, birim };
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yapılacak
            Swal.fire('Başarılı!', 'Depo kaydı eklendi.', 'success');
            refreshDepoHazirlamaTable();
        }
    });
}

/**
 * Malzeme Talep tablosunu yenileme
 */
function refreshMalzemeTalepTable() {
    // Filtre değerlerini al
    const projeKoduValue = parseInt($('#projeFilter').val()) || 0;
    const statuIDValue = parseInt($('#statuFilter').val()) || 0;
    
    // Statü ID'lerini belirle
    // TabloID 0: Tümü → boş array, malzemeTalepEtGetir: true
    // TabloID 1: Gelmedi → [1], malzemeTalepEtGetir: false
    // TabloID 2: Geldi → [2], malzemeTalepEtGetir: false
    let statuIDs = [];
    let malzemeTalepEtGetir = false;
    
    if (statuIDValue === 0) {
        statuIDs = []; // Tümü - boş array
        malzemeTalepEtGetir = true; // Tümü seçiliyse true
    } else {
        statuIDs = [statuIDValue]; // Seçilen statü
        malzemeTalepEtGetir = false; // Tekil seçiliyse false
    }
    
    // Request parametrelerini hazırla
    const requestData = {
        projeKodu: projeKoduValue === 0 ? 0 : projeKoduValue, // Tümü için 0, seçiliyse projeKodu gönder
        talepSurecStatuIDs: statuIDs,
        searchText: "",
        malzemeTalepEtGetir: malzemeTalepEtGetir
    };
    
    // API'ye POST isteği gönder
    $.ajax({
        url: '/panel/MalzemeTalepleriniGetir',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function (response) {
            if (response.isSuccess && response.value) {
                // DataTable'ı güncelle
                if (window.malzemeTalepTable) {
                    window.malzemeTalepTable.clear();
                    window.malzemeTalepTable.rows.add(response.value);
                    window.malzemeTalepTable.draw();
                } else {
                    console.error('window.malzemeTalepTable bulunamadı!');
                }
                
                // Checkbox'ları sıfırla
                $('#selectAll').prop('checked', false);
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Uyarı',
                    text: 'Veri bulunamadı!'
                });
            }
        },
        error: function (error) {
            console.error('Malzeme Talep listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Malzeme Talep listesi yüklenemedi!'
            });
        }
    });
}

/**
 * Depo Hazırlama tablosunu yenileme
 */
function refreshDepoHazirlamaTable() {
    // Filtre değerlerini al
    const projeKoduValue = parseInt($('#depoProjeFilter').val()) || 0;
    
    // Depo için sabit parametreler
    // talepSurecStatuIDs: [3] (Depo Hazırlama statüsü)
    // malzemeTalepEtGetir: false
    const requestData = {
        projeKodu: projeKoduValue === 0 ? 0 : projeKoduValue, // Tümü için 0, seçiliyse projeKodu gönder
        talepSurecStatuIDs: [3], // Depo Hazırlama için her zaman 3
        searchText: "",
        malzemeTalepEtGetir: false // Depo için her zaman false
    };
    
    // API'ye POST isteği gönder
    $.ajax({
        url: '/panel/MalzemeTalepleriniGetir',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function (response) {
            if (response.isSuccess && response.value) {
                // DataTable'ı güncelle
                if (window.depoHazirlamaTable) {
                    window.depoHazirlamaTable.clear();
                    window.depoHazirlamaTable.rows.add(response.value);
                    window.depoHazirlamaTable.draw();
                } else {
                    console.error('window.depoHazirlamaTable bulunamadı!');
                }
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Uyarı',
                    text: 'Veri bulunamadı!'
                });
            }
        },
        error: function (error) {
            console.error('Depo Hazırlama listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Depo Hazırlama listesi yüklenemedi!'
            });
        }
    });
}

/**
 * Excel'e aktarma
 */
function exportToExcel(type) {
    Swal.fire({
        icon: 'info',
        title: 'Bilgi',
        text: `${type === 'malzeme' ? 'Malzeme Talep' : 'Depo Hazırlama'} verileri Excel'e aktarılıyor...`
    });
    // Excel export işlemi yapılacak
}

/**
 * Detay gösterme
 */
function showDetay(id, type) {
    if (type === 'malzeme') {
        // DataTable'dan ilgili satırı bul
        const table = window.malzemeTalepTable;
        const rowData = table.rows().data().toArray().find(row => row.malzemeTalep.malzemeTalebiEssizID === id);
        
        if (rowData) {
            const malzeme = rowData.malzemeTalep;
            const tarih = malzeme.satOlusturmaTarihi ? new Date(malzeme.satOlusturmaTarihi).toLocaleString('tr-TR') : '-';
            
            $('#detayModalLabel').html(`<i class="fas fa-info-circle mr-2"></i>Malzeme Talep Detayı`);
            $('#detayModalIcerik').html(`
                <div class="row">
                    <div class="col-md-6">
                        <p><strong>SAT Seri No:</strong> ${malzeme.satSeriNo || '-'}</p>
                        <p><strong>SAT Sıra No:</strong> ${malzeme.satSiraNo || '-'}</p>
                        <p><strong>Oluşturma Tarihi:</strong> ${tarih}</p>
                    </div>
                    <div class="col-md-6">
                        <p><strong>Proje Kodu:</strong> ${malzeme.projeKodu || '-'}</p>
                        <p><strong>Malzeme Kodu:</strong> ${malzeme.malzemeKodu || '-'}</p>
                        <p><strong>Malzeme Adı:</strong> ${malzeme.malzemeIsmi || '-'}</p>
                    </div>
                    <div class="col-12 mt-3">
                        <p><strong>Talep Edilen Miktar:</strong> ${malzeme.malzemeOrijinalTalepEdilenMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Sevk Edilen Miktar:</strong> ${rowData.toplamSevkEdilenMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Kalan Miktar:</strong> ${rowData.kalanMiktar?.toLocaleString('tr-TR') || '-'}</p>
                    </div>
                </div>
            `);
            $('#detayModal').modal('show');
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Kayıt bulunamadı!'
            });
        }
    } else {
        $('#detayModalLabel').html(`<i class="fas fa-info-circle mr-2"></i>Depo Kayıt Detayı`);
        $('#detayModalIcerik').html(`
            <div class="row">
                <div class="col-12">
                    <p><strong>ID:</strong> ${id}</p>
                    <p class="text-muted">Detay bilgileri API'den yüklenecek...</p>
                </div>
            </div>
        `);
        $('#detayModal').modal('show');
    }
}

/**
 * Kayıt düzenleme
 */
function editKayit(id, type) {
    Swal.fire({
        icon: 'info',
        title: 'Bilgi',
        text: `${type === 'malzeme' ? 'Talep' : 'Depo kaydı'} düzenleme sayfasına yönlendirileceksiniz...`
    });
    // Düzenleme işlemi yapılacak
}

/**
 * Kayıt silme
 */
function deleteKayit(id, type) {
    const text = type === 'malzeme' ? 'Bu talebi' : 'Bu depo kaydını';
    
    Swal.fire({
        title: 'Emin misiniz?',
        text: `${text} silmek istediğinizden emin misiniz?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ff5f00',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Sil!',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API'ye silme isteği gönderilecek
            Swal.fire('Silindi!', 'Kayıt başarıyla silindi.', 'success');
            
            if (type === 'malzeme') {
                refreshMalzemeTalepTable();
            } else {
                refreshDepoHazirlamaTable();
            }
        }
    });
}
