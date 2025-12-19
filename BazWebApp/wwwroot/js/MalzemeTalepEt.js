/**
 * Malzeme Talep Et - JavaScript Dosyası
 * İki ana tab: Malzeme Talep Et ve Depo Hazırlama
 */

// Çift istek önleme değişkenleri
let isLoadingMalzemeTalep = false;
let isLoadingDepoHazirlama = false;
let isLoadingUretimMalKabul = false;
let isLoadingKaliteKontrol = false;
let isLoadingDepoKabul = false;
let isLoadingImalatEk = false;

// İlk yükleme kontrolü
window.isInitialLoad = false;

// Departman listesi cache
let departmanListesi = [];

$(document).ready(function () {
    // Departman listesini yükle
    loadDepartmanList();
    
    // Sayfa yüklendiğinde hemen spinner'ı göster
    $('#malzemeTalepSpinner').show();
    $('#malzemeTalepTable').hide();
    
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
    
    // URL'den aktif tab'ı yükle
    loadActiveTabFromUrl();
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
            "lengthMenu": "Sayfada _MENU_ kayıt göster",
            "zeroRecords": "Eşleşen kayıt bulunamadı",
            "info": "Toplam _TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
            "infoEmpty": "Gösterilecek kayıt yok",
            "infoFiltered": "(_MAX_ kayıt içerisinden filtrelendi)",
            "search": "Ara:",
            "paginate": {
                "first": "İlk",
                "last": "Son",
                "next": "Sonraki",
                "previous": "Önceki"
            }
        },
        "dom": "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-7'i><'col-sm-12 col-md-5 text-right'p>>",
        "responsive": true,
        "lengthChange": false,
        "autoWidth": false,
        "pageLength": 10,
        "order": [[7, "desc"]], // Kayıt tarihine göre sırala
        "data": [], // Başlangıçta boş veri
        "columns": [
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `<input type="checkbox" class="row-checkbox" data-id="${row.malzemeTalebiEssizID}" data-surecid="${row.malzemeTalepSurecTakipID}" />`;
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
                "data": "kalanMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '-';
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    const kalanMiktar = Math.floor(row.kalanMiktar || 0);
                    return `<input type="number" class="form-control form-control-sm talep-miktar-input" 
                                   data-surecid="${row.malzemeTalepSurecTakipID}" 
                                   data-max="${kalanMiktar}"
                                   value="${kalanMiktar}" 
                                   min="0" 
                                   step="1" 
                                   style="width: 100px; padding: 2px 5px; font-size: 13px;" />`;
                }
            },
            {
                "data": "malzemeTalep.aciklama",
                "render": function (data, type, row) {
                    return data || '-';
                }
            },
            {
                "data": "malzemeTalep.satCariHesap",
                "render": function (data, type, row) {
                    return data || '-';
                }
            },
            {
                "data": "malzemeTalep.kayitTarihi",
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
            "lengthMenu": "Sayfada _MENU_ kayıt göster",
            "zeroRecords": "Eşleşen kayıt bulunamadı",
            "info": "Toplam _TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
            "infoEmpty": "Gösterilecek kayıt yok",
            "infoFiltered": "(_MAX_ kayıt içerisinden filtrelendi)",
            "search": "Ara:",
            "paginate": {
                "first": "İlk",
                "last": "Son",
                "next": "Sonraki",
                "previous": "Önceki"
            }
        },
        "dom": "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-7'i><'col-sm-12 col-md-5 text-right'p>>",
        "responsive": true,
        "lengthChange": false,
        "autoWidth": false,
        "pageLength": 10,
        "order": [[11, "desc"]], // Talep tarihine göre sırala (kolon sayısı arttı)
        "data": [], // Başlangıçta boş veri
        "columns": [
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `<input type="checkbox" class="depo-row-checkbox" data-id="${row.malzemeTalebiEssizID}" data-surecid="${row.malzemeTalepSurecTakipID}" />`;
                }
            },
            { "data": "sevkID", "render": function (data) { return data || '-'; } },
            { "data": "malzemeTalep.projeKodu" },
            { "data": "malzemeTalep.malzemeKodu" },
            { "data": "malzemeTalep.malzemeIsmi" },
            { "data": "talepEdilenMiktar", "render": function (data) { return data ? data.toLocaleString('tr-TR') : '-'; } },
            { 
                "data": "hazirlanabilecekMiktar", 
                "render": function (data) { 
                    return data ? data.toLocaleString('tr-TR') : '0'; 
                } 
            },
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    const hazirlanabilecekMiktar = Math.floor(row.hazirlanabilecekMiktar || 0);
                    return `<input type="number" class="form-control form-control-sm hazirlanan-miktar-input"
                                   data-surecid="${row.malzemeTalepSurecTakipID}"
                                   data-max="${hazirlanabilecekMiktar}"
                                   value="${hazirlanabilecekMiktar}"
                                   min="0"
                                   step="1"
                                   style="width: 100px; padding: 2px 5px; font-size: 13px;" />`;
                }
            },
            { "data": "malzemeTalep.aciklama", "render": function (data) { return data || '-'; } },
            { "data": "malzemeTalep.satCariHesap", "render": function (data) { return data || '-'; } },
            { "data": "malzemeTalep.satOlusturmaTarihi", "render": function (data) { if (!data) return '-'; const date = new Date(data); return date.toLocaleDateString('tr-TR') + ' ' + date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' }); } },
            { "data": "surecOlusturmaTarihi", "render": function (data) { if (!data) return '-'; const date = new Date(data); return date.toLocaleDateString('tr-TR') + ' ' + date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' }); } },
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
    
    // Üretim Mal Kabul Tablosu
    if ($.fn.DataTable.isDataTable('#uretimMalKabulTable')) {
        $('#uretimMalKabulTable').DataTable().destroy();
    }

    window.uretimMalKabulTable = $('#uretimMalKabulTable').DataTable({
        "language": {
            "lengthMenu": "Sayfada _MENU_ kayıt göster",
            "zeroRecords": "Eşleşen kayıt bulunamadı",
            "info": "Toplam _TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
            "infoEmpty": "Gösterilecek kayıt yok",
            "infoFiltered": "(_MAX_ kayıt içerisinden filtrelendi)",
            "search": "Ara:",
            "paginate": {
                "first": "İlk",
                "last": "Son",
                "next": "Sonraki",
                "previous": "Önceki"
            }
        },
        "dom": "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-7'i><'col-sm-12 col-md-5 text-right'p>>",
        "responsive": true,
        "lengthChange": false,
        "autoWidth": false,
        "pageLength": 10,
        "order": [[8, "desc"]], // Teslim tarihine göre sırala (desc)
        "data": [], // Başlangıçta boş veri
        "rowCallback": function(row, data) {
            // paramTalepSurecStatuID === 7 ise kırmızı, onayId varsa yeşil yap
            if (data.paramTalepSurecStatuID === 7) {
                $(row).addClass('iade-edilmis-satir');
                $(row).removeClass('onaylandi-satir');
                $(row).attr('style', 'background-color: #f8d7da !important; color: #721c24 !important;');
                $(row).find('td').css({
                    'background-color': '#f8d7da',
                    'color': '#721c24'
                });
            } else if (data.onayId != null && data.onayId !== '') {
                $(row).addClass('onaylandi-satir');
                $(row).removeClass('iade-edilmis-satir');
                $(row).attr('style', 'background-color: #d4edda !important; color: #155724 !important;');
                $(row).find('td').css({
                    'background-color': '#d4edda',
                    'color': '#155724'
                });
            } else {
                $(row).removeClass('onaylandi-satir iade-edilmis-satir');
                $(row).removeAttr('style');
            }
        },
        "columns": [
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `<input type="checkbox" class="uretim-row-checkbox" data-id="${row.malzemeTalebiEssizID}" data-surecid="${row.malzemeTalepSurecTakipID}" />`;
                }
            },
            {
                "data": "sevkID",
                "render": function (data) {
                    return data || '-';
                }
            },
            {
                "data": "hazirId",
                "render": function (data) {
                    return data || '-';
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
                "data": "islenenMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '0';
                }
            },
            {
                "data": "surecOlusturmaTarihi",
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
                        <button class="btn btn-sm btn-info btn-detay" data-id="${row.malzemeTalep.malzemeTalebiEssizID}" data-type="uretim" title="Detay">
                            <i class="fas fa-eye"></i>
                        </button>
                    `;
                }
            }
        ]
    });
    
    // Kalite Kontrol Tablosu
    if ($.fn.DataTable.isDataTable('#kaliteKontrolTable')) {
        $('#kaliteKontrolTable').DataTable().destroy();
    }

    window.kaliteKontrolTable = $('#kaliteKontrolTable').DataTable({
        "language": {
            "lengthMenu": "Sayfada _MENU_ kayıt göster",
            "zeroRecords": "Eşleşen kayıt bulunamadı",
            "info": "Toplam _TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
            "infoEmpty": "Gösterilecek kayıt yok",
            "infoFiltered": "(_MAX_ kayıt içerisinden filtrelendi)",
            "search": "Ara:",
            "paginate": {
                "first": "İlk",
                "last": "Son",
                "next": "Sonraki",
                "previous": "Önceki"
            }
        },
        "dom": "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-7'i><'col-sm-12 col-md-5 text-right'p>>",
        "responsive": true,
        "lengthChange": false,
        "autoWidth": false,
        "pageLength": 10,
        "order": [[7, "desc"]], // Teslim tarihine göre sırala
        "data": [], // Başlangıçta boş veri
        "columns": [
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `<input type="checkbox" class="kalite-row-checkbox" data-id="${row.malzemeTalebiEssizID}" data-surecid="${row.malzemeTalepSurecTakipID}" />`;
                }
            },
            {
                "data": "sevkID",
                "render": function (data) {
                    return data || '-';
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
                "data": "islenenMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '0';
                }
            },
            {
                "data": "surecOlusturmaTarihi",
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
                        <button class="btn btn-sm btn-info btn-detay" data-id="${row.malzemeTalep.malzemeTalebiEssizID}" data-type="kalite" title="Detay">
                            <i class="fas fa-eye"></i>
                        </button>
                    `;
                }
            }
        ]
    });

    // Depo Kabul Tablosu
    if ($.fn.DataTable.isDataTable('#depoKabulTable')) {
        $('#depoKabulTable').DataTable().destroy();
    }

    window.depoKabulTable = $('#depoKabulTable').DataTable({
        "language": {
            "lengthMenu": "Sayfada _MENU_ kayıt göster",
            "zeroRecords": "Eşleşen kayıt bulunamadı",
            "info": "Toplam _TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
            "infoEmpty": "Gösterilecek kayıt yok",
            "infoFiltered": "(_MAX_ kayıt içerisinden filtrelendi)",
            "search": "Ara:",
            "paginate": {
                "first": "İlk",
                "last": "Son",
                "next": "Sonraki",
                "previous": "Önceki"
            }
        },
        "dom": "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-7'i><'col-sm-12 col-md-5 text-right'p>>",
        "responsive": true,
        "lengthChange": false,
        "autoWidth": false,
        "pageLength": 10,
        "order": [[8, "desc"]], // Onay tarihine göre sırala (desc)
        "data": [], // Başlangıçta boş veri
        "columns": [
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `<input type="checkbox" class="depo-kabul-row-checkbox" data-id="${row.malzemeTalebiEssizID}" data-surecid="${row.malzemeTalepSurecTakipID}" />`;
                }
            },
            {
                "data": "sevkID",
                "render": function (data) {
                    return data || '-';
                }
            },
            {
                "data": "onayId",
                "render": function (data) {
                    return data || '-';
                }
            },
            {
                "data": null,
                "render": function (data, type, row) {
                    return `${row.malzemeTalep.satSeriNo || '-'} / ${row.malzemeTalep.satSiraNo || '-'}`;
                }
            },
            {
                "data": "malzemeTalep.projeKodu",
                "render": function (data, type, row) {
                    return data || '-';
                }
            },
            {
                "data": "malzemeTalep.malzemeKodu",
                "render": function (data, type, row) {
                    return data || '-';
                }
            },
            {
                "data": "malzemeTalep.malzemeIsmi",
                "render": function (data, type, row) {
                    return data || '-';
                }
            },
            {
                "data": "islenenMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '0';
                }
            },
            {
                "data": "surecOlusturmaTarihi",
                "render": function (data, type, row) {
                    if (!data) return '-';
                    const date = new Date(data);
                    return date.toLocaleDateString('tr-TR') + ' ' + date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
                }
            },
            {
                "data": "paramTalepSurecStatuID",
                "orderable": false,
                "render": function (data, type, row) {
                    if (data === 8) {
                        return '<span class="badge badge-success">Kabul Edildi</span>';
                    } else if (data === 7) {
                        return '<span class="badge badge-danger">Red Edildi</span>';
                    } else {
                        return '<span class="badge badge-secondary">Bekliyor</span>';
                    }
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                        <button class="btn btn-sm btn-info btn-detay" data-id="${row.malzemeTalep.malzemeTalebiEssizID}" data-type="depo-kabul" title="Detay">
                            <i class="fas fa-eye"></i>
                        </button>
                    `;
                }
            }
        ],
        "rowCallback": function(row, data) {
            // Satır renklendirme
            if (data.paramTalepSurecStatuID === 8) {
                $(row).css({
                    'background-color': '#d4edda',
                    'color': '#155724'
                });
            } else if (data.paramTalepSurecStatuID === 7) {
                $(row).css({
                    'background-color': '#f8d7da',
                    'color': '#721c24'
                });
            }
            
            // Checkbox kontrol - sadece statü 6 olanlar seçilebilir
            const checkbox = $(row).find('.depo-kabul-row-checkbox');
            if (data.paramTalepSurecStatuID === 7 || data.paramTalepSurecStatuID === 8) {
                checkbox.prop('disabled', true);
            }
        }
    });

    // İmalat Ek Talepler Tablosu
    if ($.fn.DataTable.isDataTable('#imalatEkTaleplerTable')) {
        $('#imalatEkTaleplerTable').DataTable().destroy();
    }

    window.imalatEkTaleplerTable = $('#imalatEkTaleplerTable').DataTable({
        "language": {
            "lengthMenu": "Sayfada _MENU_ kayıt göster",
            "zeroRecords": "Eşleşen kayıt bulunamadı",
            "info": "Toplam _TOTAL_ kayıttan _START_ - _END_ arası gösteriliyor",
            "infoEmpty": "Gösterilecek kayıt yok",
            "infoFiltered": "(_MAX_ kayıt içerisinden filtrelendi)",
            "search": "Ara:",
            "paginate": {
                "first": "İlk",
                "last": "Son",
                "next": "Sonraki",
                "previous": "Önceki"
            }
        },
        "dom": "<'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-7'i><'col-sm-12 col-md-5 text-right'p>>",
        "responsive": true,
        "lengthChange": false,
        "autoWidth": false,
        "pageLength": 10,
        "order": [[8, "desc"]], // Kayıt tarihine göre sırala
        "data": [], // Başlangıçta boş veri
        "columns": [
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
                "data": "kalanMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '-';
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    const currentValue = row.malzemeTalep.buTalebiKarsilayanSATSeriNo || '';
                    return `<input type="text" class="form-control form-control-sm sat-seri-input"
                                   data-id="${row.malzemeTalebiEssizID}"
                                   value="${currentValue}"
                                   placeholder="Seri No"
                                   style="width: 100px; padding: 2px 5px; font-size: 13px;" />`;
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    const currentValue = row.malzemeTalep.buTalebiKarsilayanSATSiraNo || '';
                    return `<input type="text" class="form-control form-control-sm sat-sira-input"
                                   data-id="${row.malzemeTalebiEssizID}"
                                   value="${currentValue}"
                                   placeholder="Sıra No"
                                   style="width: 100px; padding: 2px 5px; font-size: 13px;" />`;
                }
            },
            {
                "data": "malzemeTalep.aciklama",
                "render": function (data, type, row) {
                    return data || '-';
                }
            },
            {
                "data": "malzemeTalep.satCariHesap",
                "render": function (data, type, row) {
                    return data || '-';
                }
            },
            {
                "data": "malzemeTalep.kayitTarihi",
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
                        <button class="btn btn-sm btn-info btn-detay" data-id="${row.malzemeTalep.malzemeTalebiEssizID}" data-type="imalat-ek" title="Detay">
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
        // İlk yükleme sırasında change event'ini engelle
        if (!window.isInitialLoad) {
            refreshMalzemeTalepTable();
        }
    });
    
    // Statü filtresi değişikliği
    $('#statuFilter').on('change', function () {
        // İlk yükleme sırasında change event'ini engelle
        if (!window.isInitialLoad) {
            refreshMalzemeTalepTable();
        }
    });
    
    // Depo Proje filtresi değişikliği
    $('#depoProjeFilter').on('change', function () {
        // İlk yükleme sırasında change event'ini engelle
        if (!window.isInitialLoad) {
            refreshDepoHazirlamaTable();
        }
    });
    
    // Üretim Proje filtresi değişikliği
    $('#uretimProjeFilter').on('change', function () {
        // İlk yükleme sırasında change event'ini engelle
        if (!window.isInitialLoad) {
            refreshUretimMalKabulTable();
        }
    });
    
    // Kalite Kontrol Proje filtresi değişikliği
    $('#kaliteProjeFilter').on('change', function () {
        // İlk yükleme sırasında change event'ini engelle
        if (!window.isInitialLoad) {
            refreshKaliteKontrolTable();
        }
    });

    // İmalat Ek Proje filtresi değişikliği
    $('#imalatEkProjeFilter').on('change', function () {
        // İlk yükleme sırasında change event'ini engelle
        if (!window.isInitialLoad) {
            refreshImalatEkTaleplerTable();
        }
    });

    // Malzeme arama input
    $('#malzemeSearchInput').on('keyup', function () {
        window.malzemeTalepTable.search($(this).val()).draw();
    });

    // Depo arama input
    $('#depoSearchInput').on('keyup', function () {
        window.depoHazirlamaTable.search($(this).val()).draw();
    });

    // Üretim arama input
    $('#uretimSearchInput').on('keyup', function () {
        window.uretimMalKabulTable.search($(this).val()).draw();
    });

    // Kalite Kontrol arama input
    $('#kaliteSearchInput').on('keyup', function () {
        window.kaliteKontrolTable.search($(this).val()).draw();
    });

    // Depo Kabul arama input
    $('#depoKabulSearchInput').on('keyup', function () {
        window.depoKabulTable.search($(this).val()).draw();
    });

    // İmalat Ek arama input
    $('#imalatEkSearchInput').on('keyup', function () {
        window.imalatEkTaleplerTable.search($(this).val()).draw();
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

    $('#btnExcelAktarUretim').on('click', function () {
        exportToExcel('uretim');
    });

    $('#btnExcelAktarKalite').on('click', function () {
        exportToExcel('kalite');
    });

    $('#btnExcelAktarDepoKabul').on('click', function () {
        exportToExcel('depo-kabul');
    });

    // Depo Kabul butonu
    $('#btnDepoKabul').on('click', function () {
        handleDepoKabul();
    });

    // Depo Red butonu
    $('#btnDepoRed').on('click', function () {
        handleDepoRed();
    });

    // Depo Karar Geri Al butonu
    $('#btnDepoKararGeriAl').on('click', function () {
        handleDepoKararGeriAl();
    });

    $('#btnExcelAktarImalat').on('click', function () {
        exportToExcel('imalat-ek');
    });

    // İmalat Ek Kaydet butonu
    $('#btnKaydetImalatEk').on('click', function () {
        kaydetImalatEkTalepler();
    });

    // Son İşlemi Geri Al butonu
    $('#btnSonIslemGeriAl').on('click', function () {
        handleSonIslemGeriAl();
    });

    // Depo Hazırlama Son İşlemi Geri Al butonu
    $('#btnDepoHazirlamaSonIslemGeriAl').on('click', function () {
        handleDepoHazirlamaSonIslemGeriAl();
    });

    // Üretim Mal Kabul Son İşlemi Geri Al butonu
    $('#btnUretimMalKabulSonIslemGeriAl').on('click', function () {
        handleUretimMalKabulSonIslemGeriAl();
    });

    // Kalite Kontrol Son İşlemi Geri Al butonu
    $('#btnKaliteKontrolSonIslemGeriAl').on('click', function () {
        handleKaliteKontrolSonIslemGeriAl();
    });

    // Hazırlandı butonu
    $('#btnHazirlandi').on('click', function () {
        handleHazirlandi();
    });

    // Mal Kabul butonu
    $('#btnMalKabul').on('click', function () {
        handleMalKabul();
    });

    // İade Et butonu
    $('#btnIadeEt').on('click', function () {
        handleIadeEt();
    });

    // Hasarlı butonu
    $('#btnHasarli').on('click', function () {
        handleHasarli();
    });

    // Kalite Mal Kabul butonu
    $('#btnKaliteMalKabul').on('click', function () {
        handleKaliteMalKabul();
    });

    // Tab değişikliği olayları
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        const targetTab = $(e.target).attr("href");
        
        // URL'e hash ekle (sayfa yenilendiğinde aynı tab açılsın)
        if (history.pushState) {
            history.pushState(null, null, targetTab);
        } else {
            window.location.hash = targetTab;
        }
        
        handleTabChange(targetTab);
    });

    // Tümünü seç checkbox'ı - Malzeme Talep Et tablosu
    // Sadece kalan miktar ile talep edilen miktar TAMAMEN AYNI olan satırları seç
    $('#selectAllMalzemeTalep').on('change', function () {
        const isChecked = $(this).is(':checked');
        let eligibleCount = 0;
        
        $('.row-checkbox').each(function() {
            const row = $(this).closest('tr');
            const talepMiktarInput = row.find('.talep-miktar-input');
            const talepMiktar = parseInt(talepMiktarInput.val()) || 0;
            const kalanMiktar = parseInt(talepMiktarInput.data('max')) || 0;
            const isEqual = talepMiktar === kalanMiktar && kalanMiktar > 0;
            
            if (isEqual) {
                // Eşit olan satırları seç/kaldır
                $(this).prop('checked', isChecked);
                eligibleCount++;
            } else {
                // Eşit olmayan satırların seçimini KALDIR
                $(this).prop('checked', false);
            }
        });
        
        // Eğer seçilebilir satır yoksa "Tümünü Seç"i kaldır
        if (eligibleCount === 0 && isChecked) {
            $(this).prop('checked', false);
            Swal.fire({
                icon: 'info',
                title: 'Bilgi',
                text: 'Tümünü seç özelliği sadece kalan miktar ile talep edilen miktar eşit olan satırlar için geçerlidir.',
                confirmButtonText: 'Tamam'
            });
        }
    });

    // Tekil checkbox'lar değiştiğinde - seçim uyumluluğu kontrolü
    $(document).on('change', '.row-checkbox', function () {
        const isChecked = $(this).is(':checked');
        
        if (isChecked) {
            // Checkbox işaretlenmeye çalışılıyor, kontrol et
            const row = $(this).closest('tr');
            const talepMiktarInput = row.find('.talep-miktar-input');
            const talepMiktar = parseInt(talepMiktarInput.val()) || 0;
            const kalanMiktar = parseInt(talepMiktarInput.data('max')) || 0;
            const isCurrentEqual = talepMiktar === kalanMiktar;
            
            // Eğer başka seçili satırlar varsa kontrol et
            const otherCheckedBoxes = $('.row-checkbox:checked').not(this);
            
            if (otherCheckedBoxes.length > 0) {
                // En az bir tane daha seçili satır var
                let hasEqualRows = false;
                let hasDifferentRows = false;
                
                otherCheckedBoxes.each(function() {
                    const otherRow = $(this).closest('tr');
                    const otherTalepMiktarInput = otherRow.find('.talep-miktar-input');
                    const otherTalepMiktar = parseInt(otherTalepMiktarInput.val()) || 0;
                    const otherKalanMiktar = parseInt(otherTalepMiktarInput.data('max')) || 0;
                    
                    if (otherTalepMiktar === otherKalanMiktar) {
                        hasEqualRows = true;
                    } else {
                        hasDifferentRows = true;
                    }
                });
                
                // KURAL: Eşit (kalan=talep) ve Farklı (kalan≠talep) satırlar KARIŞTIRILAMAZ
                if (isCurrentEqual && hasDifferentRows) {
                    // Şu an seçilmeye çalışılan satır eşit, ama zaten farklı satırlar seçili
                    $(this).prop('checked', false);
                    Swal.fire({
                        icon: 'warning',
                        title: 'Uyarı',
                        text: 'Talep miktarı farklı olan satırlar seçiliyken, eşit olan satırlar seçilemez. Önce diğer seçimleri kaldırın.',
                        confirmButtonText: 'Tamam',
                        timer: 3000
                    });
                    return;
                } else if (!isCurrentEqual && hasEqualRows) {
                    // Şu an seçilmeye çalışılan satır farklı, ama zaten eşit satırlar seçili
                    $(this).prop('checked', false);
                    Swal.fire({
                        icon: 'warning',
                        title: 'Uyarı',
                        text: 'Talep miktarı eşit olan satırlar seçiliyken, farklı olan satırlar seçilemez. Önce diğer seçimleri kaldırın.',
                        confirmButtonText: 'Tamam',
                        timer: 3000
                    });
                    return;
                } else if (!isCurrentEqual && hasDifferentRows) {
                    // Şu an seçilmeye çalışılan satır farklı, ve zaten farklı satırlar var
                    // SADECE 1 TANE farklı satır seçilebilir
                    $(this).prop('checked', false);
                    Swal.fire({
                        icon: 'warning',
                        title: 'Uyarı',
                        text: 'Talep miktarı farklı olan satırlardan sadece 1 tane seçilebilir.',
                        confirmButtonText: 'Tamam',
                        timer: 3000
                    });
                    return;
                }
            }
        }
        
        updateSelectAllCheckbox();
    });

    // Tümünü seç checkbox'ı - Depo Hazırlama tablosu
    $('#selectAllDepoHazirlama').on('change', function () {
        const isChecked = $(this).is(':checked');
        $('.depo-row-checkbox').prop('checked', isChecked);
    });

    // Depo checkbox'ları değiştiğinde - tümünü seç durumunu güncelle
    $(document).on('change', '.depo-row-checkbox', function () {
        updateSelectAllDepoCheckbox();
    });

    // Tümünü seç checkbox'ı - Üretim Mal Kabul tablosu
    $('#selectAllUretimMalKabul').on('change', function () {
        const isChecked = $(this).is(':checked');
        $('.uretim-row-checkbox').prop('checked', isChecked);
    });

    // Tümünü seç checkbox'ı - Depo Kabul tablosu
    $('#selectAllDepoKabul').on('change', function () {
        const isChecked = $(this).is(':checked');
        // Sadece aktif (disabled olmayan) checkbox'ları seç
        $('.depo-kabul-row-checkbox:not(:disabled)').prop('checked', isChecked);
    });

    // Üretim checkbox'ları değiştiğinde - tümünü seç durumunu güncelle
    $(document).on('change', '.uretim-row-checkbox', function () {
        updateSelectAllUretimCheckbox();
    });

    // Depo Kabul checkbox'ları değiştiğinde - tümünü seç durumunu güncelle
    $(document).on('change', '.depo-kabul-row-checkbox', function () {
        updateSelectAllDepoKabulCheckbox();
    });

    // Kalite kontrol checkbox'ları değiştiğinde - sadece 1 tane seçilebilir
    $(document).on('change', '.kalite-row-checkbox', function () {
        if ($(this).is(':checked')) {
            // Diğer tüm checkbox'ları kaldır
            $('.kalite-row-checkbox').not(this).prop('checked', false);
        }
    });

    // Depo kabul checkbox'ları değiştiğinde - sadece 1 tane seçilebilir
    $(document).on('change', '.depo-kabul-row-checkbox', function () {
        if ($(this).is(':checked')) {
            // Diğer tüm checkbox'ları kaldır
            $('.depo-kabul-row-checkbox').not(this).prop('checked', false);
        }
    });

    // Custom page length değişikliği - Malzeme Talep
    $('#malzemeTalepPageLength').on('change', function () {
        const length = parseInt($(this).val());
        window.malzemeTalepTable.page.len(length).draw();
    });

    // Custom page length değişikliği - Depo Hazırlama
    $('#depoHazirlamaPageLength').on('change', function () {
        const length = parseInt($(this).val());
        window.depoHazirlamaTable.page.len(length).draw();
    });

    // Custom page length değişikliği - Üretim Mal Kabul
    $('#uretimMalKabulPageLength').on('change', function () {
        const length = parseInt($(this).val());
        window.uretimMalKabulTable.page.len(length).draw();
    });

    // Custom page length değişikliği - Kalite Kontrol
    $('#kaliteKontrolPageLength').on('change', function () {
        const length = parseInt($(this).val());
        window.kaliteKontrolTable.page.len(length).draw();
    });

    // Custom page length değişikliği - Depo Kabul
    $('#depoKabulPageLength').on('change', function () {
        const length = parseInt($(this).val());
        window.depoKabulTable.page.len(length).draw();
    });

    // Custom page length değişikliği - İmalat Ek
    $('#imalatEkPageLength').on('change', function () {
        const length = parseInt($(this).val());
        window.imalatEkTaleplerTable.page.len(length).draw();
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

    // Talep Edilen Miktar input değişikliği - blur event'inde kontrol
    $(document).on('blur', '.talep-miktar-input', function () {
        const maxValue = parseInt($(this).data('max'));
        let currentValue = parseInt($(this).val());
        
        // NaN kontrolü
        if (isNaN(currentValue)) {
            currentValue = 0;
            $(this).val(0);
        }
        
        // Negatif değer kontrolü
        if (currentValue < 0) {
            $(this).val(0);
            currentValue = 0;
        }
        
        // Eğer talep edilen miktar kalan miktardan farklıysa, checkbox'ı kaldır ve tümünü seç'i kaldır
        const row = $(this).closest('tr');
        const checkbox = row.find('.row-checkbox');
        
        if (currentValue !== maxValue) {
            checkbox.prop('checked', false);
            // Input değiştiğinde tümünü seç'i MUTLAKA kaldır
            $('#selectAllMalzemeTalep').prop('checked', false);
        } else {
            // Değer tekrar kalan miktara eşit oldu, tümünü seç'i kontrol et
            setTimeout(function() {
                updateSelectAllCheckbox();
            }, 0);
        }
    });
}

/**
 * Tümünü seç checkbox'ını güncelle
 * Sadece kalan miktar = talep edilen miktar olan satırlar kontrol edilir
 */
function updateSelectAllCheckbox() {
    let eligibleCount = 0; // Seçilebilir satır sayısı (kalan = talep)
    let selectedEligibleCount = 0; // Seçilebilir satırlardan seçili olanlar

    $('.row-checkbox').each(function() {
        const row = $(this).closest('tr');
        const talepMiktarInput = row.find('.talep-miktar-input');

        if (talepMiktarInput.length === 0) return; // Input yoksa atla

        const talepMiktar = parseInt(talepMiktarInput.val()) || 0;
        const kalanMiktar = parseInt(talepMiktarInput.data('max')) || 0;

        // Sadece kalan miktar = talep edilen miktar olan satırları say
        if (talepMiktar === kalanMiktar && kalanMiktar > 0) {
            eligibleCount++;
            if ($(this).is(':checked')) {
                selectedEligibleCount++;
            }
        }
    });

    // Tümünü seç checkbox'ını güncelle:
    // - Seçilebilir satır yoksa → kaldır
    // - Seçilebilir satırların hepsi seçili → işaretle
    // - Seçilebilir satırlardan biri bile seçili değil → kaldır
    const shouldBeChecked = eligibleCount > 0 && selectedEligibleCount === eligibleCount;

    $('#selectAllMalzemeTalep').prop('checked', shouldBeChecked);
}

/**
 * Depo Hazırlama Tümünü seç checkbox'ını güncelle
 */
function updateSelectAllDepoCheckbox() {
    const totalCheckboxes = $('.depo-row-checkbox').length;
    const checkedCheckboxes = $('.depo-row-checkbox:checked').length;

    $('#selectAllDepoHazirlama').prop('checked', totalCheckboxes > 0 && totalCheckboxes === checkedCheckboxes);
}

/**
 * Üretim Mal Kabul Tümünü seç checkbox'ını güncelle
 */
function updateSelectAllUretimCheckbox() {
    const totalCheckboxes = $('.uretim-row-checkbox').length;
    const checkedCheckboxes = $('.uretim-row-checkbox:checked').length;

    $('#selectAllUretimMalKabul').prop('checked', totalCheckboxes > 0 && totalCheckboxes === checkedCheckboxes);
}

/**
 * Depo Kabul Tümünü seç checkbox'ını güncelle
 */
function updateSelectAllDepoKabulCheckbox() {
    const totalActiveCheckboxes = $('.depo-kabul-row-checkbox:not(:disabled)').length;
    const checkedCheckboxes = $('.depo-kabul-row-checkbox:checked').length;

    $('#selectAllDepoKabul').prop('checked', totalActiveCheckboxes > 0 && totalActiveCheckboxes === checkedCheckboxes);
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
    
    $('#depoProjeFilter').select2({
        theme: 'bootstrap4',
        placeholder: 'Tümü',
        allowClear: false
    });
    
    $('#uretimProjeFilter').select2({
        theme: 'bootstrap4',
        placeholder: 'Tümü',
        allowClear: false
    });
    
    $('#kaliteProjeFilter').select2({
        theme: 'bootstrap4',
        placeholder: 'Tümü',
        allowClear: false
    });
    
    $('#depoKabulProjeFilter').select2({
        theme: 'bootstrap4',
        placeholder: 'Tümü',
        allowClear: false
    });

    $('#imalatEkProjeFilter').select2({
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
                const uretimSelect = $('#uretimProjeFilter');
                const kaliteSelect = $('#kaliteProjeFilter');
                const depoKabulSelect = $('#depoKabulProjeFilter');
                const imalatEkSelect = $('#imalatEkProjeFilter');
                
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
                    
                    // Üretim Mal Kabul için
                    const uretimOption = new Option(
                        optionText,
                        proje.projeKodu, // Value olarak projeKodu kullan
                        proje.tabloID === 0, // Tümü seçili olsun
                        proje.tabloID === 0
                    );
                    uretimSelect.append(uretimOption);
                    
                    // Kalite Kontrol için
                    const kaliteOption = new Option(
                        optionText,
                        proje.projeKodu, // Value olarak projeKodu kullan
                        proje.tabloID === 0, // Tümü seçili olsun
                        proje.tabloID === 0
                    );
                    kaliteSelect.append(kaliteOption);
                    
                    // Depo Kabul için
                    const depoKabulOption = new Option(
                        optionText,
                        proje.projeKodu, // Value olarak projeKodu kullan
                        proje.tabloID === 0, // Tümü seçili olsun
                        proje.tabloID === 0
                    );
                    depoKabulSelect.append(depoKabulOption);

                    // İmalat Ek Talepler için
                    const imalatEkOption = new Option(
                        optionText,
                        proje.projeKodu, // Value olarak projeKodu kullan
                        proje.tabloID === 0, // Tümü seçili olsun
                        proje.tabloID === 0
                    );
                    imalatEkSelect.append(imalatEkOption);
                });

                // Select2'yi güncelle ama change event'ini tetikleme
                select.trigger('change.select2');
                depoSelect.trigger('change.select2');
                uretimSelect.trigger('change.select2');
                kaliteSelect.trigger('change.select2');
                depoKabulSelect.trigger('change.select2');
                imalatEkSelect.trigger('change.select2');
                
                // İlk yükleme tamamlandı, şimdi verileri çek
                if (window.isInitialLoad) {
                    window.isInitialLoad = false;
                    // Spinner zaten gösteriliyor, direkt veri çekmeye başla
                    refreshMalzemeTalepTable();
                }
            }
        },
        error: function (error) {
            console.error('Proje listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Proje listesi yüklenemedi!'
            });
            
            // Hata durumunda da ilk yüklemeyi işaretle
            if (window.isInitialLoad) {
                window.isInitialLoad = false;
            }
        }
    });
}

/**
 * Statü listesini yükle
 */
function loadStatuList() {
    const select = $('#statuFilter');
    
    // Sabit statü seçeneklerini ekle
    select.append(new Option('Tümü', '0', true, true));
    select.append(new Option('Geldi', '1', false, false));
    select.append(new Option('Gelmedi', '2', false, false));
    
    select.trigger('change');
}

/**
 * Departman listesini yükle
 */
function loadDepartmanList() {
    $.ajax({
        url: '/panel/GetOrganizasyonBirimleri',
        type: 'GET',
        data: { kurumId: 2, tipId: 1 },
        success: function(response) {
            if (response.isSuccess && response.value) {
                departmanListesi = response.value;
                console.log('Departman listesi yüklendi:', departmanListesi);
            }
        },
        error: function(xhr, status, error) {
            console.error('Departman listesi yüklenirken hata:', error);
        }
    });
}

/**
 * Departman ID'ye göre departman adını getir
 */
function getDepartmanAdi(departmanID) {
    const departman = departmanListesi.find(d => d.tabloId === departmanID);
    return departman ? departman.tanim : 'Bilinmeyen Departman';
}

/**
 * İlk verileri yükleme
 */
function loadInitialData() {
    // İlk yükleme için bayrak
    window.isInitialLoad = true;
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
        case '#uretimMalKabulTabContent':
            if (window.uretimMalKabulTable) {
                window.uretimMalKabulTable.columns.adjust().draw();
            }
            refreshUretimMalKabulTable();
            break;
        case '#kaliteKontrolTabContent':
            if (window.kaliteKontrolTable) {
                window.kaliteKontrolTable.columns.adjust().draw();
            }
            refreshKaliteKontrolTable();
            break;
        case '#depoKabulTabContent':
            if (window.depoKabulTable) {
                window.depoKabulTable.columns.adjust().draw();
            }
            refreshDepoKabulTable();
            break;
        case '#imalatEkTaleplerTabContent':
            if (window.imalatEkTaleplerTable) {
                window.imalatEkTaleplerTable.columns.adjust().draw();
            }
            refreshImalatEkTaleplerTable();
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
            text: 'Lütfen en az bir kayıt seçiniz!'
        });
        return;
    }
    
    // Her seçili satır için talep edilen miktarı topla
    const items = [];
    const projeGroups = {}; // Proje bazlı gruplama için
    const overRequestedItems = []; // Fazla talep edilen malzemeler
    let hasError = false;
    
    checkedBoxes.each(function() {
        const malzemeTalebiEssizID = $(this).data('id');
        const row = $(this).closest('tr');
        const talepMiktarInput = row.find('.talep-miktar-input');
        const talepMiktar = parseInt(talepMiktarInput.val());
        const maxMiktar = parseInt(talepMiktarInput.data('max'));
        
        // Proje kodunu DataTable'dan al
        const rowData = window.malzemeTalepTable.row(row).data();
        const projeKodu = rowData.malzemeTalep.projeKodu;
        const malzemeKodu = rowData.malzemeTalep.malzemeKodu;
        
        if (!talepMiktar || talepMiktar <= 0) {
            hasError = true;
            Swal.fire({
                icon: 'warning',
                title: 'Uyarı',
                text: 'Lütfen geçerli bir talep miktarı giriniz!'
            });
            return false; // Break the loop
        }
        
        if (talepMiktar > maxMiktar) {
            overRequestedItems.push({
                malzemeKodu: malzemeKodu,
                projeKodu: projeKodu,
                talepMiktar: talepMiktar,
                kalanMiktar: maxMiktar
            });
            // Fazla talep olsa bile listeye ekle, sonra topluca uyarı göstereceğiz
        }
        
        items.push({
            malzemeTalebiEssizID: malzemeTalebiEssizID,
            sevkEdilenMiktar: talepMiktar
        });
        
        // Proje bazlı gruplama
        if (!projeGroups[projeKodu]) {
            projeGroups[projeKodu] = 0;
        }
        projeGroups[projeKodu]++;
    });
    
    if (hasError) {
        return;
    }
    
    // Departman seçimi için popup göster
    showDepartmanSecimPopup(items, projeGroups, overRequestedItems);
}

/**
 * Departman seçim popup'unu göster
 */
function showDepartmanSecimPopup(items, projeGroups, overRequestedItems) {
    // Departman listesini HTML olarak hazırla
    let departmanOptionsHtml = '<option value="">Departman Seçiniz</option>';
    departmanListesi.forEach(function(departman) {
        departmanOptionsHtml += `<option value="${departman.tabloId}">${departman.tanim}</option>`;
    });
    
    Swal.fire({
        title: 'Departman Seçimi',
        html: `
            <div class="form-group text-left" style="margin-bottom: 15px;">
                <label for="departmanSelect" style="display: block; margin-bottom: 8px; font-weight: normal;">Talep Eden Departman:</label>
                <select id="departmanSelect" class="form-control" style="width: 100%; padding: 8px;">
                    ${departmanOptionsHtml}
                </select>
                <small class="form-text text-muted" style="display: block; margin-top: 5px;">Tüm malzemeler bu departman adına talep edilecektir.</small>
            </div>
        `,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Devam Et',
        cancelButtonText: 'İptal',
        confirmButtonColor: '#007bff',
        cancelButtonColor: '#6c757d',
        width: '500px',
        preConfirm: () => {
            const departmanId = document.getElementById('departmanSelect').value;
            if (!departmanId) {
                Swal.showValidationMessage('Lütfen bir departman seçiniz!');
                return false;
            }
            return parseInt(departmanId);
        }
    }).then((result) => {
        if (result.isConfirmed && result.value) {
            // Seçilen departman ID'sini tüm item'lara ekle
            const departmanId = result.value;
            items.forEach(function(item) {
                item.malzemeSevkTalebiYapanDepartmanID = departmanId;
            });
            
            // Request data hazırla
            const requestData = {
                talepItems: items
            };
            
            // Onay popup'ını göster
            showTalepOnayPopup(requestData, projeGroups, overRequestedItems);
        }
    });
}

/**
 * Talep onay popup'unu göster ve API çağrısı yap
 */
function showTalepOnayPopup(requestData, projeGroups, overRequestedItems) {
    // Onay mesajını oluştur
    let confirmMessage = '';
    const projeKeys = Object.keys(projeGroups);
    
    if (projeKeys.length === 1) {
        // Tek proje
        const projeKodu = projeKeys[0];
        const kalemSayisi = projeGroups[projeKodu];
        confirmMessage = `${kalemSayisi} kalem ${projeKodu} projesine talep edilecektir. Emin misiniz?`;
    } else {
        // Birden fazla proje
        const projeListesi = projeKeys.map(proje => `${projeGroups[proje]} kalem ${proje}`).join(', ');
        confirmMessage = `${projeListesi} projesine talep edilecektir. Emin misiniz?`;
    }
    
    // Fazla talep edilen malzemeler varsa uyarı ekle
    if (overRequestedItems.length > 0) {
        confirmMessage += '\n\n⚠️ UYARI: Aşağıdaki malzemeler kalan miktardan fazla talep edilmiştir:\n\n';
        overRequestedItems.forEach(function(item) {
            confirmMessage += `• ${item.malzemeKodu} (Proje: ${item.projeKodu}) - Talep: ${item.talepMiktar}, Kalan: ${item.kalanMiktar}\n`;
        });
        confirmMessage += '\nYine de devam etmek istiyor musunuz?';
    }
    
    // Onay modalı göster
    Swal.fire({
        title: overRequestedItems.length > 0 ? 'Dikkat!' : 'Onay',
        text: confirmMessage,
        icon: overRequestedItems.length > 0 ? 'warning' : 'question',
        showCancelButton: true,
        confirmButtonText: 'Evet, Talep Et',
        cancelButtonText: 'İptal',
        confirmButtonColor: overRequestedItems.length > 0 ? '#ff9800' : '#007bff',
        cancelButtonColor: '#6c757d'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/MalzemeTalepEt',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(requestData),
                success: function(response) {
                    if (response.isSuccess) {
                        // Servisten gelen mesajı göster (value string olarak dönüyor)
                        const successMessage = response.value || 'Malzeme talepleri başarıyla oluşturuldu!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#007bff'
                        }).then(() => {
                            // Checkbox'ları temizle
                            $('.row-checkbox').prop('checked', false);
                            $('#selectAllMalzemeTalep').prop('checked', false);
                            
                            // Tabloyu mevcut filtreler ile yenile
                            refreshMalzemeTalepTable();
                        });
                    } else {
                        // Hata durumunda da servisten gelen mesajı göster
                        let errorMessage = 'Talepler oluşturulamadı!';
                        
                        if (response.value && typeof response.value === 'string') {
                            errorMessage = response.value;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function(error) {
                    console.error('Talep Et - Error Response:', error);
                    
                    let errorMessage = 'Talepler oluşturulamadı!';
                    
                    // Servisten string mesaj dönebilir
                    if (error.responseJSON) {
                        if (error.responseJSON.value && typeof error.responseJSON.value === 'string') {
                            errorMessage = error.responseJSON.value;
                        } else if (error.responseJSON.errors && error.responseJSON.errors.length > 0) {
                            errorMessage = error.responseJSON.errors.join(', ');
                        }
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
        }
    });
}

/**
 * Talep kaydet butonu - ARTIK KULLANILMIYOR (Direkt yeniTalepEkle fonksiyonu içinde yapılıyor)
 */
// $(document).on('click', '#btnTalepKaydet', function() { ... });

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
    // Çift istek önleme kontrolü
    if (isLoadingMalzemeTalep) {
        console.log('Malzeme Talep verileri zaten yükleniyor...');
        return;
    }
    
    // Loading durumunu aktif et
    isLoadingMalzemeTalep = true;
    
    // Spinner'ı göster, tabloyu gizle (ilk yüklemede zaten gösteriliyor olabilir)
    if (!$('#malzemeTalepSpinner').is(':visible')) {
        $('#malzemeTalepSpinner').show();
    }
    $('#malzemeTalepTable').hide();
    
    // Filtre değerlerini al
    const projeKoduValue = $('#projeFilter').val();
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
                $('.row-checkbox').prop('checked', false);
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
        },
        complete: function() {
            // Loading durumunu kapat
            isLoadingMalzemeTalep = false;
            
            // Spinner'ı gizle, tabloyu göster
            $('#malzemeTalepSpinner').hide();
            $('#malzemeTalepTable').show();
        }
    });
}

/**
 * Depo Hazırlama tablosunu yenileme
 */
function refreshDepoHazirlamaTable() {
    // Çift istek önleme kontrolü
    if (isLoadingDepoHazirlama) {
        console.log('Depo Hazırlama verileri zaten yükleniyor...');
        return;
    }
    
    // Loading durumunu aktif et
    isLoadingDepoHazirlama = true;
    
    // Spinner'ı göster, tabloyu gizle
    $('#depoHazirlamaSpinner').show();
    $('#depoHazirlamaTable').hide();
    
    // Filtre değerlerini al
    const projeKoduValue = $('#depoProjeFilter').val();
    
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
        },
        complete: function() {
            // Loading durumunu kapat
            isLoadingDepoHazirlama = false;
            
            // Spinner'ı gizle, tabloyu göster
            $('#depoHazirlamaSpinner').hide();
            $('#depoHazirlamaTable').show();
        }
    });
}

/**
 * Üretim Mal Kabul tablosunu yenileme
 */
function refreshUretimMalKabulTable() {
    // Çift istek önleme kontrolü
    if (isLoadingUretimMalKabul) {
        console.log('Üretim Mal Kabul verileri zaten yükleniyor...');
        return;
    }
    
    // Loading durumunu aktif et
    isLoadingUretimMalKabul = true;
    
    // Spinner'ı göster, tabloyu gizle
    $('#uretimMalKabulSpinner').show();
    $('#uretimMalKabulTable').hide();
    
    // Filtre değerlerini al
    const projeKoduValue = $('#uretimProjeFilter').val();
    
    // Üretim Mal Kabul için sabit parametreler
    // talepSurecStatuIDs: [4, 7] (Üretim Mal Kabul ve İade Edilen)
    // malzemeTalepEtGetir: false
    const requestData = {
        projeKodu: projeKoduValue === 0 ? 0 : projeKoduValue, // Tümü için 0, seçiliyse projeKodu gönder
        talepSurecStatuIDs: [4, 7], // Üretim Mal Kabul (4) ve İade Edilen (7)
        searchText: "",
        malzemeTalepEtGetir: false // Üretim için her zaman false
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
                if (window.uretimMalKabulTable) {
                    window.uretimMalKabulTable.clear();
                    window.uretimMalKabulTable.rows.add(response.value);
                    window.uretimMalKabulTable.draw();
                } else {
                    console.error('window.uretimMalKabulTable bulunamadı!');
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
            console.error('Üretim Mal Kabul listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Üretim Mal Kabul listesi yüklenemedi!'
            });
        },
        complete: function() {
            // Loading durumunu kapat
            isLoadingUretimMalKabul = false;
            
            // Spinner'ı gizle, tabloyu göster
            $('#uretimMalKabulSpinner').hide();
            $('#uretimMalKabulTable').show();
        }
    });
}

/**
 * Kalite Kontrol tablosunu yenileme
 */
function refreshKaliteKontrolTable() {
    // Çift istek önleme kontrolü
    if (isLoadingKaliteKontrol) {
        console.log('Kalite Kontrol verileri zaten yükleniyor...');
        return;
    }
    
    // Loading durumunu aktif et
    isLoadingKaliteKontrol = true;
    
    // Spinner'ı göster, tabloyu gizle
    $('#kaliteKontrolSpinner').show();
    $('#kaliteKontrolTable').hide();
    
    // Filtre değerlerini al
    const projeKoduValue = $('#kaliteProjeFilter').val();
    
    // Kalite Kontrol için sabit parametreler
    // talepSurecStatuIDs: [5] (Kalite Kontrol statüsü)
    // malzemeTalepEtGetir: false
    const requestData = {
        projeKodu: projeKoduValue === 0 ? 0 : projeKoduValue, // Tümü için 0, seçiliyse projeKodu gönder
        talepSurecStatuIDs: [5], // Kalite Kontrol için her zaman 5
        searchText: "",
        malzemeTalepEtGetir: false // Kalite kontrol için her zaman false
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
                if (window.kaliteKontrolTable) {
                    window.kaliteKontrolTable.clear();
                    window.kaliteKontrolTable.rows.add(response.value);
                    window.kaliteKontrolTable.draw();
                } else {
                    console.error('window.kaliteKontrolTable bulunamadı!');
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
            console.error('Kalite Kontrol listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Kalite Kontrol listesi yüklenemedi!'
            });
        },
        complete: function() {
            // Loading durumunu kapat
            isLoadingKaliteKontrol = false;
            
            // Spinner'ı gizle, tabloyu göster
            $('#kaliteKontrolSpinner').hide();
            $('#kaliteKontrolTable').show();
        }
    });
}

/**
 * Depo Kabul tablosunu yenileme
 */
function refreshDepoKabulTable() {
    // Çift istek önleme kontrolü
    if (isLoadingDepoKabul) {
        console.log('Depo Kabul verileri zaten yükleniyor...');
        return;
    }
    
    // Loading durumunu aktif et
    isLoadingDepoKabul = true;
    
    // Spinner'ı göster, tabloyu gizle
    $('#depoKabulSpinner').show();
    $('#depoKabulTable').hide();
    
    // Filtre değerlerini al
    const projeKoduValue = $('#depoKabulProjeFilter').val();
    
    // Depo Kabul için sabit parametreler
    // talepSurecStatuIDs: [6, 7, 8] (Depo Kabul, Red Edildi, Kabul Edildi statüleri)
    // malzemeTalepEtGetir: false
    const requestData = {
        projeKodu: projeKoduValue === 0 ? 0 : projeKoduValue, // Tümü için 0, seçiliyse projeKodu gönder
        talepSurecStatuIDs: [6, 8], // Depo Kabul için 6, 7 ve 8
        searchText: "",
        malzemeTalepEtGetir: false // Depo kabul için her zaman false
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
                if (window.depoKabulTable) {
                    window.depoKabulTable.clear();
                    window.depoKabulTable.rows.add(response.value);
                    window.depoKabulTable.draw();
                } else {
                    console.error('window.depoKabulTable bulunamadı!');
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
            console.error('Depo Kabul listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Depo Kabul listesi yüklenirken bir hata oluştu!'
            });
        },
        complete: function () {
            // Loading durumunu pasif et
            isLoadingDepoKabul = false;
            
            // Spinner'ı gizle, tabloyu göster
            $('#depoKabulSpinner').hide();
            $('#depoKabulTable').show();
        }
    });
}

/**
 * İmalat Ek Talepler tablosunu yenileme
 */
function refreshImalatEkTaleplerTable() {
    // Çift istek önleme kontrolü
    if (isLoadingImalatEk) {
        console.log('İmalat Ek Talepler verileri zaten yükleniyor...');
        return;
    }

    // Loading durumunu aktif et
    isLoadingImalatEk = true;

    // Spinner'ı göster, tabloyu gizle
    $('#imalatEkSpinner').show();
    $('#imalatEkTaleplerTable').hide();

    // Filtre değerlerini al
    const projeKoduValue = $('#imalatEkProjeFilter').val();

    // İmalat Ek Talepler için parametreler
    // talepSurecStatuIDs: [] (boş)
    // malzemeTalepEtGetir: false
    // sadeceEkTalepleriGetir: true
    const requestData = {
        projeKodu: projeKoduValue === 0 ? 0 : projeKoduValue, // Tümü için 0, seçiliyse projeKodu gönder
        talepSurecStatuIDs: [], // İmalat Ek için boş
        searchText: "",
        malzemeTalepEtGetir: false,
        sadeceEkTalepleriGetir: true // İmalat Ek için her zaman true
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
                if (window.imalatEkTaleplerTable) {
                    window.imalatEkTaleplerTable.clear();
                    window.imalatEkTaleplerTable.rows.add(response.value);
                    window.imalatEkTaleplerTable.draw();
                } else {
                    console.error('window.imalatEkTaleplerTable bulunamadı!');
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
            console.error('İmalat Ek Talepler listesi yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'İmalat Ek Talepler listesi yüklenirken bir hata oluştu!'
            });
        },
        complete: function () {
            // Loading durumunu pasif et
            isLoadingImalatEk = false;

            // Spinner'ı gizle, tabloyu göster
            $('#imalatEkSpinner').hide();
            $('#imalatEkTaleplerTable').show();
        }
    });
}

/**
 * İmalat Ek Talepler SAT bilgilerini kaydet
 */
function kaydetImalatEkTalepler() {
    // Tablodaki tüm SAT Seri ve Sıra No inputlarını topla
    const items = [];

    $('#imalatEkTaleplerTable tbody tr').each(function () {
        const row = window.imalatEkTaleplerTable.row(this);
        const rowData = row.data();

        if (rowData) {
            const seriInput = $(this).find('.sat-seri-input');
            const siraInput = $(this).find('.sat-sira-input');

            const malzemeTalebiEssizID = rowData.malzemeTalebiEssizID;
            const seriNo = seriInput.val() || null;
            const siraNo = siraInput.val() || null;

            items.push({
                malzemeTalebiEssizID: malzemeTalebiEssizID,
                buTalebiKarsilayanSATSeriNo: seriNo,
                buTalebiKarsilayanSATSiraNo: siraNo
            });
        }
    });

    if (items.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Kaydedilecek veri bulunamadı!'
        });
        return;
    }

    // Kullanıcıya onay sorusu sor
    Swal.fire({
        title: 'Emin misiniz?',
        text: 'SAT bilgilerini güncellemek istediğinizden emin misiniz?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, Güncelle',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API'ye POST isteği gönder
            $.ajax({
                url: '/panel/TopluSATBilgisiGuncelle',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ items: items }),
                success: function (response) {
                    console.log('TopluSATBilgisiGuncelle Response:', response);

                    if (response.isSuccess) {
                        // Servisten gelen mesajı göster (value string olarak dönüyor)
                        const successMessage = response.value || 'SAT bilgileri başarıyla güncellendi!';

                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#007bff'
                        }).then(() => {
                            // Tabloyu yenile
                            refreshImalatEkTaleplerTable();
                        });
                    } else {
                        // Hata durumunda servisten gelen mesajı göster
                        let errorMessage = 'Güncelleme sırasında bir hata oluştu!';

                        if (response.value && typeof response.value === 'string') {
                            errorMessage = response.value;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }

                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (error) {
                    console.error('SAT bilgileri güncellenirken hata:', error);

                    let errorMessage = 'SAT bilgileri güncellenirken bir hata oluştu!';

                    // Servisten string mesaj dönebilir
                    if (error.responseJSON) {
                        if (error.responseJSON.value && typeof error.responseJSON.value === 'string') {
                            errorMessage = error.responseJSON.value;
                        } else if (error.responseJSON.errors && error.responseJSON.errors.length > 0) {
                            errorMessage = error.responseJSON.errors.join(', ');
                        }
                    } else if (error.responseText) {
                        try {
                            const errorData = JSON.parse(error.responseText);
                            if (errorData.value) {
                                errorMessage = errorData.value;
                            }
                        } catch (e) {
                            errorMessage = error.responseText;
                        }
                    }

                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * Excel'e aktarma
 */
function exportToExcel(type) {
    try {
        let table;
        if (type === 'malzeme') {
            table = window.malzemeTalepTable;
        } else if (type === 'depo') {
            table = window.depoHazirlamaTable;
        } else if (type === 'uretim') {
            table = window.uretimMalKabulTable;
        } else if (type === 'kalite') {
            table = window.kaliteKontrolTable;
        } else if (type === 'depo-kabul') {
            table = window.depoKabulTable;
        } else if (type === 'imalat-ek') {
            table = window.imalatEkTaleplerTable;
        }
        
        if (!table) {
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Tablo bulunamadı!'
            });
            return;
        }

        // Tablodan tüm verileri al (filtrelenmiş haliyle)
        const data = table.rows({ search: 'applied' }).data().toArray();

        if (data.length === 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Uyarı',
                text: 'Aktarılacak veri bulunamadı!'
            });
            return;
        }

        // Excel için veri hazırlama
        const excelData = [];
        
        if (type === 'malzeme') {
            // Başlık satırı
            excelData.push([
                'SAT Seri No',
                'SAT Sıra No',
                'Malzeme Talep Kodu',
                'Proje Kodu',
                'Malzeme Kodu',
                'Malzeme İsmi',
                'Talep Edilen Miktar',
                'Sevk Edilen Miktar',
                'Kalan Miktar',
                'Oluşturma Tarihi'
            ]);

            // Veri satırları
            data.forEach(item => {
                const talepKodu = `${item.malzemeTalep?.satSeriNo || ''}/${item.malzemeTalep?.satSiraNo || ''}`;
                const olusturmaTarihi = item.malzemeTalep?.satOlusturmaTarihi 
                    ? new Date(item.malzemeTalep.satOlusturmaTarihi).toLocaleDateString('tr-TR') + ' ' + 
                      new Date(item.malzemeTalep.satOlusturmaTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })
                    : '';
                
                excelData.push([
                    item.malzemeTalep?.satSeriNo || '',
                    item.malzemeTalep?.satSiraNo || '',
                    talepKodu,
                    item.malzemeTalep?.projeKodu || '',
                    item.malzemeTalep?.malzemeKodu || '',
                    item.malzemeTalep?.malzemeIsmi || '',
                    item.malzemeTalep?.malzemeOrijinalTalepEdilenMiktar || 0,
                    item.toplamSevkEdilenMiktar || 0,
                    item.kalanMiktar || 0,
                    olusturmaTarihi
                ]);
            });
        } else if (type === 'depo') {
            // Depo Hazır Edim için başlık satırı
            excelData.push([
                'Sevk ID',
                'Proje Kodu',
                'Malzeme Kodu',
                'Malzeme İsmi',
                'Talep Edilen Miktar',
                'Hazırlanabilecek Miktar',
                'Açıklama',
                'SAT Cari Hesap',
                'SAT Tarihi',
                'Talep Tarihi'
            ]);

            // Veri satırları
            data.forEach(item => {
                const satTarihi = item.malzemeTalep?.satOlusturmaTarihi
                    ? new Date(item.malzemeTalep.satOlusturmaTarihi).toLocaleDateString('tr-TR') + ' ' +
                      new Date(item.malzemeTalep.satOlusturmaTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })
                    : '';
                
                const talepTarihi = item.surecOlusturmaTarihi
                    ? new Date(item.surecOlusturmaTarihi).toLocaleDateString('tr-TR') + ' ' +
                      new Date(item.surecOlusturmaTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })
                    : '';

                excelData.push([
                    item.sevkID || '',
                    item.malzemeTalep?.projeKodu || '',
                    item.malzemeTalep?.malzemeKodu || '',
                    item.malzemeTalep?.malzemeIsmi || '',
                    item.talepEdilenMiktar || 0,
                    item.hazirlanabilecekMiktar || 0,
                    item.malzemeTalep?.aciklama || '',
                    item.malzemeTalep?.satCariHesap || '',
                    satTarihi,
                    talepTarihi
                ]);
            });
        } else if (type === 'uretim') {
            // Üretim Mal Kabul için başlık satırı
            excelData.push([
                'Sevk ID',
                'Hazır ID',
                'SAT Seri/Sıra',
                'Proje Kodu',
                'Malzeme Kodu',
                'Malzeme İsmi',
                'Miktar',
                'Teslim Tarihi'
            ]);

            // Veri satırları
            data.forEach(item => {
                const satSeriSira = `${item.malzemeTalep?.satSeriNo || ''}/${item.malzemeTalep?.satSiraNo || ''}`;
                const teslimTarihi = item.surecOlusturmaTarihi
                    ? new Date(item.surecOlusturmaTarihi).toLocaleDateString('tr-TR') + ' ' +
                      new Date(item.surecOlusturmaTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })
                    : '';

                excelData.push([
                    item.sevkID || '',
                    item.hazirId || '',
                    satSeriSira,
                    item.malzemeTalep?.projeKodu || '',
                    item.malzemeTalep?.malzemeKodu || '',
                    item.malzemeTalep?.malzemeIsmi || '',
                    item.islenenMiktar || 0,
                    teslimTarihi
                ]);
            });
        } else if (type === 'depo-kabul') {
            // Depo Kabul için başlık satırı
            excelData.push([
                'Sevk ID',
                'Onay ID',
                'SAT Seri/Sıra',
                'Proje Kodu',
                'Malzeme Kodu',
                'Malzeme İsmi',
                'Onaylanan Miktar',
                'Onay Tarihi'
            ]);

            // Veri satırları
            data.forEach(item => {
                const satSeriSira = `${item.malzemeTalep?.satSeriNo || ''}/${item.malzemeTalep?.satSiraNo || ''}`;
                const onayTarihi = item.surecOlusturmaTarihi
                    ? new Date(item.surecOlusturmaTarihi).toLocaleDateString('tr-TR') + ' ' +
                      new Date(item.surecOlusturmaTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })
                    : '';

                excelData.push([
                    item.sevkID || '',
                    item.onayId || '',
                    satSeriSira,
                    item.malzemeTalep?.projeKodu || '',
                    item.malzemeTalep?.malzemeKodu || '',
                    item.malzemeTalep?.malzemeIsmi || '',
                    item.islenenMiktar || 0,
                    onayTarihi
                ]);
            });
        } else if (type === 'imalat-ek') {
            // İmalat Ek Talepler için başlık satırı
            excelData.push([
                'Proje Kodu',
                'Malzeme Kodu',
                'Malzeme İsmi',
                'Kalan Miktar',
                'SAT Seri No',
                'SAT Sıra No',
                'Açıklama',
                'SAT Cari Hesap',
                'Kayıt Tarihi'
            ]);

            // Veri satırları
            data.forEach(item => {
                const kayitTarihi = item.malzemeTalep?.kayitTarihi
                    ? new Date(item.malzemeTalep.kayitTarihi).toLocaleDateString('tr-TR') + ' ' +
                      new Date(item.malzemeTalep.kayitTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })
                    : '';

                excelData.push([
                    item.malzemeTalep?.projeKodu || '',
                    item.malzemeTalep?.malzemeKodu || '',
                    item.malzemeTalep?.malzemeIsmi || '',
                    item.kalanMiktar || 0,
                    item.malzemeTalep?.buTalebiKarsilayanSATSeriNo || '',
                    item.malzemeTalep?.buTalebiKarsilayanSATSiraNo || '',
                    item.malzemeTalep?.aciklama || '',
                    item.malzemeTalep?.satCariHesap || '',
                    kayitTarihi
                ]);
            });
        } else {
            // Kalite Kontrol ve diğer tipler için başlık satırı
            excelData.push([
                'SAT Seri No',
                'SAT Sıra No',
                'Malzeme Talep Kodu',
                'Proje Kodu',
                'Malzeme Kodu',
                'Malzeme İsmi',
                'Talep Edilen Miktar',
                'Sevk Edilen Miktar',
                'Kalan Miktar',
                'Oluşturma Tarihi'
            ]);

            // Veri satırları
            data.forEach(item => {
                const talepKodu = `${item.malzemeTalep?.satSeriNo || ''}/${item.malzemeTalep?.satSiraNo || ''}`;
                const olusturmaTarihi = item.malzemeTalep?.satOlusturmaTarihi 
                    ? new Date(item.malzemeTalep.satOlusturmaTarihi).toLocaleDateString('tr-TR') + ' ' + 
                      new Date(item.malzemeTalep.satOlusturmaTarihi).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' })
                    : '';
                
                excelData.push([
                    item.malzemeTalep?.satSeriNo || '',
                    item.malzemeTalep?.satSiraNo || '',
                    talepKodu,
                    item.malzemeTalep?.projeKodu || '',
                    item.malzemeTalep?.malzemeKodu || '',
                    item.malzemeTalep?.malzemeIsmi || '',
                    item.malzemeTalep?.malzemeOrijinalTalepEdilenMiktar || 0,
                    item.toplamSevkEdilenMiktar || 0,
                    item.kalanMiktar || 0,
                    olusturmaTarihi
                ]);
            });
        }

        // Excel workbook oluştur
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet(excelData);

        // Sütun genişliklerini ayarla (tip bazlı)
        let colWidths;
        if (type === 'depo') {
            colWidths = [
                { wch: 15 },  // Sevk ID
                { wch: 15 },  // Proje Kodu
                { wch: 20 },  // Malzeme Kodu
                { wch: 40 },  // Malzeme İsmi
                { wch: 20 },  // Talep Edilen Miktar
                { wch: 22 },  // Hazırlanabilecek Miktar
                { wch: 30 },  // Açıklama
                { wch: 25 },  // SAT Cari Hesap
                { wch: 20 },  // SAT Tarihi
                { wch: 20 }   // Talep Tarihi
            ];
        } else if (type === 'uretim') {
            colWidths = [
                { wch: 15 },  // Sevk ID
                { wch: 15 },  // Hazır ID
                { wch: 20 },  // SAT Seri/Sıra
                { wch: 15 },  // Proje Kodu
                { wch: 20 },  // Malzeme Kodu
                { wch: 40 },  // Malzeme İsmi
                { wch: 15 },  // Miktar
                { wch: 20 }   // Oluşturma Tarihi
            ];
        } else if (type === 'depo-kabul') {
            colWidths = [
                { wch: 15 },  // Sevk ID
                { wch: 15 },  // Onay ID
                { wch: 20 },  // SAT Seri/Sıra
                { wch: 15 },  // Proje Kodu
                { wch: 20 },  // Malzeme Kodu
                { wch: 40 },  // Malzeme İsmi
                { wch: 18 },  // Onaylanan Miktar
                { wch: 20 }   // Oluşturma Tarihi
            ];
        } else if (type === 'imalat-ek') {
            colWidths = [
                { wch: 15 },  // Proje Kodu
                { wch: 20 },  // Malzeme Kodu
                { wch: 40 },  // Malzeme İsmi
                { wch: 15 },  // Kalan Miktar
                { wch: 15 },  // SAT Seri No
                { wch: 15 },  // SAT Sıra No
                { wch: 30 },  // Açıklama
                { wch: 25 },  // SAT Cari Hesap
                { wch: 20 }   // Kayıt Tarihi
            ];
        } else {
            // Malzeme ve Kalite Kontrol için
            colWidths = [
                { wch: 15 },  // SAT Seri No
                { wch: 15 },  // SAT Sıra No
                { wch: 20 },  // Malzeme Talep Kodu
                { wch: 15 },  // Proje Kodu
                { wch: 15 },  // Malzeme Kodu
                { wch: 40 },  // Malzeme İsmi
                { wch: 18 },  // Talep Edilen Miktar
                { wch: 18 },  // Sevk Edilen Miktar
                { wch: 15 },  // Kalan Miktar
                { wch: 20 }   // Oluşturma Tarihi
            ];
        }
        ws['!cols'] = colWidths;

        // Worksheet'i workbook'a ekle
        let sheetName;
        if (type === 'malzeme') {
            sheetName = 'Malzeme Talep Et';
        } else if (type === 'depo') {
            sheetName = 'Depo Hazır Edim';
        } else if (type === 'uretim') {
            sheetName = 'Üretim Mal Kabul';
        } else if (type === 'kalite') {
            sheetName = 'Kalite Kontrol';
        } else if (type === 'depo-kabul') {
            sheetName = 'Depo Kabul';
        } else if (type === 'imalat-ek') {
            sheetName = 'İmalat Ek Talepler';
        }
        XLSX.utils.book_append_sheet(wb, ws, sheetName);

        // Dosya adını oluştur (tarih ve saat ile)
        const now = new Date();
        const dateStr = now.toLocaleDateString('tr-TR').replace(/\./g, '-');
        const timeStr = now.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' }).replace(/:/g, '-');
        let fileName;
        if (type === 'malzeme') {
            fileName = `Malzeme_Talep_Et_${dateStr}_${timeStr}.xlsx`;
        } else if (type === 'depo') {
            fileName = `Depo_Hazir_Edim_${dateStr}_${timeStr}.xlsx`;
        } else if (type === 'uretim') {
            fileName = `Uretim_Mal_Kabul_${dateStr}_${timeStr}.xlsx`;
        } else if (type === 'kalite') {
            fileName = `Kalite_Kontrol_${dateStr}_${timeStr}.xlsx`;
        } else if (type === 'depo-kabul') {
            fileName = `Depo_Kabul_${dateStr}_${timeStr}.xlsx`;
        } else if (type === 'imalat-ek') {
            fileName = `Imalat_Ek_Talepler_${dateStr}_${timeStr}.xlsx`;
        }

        // Excel dosyasını indir
        XLSX.writeFile(wb, fileName);

        Swal.fire({
            icon: 'success',
            title: 'Başarılı',
            text: `${data.length} kayıt Excel'e aktarıldı!`,
            timer: 2000,
            showConfirmButton: false
        });

    } catch (error) {
        console.error('Excel export hatası:', error);
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Excel dosyası oluşturulurken bir hata oluştu!'
        });
    }
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
    } else if (type === 'depo') {
        // Depo Hazırlama tablosundan ilgili satırı bul
        const table = window.depoHazirlamaTable;
        const rowData = table.rows().data().toArray().find(row => row.malzemeTalep.malzemeTalebiEssizID === id);
        
        if (rowData) {
            const malzeme = rowData.malzemeTalep;
            const tarih = malzeme.satOlusturmaTarihi ? new Date(malzeme.satOlusturmaTarihi).toLocaleString('tr-TR') : '-';
            
            // Departman bilgisini al (rowData'nın içinde, malzemeTalep'in içinde değil)
            let departmanBilgisi = '-';
            if (rowData.departmanSevkiyatlari && rowData.departmanSevkiyatlari.length > 0) {
                const departmanId = rowData.departmanSevkiyatlari[0].departmanID;
                const departmanAdi = getDepartmanAdi(departmanId);
                departmanBilgisi = `${departmanAdi} (ID: ${departmanId})`;
            }
            
            $('#detayModalLabel').html(`<i class="fas fa-info-circle mr-2"></i>Depo Hazırlama Detayı`);
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
                        <p><strong>Departman:</strong> ${departmanBilgisi}</p>
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
    } else if (type === 'uretim') {
        // Üretim Mal Kabul tablosundan ilgili satırı bul
        const table = window.uretimMalKabulTable;
        const rowData = table.rows().data().toArray().find(row => row.malzemeTalep.malzemeTalebiEssizID === id);
        
        if (rowData) {
            const malzeme = rowData.malzemeTalep;
            const satTarihi = malzeme.satOlusturmaTarihi ? new Date(malzeme.satOlusturmaTarihi).toLocaleString('tr-TR') : '-';
            const teslimTarihi = rowData.surecOlusturmaTarihi ? new Date(rowData.surecOlusturmaTarihi).toLocaleString('tr-TR') : '-';
            
            // Departman bilgisini al
            let departmanBilgisi = '-';
            if (rowData.departmanSevkiyatlari && rowData.departmanSevkiyatlari.length > 0) {
                const departmanId = rowData.departmanSevkiyatlari[0].departmanID;
                const departmanAdi = getDepartmanAdi(departmanId);
                departmanBilgisi = `${departmanAdi} (ID: ${departmanId})`;
            }
            
            // İade edilmiş kayıt için ek bilgiler
            let iadeDetayHtml = '';
            if (rowData.paramTalepSurecStatuID === 7) {
                iadeDetayHtml = `
                    <div class="col-12 mt-3">
                        <div class="alert alert-warning" role="alert">
                            <h6 class="alert-heading"><i class="fas fa-exclamation-triangle mr-2"></i>Kalite Kontrolünden Geçememiş Kayıt</h6>
                            <hr>
                            <p class="mb-1"><strong>Bildirim Tipi:</strong> ${rowData.bildirimTipiTanimlama || '-'}</p>
                            <p class="mb-0"><strong>Notu:</strong> ${rowData.surecStatuGirilenNot || '-'}</p>
                        </div>
                    </div>
                `;
            }
            
            $('#detayModalLabel').html(`<i class="fas fa-info-circle mr-2"></i>Üretim Mal Kabul Detayı`);
            $('#detayModalIcerik').html(`
                <div class="row">
                    <div class="col-md-6">
                        <p><strong>Sevk ID:</strong> ${rowData.sevkID || '-'}</p>
                        <p><strong>Hazır ID:</strong> ${rowData.hazirId || '-'}</p>
                        <p><strong>SAT Seri No:</strong> ${malzeme.satSeriNo || '-'}</p>
                        <p><strong>SAT Sıra No:</strong> ${malzeme.satSiraNo || '-'}</p>
                        <p><strong>SAT Oluşturma Tarihi:</strong> ${satTarihi}</p>
                        <p><strong>Teslim Tarihi:</strong> ${teslimTarihi}</p>
                    </div>
                    <div class="col-md-6">
                        <p><strong>Proje Kodu:</strong> ${malzeme.projeKodu || '-'}</p>
                        <p><strong>Malzeme Kodu:</strong> ${malzeme.malzemeKodu || '-'}</p>
                        <p><strong>Malzeme Adı:</strong> ${malzeme.malzemeIsmi || '-'}</p>
                    </div>
                    <div class="col-12 mt-3">
                        <p><strong>Orijinal Miktar:</strong> ${malzeme.malzemeOrijinalTalepEdilenMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>İşlenen Miktar:</strong> ${rowData.islenenMiktar?.toLocaleString('tr-TR') || '0'}</p>
                        <p><strong>Talep Edilen Miktar:</strong> ${rowData.talepEdilenMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Sevk Edilen Miktar:</strong> ${rowData.toplamSevkEdilenMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Kalan Miktar:</strong> ${rowData.kalanMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Departman:</strong> ${departmanBilgisi}</p>
                    </div>
                    ${iadeDetayHtml}
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
    } else if (type === 'kalite') {
        // Kalite Kontrol tablosundan ilgili satırı bul
        const table = window.kaliteKontrolTable;
        const rowData = table.rows().data().toArray().find(row => row.malzemeTalep.malzemeTalebiEssizID === id);
        
        if (rowData) {
            const malzeme = rowData.malzemeTalep;
            const tarih = malzeme.satOlusturmaTarihi ? new Date(malzeme.satOlusturmaTarihi).toLocaleString('tr-TR') : '-';
            
            // Departman bilgisini al
            let departmanBilgisi = '-';
            if (rowData.departmanSevkiyatlari && rowData.departmanSevkiyatlari.length > 0) {
                const departmanId = rowData.departmanSevkiyatlari[0].departmanID;
                const departmanAdi = getDepartmanAdi(departmanId);
                departmanBilgisi = `${departmanAdi} (ID: ${departmanId})`;
            }
            
            $('#detayModalLabel').html(`<i class="fas fa-info-circle mr-2"></i>Kalite Kontrol Detayı`);
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
                        <p><strong>Departman:</strong> ${departmanBilgisi}</p>
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
    } else if (type === 'imalat-ek') {
        // İmalat Ek Talepler tablosundan ilgili satırı bul
        const table = window.imalatEkTaleplerTable;
        const rowData = table.rows().data().toArray().find(row => row.malzemeTalep.malzemeTalebiEssizID === id);

        if (rowData) {
            const malzeme = rowData.malzemeTalep;
            const kayitTarihi = malzeme.kayitTarihi ? new Date(malzeme.kayitTarihi).toLocaleString('tr-TR') : '-';

            // Departman bilgisini al
            let departmanBilgisi = '-';
            if (rowData.departmanSevkiyatlari && rowData.departmanSevkiyatlari.length > 0) {
                const departmanId = rowData.departmanSevkiyatlari[0].departmanID;
                const departmanAdi = getDepartmanAdi(departmanId);
                departmanBilgisi = `${departmanAdi} (ID: ${departmanId})`;
            }

            $('#detayModalLabel').html(`<i class="fas fa-info-circle mr-2"></i>İmalat Ek Talep Detayı`);
            $('#detayModalIcerik').html(`
                <div class="row">
                    <div class="col-md-6">
                        <p><strong>Proje Kodu:</strong> ${malzeme.projeKodu || '-'}</p>
                        <p><strong>Malzeme Kodu:</strong> ${malzeme.malzemeKodu || '-'}</p>
                        <p><strong>Malzeme Adı:</strong> ${malzeme.malzemeIsmi || '-'}</p>
                    </div>
                    <div class="col-md-6">
                        <p><strong>SAT Seri No:</strong> ${malzeme.buTalebiKarsilayanSATSeriNo || '-'}</p>
                        <p><strong>SAT Sıra No:</strong> ${malzeme.buTalebiKarsilayanSATSiraNo || '-'}</p>
                        <p><strong>Kayıt Tarihi:</strong> ${kayitTarihi}</p>
                    </div>
                    <div class="col-12 mt-3">
                        <p><strong>Kalan Miktar:</strong> ${rowData.kalanMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>SAT Cari Hesap:</strong> ${malzeme.satCariHesap || '-'}</p>
                        <p><strong>Açıklama:</strong> ${malzeme.aciklama || '-'}</p>
                        <p><strong>Departman:</strong> ${departmanBilgisi}</p>
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
    } else if (type === 'depo-kabul') {
        // Depo Kabul tablosundan ilgili satırı bul
        const table = window.depoKabulTable;
        const rowData = table.rows().data().toArray().find(row => row.malzemeTalep.malzemeTalebiEssizID === id);
        
        if (rowData) {
            const malzeme = rowData.malzemeTalep;
            const satTarihi = malzeme.satOlusturmaTarihi ? new Date(malzeme.satOlusturmaTarihi).toLocaleString('tr-TR') : '-';
            const onayTarihi = rowData.surecOlusturmaTarihi ? new Date(rowData.surecOlusturmaTarihi).toLocaleString('tr-TR') : '-';
            
            // Departman bilgisini al
            let departmanBilgisi = '-';
            if (rowData.departmanSevkiyatlari && rowData.departmanSevkiyatlari.length > 0) {
                const departmanId = rowData.departmanSevkiyatlari[0].departmanID;
                const departmanAdi = getDepartmanAdi(departmanId);
                departmanBilgisi = `${departmanAdi} (ID: ${departmanId})`;
            }
            
            $('#detayModalLabel').html(`<i class="fas fa-info-circle mr-2"></i>Depo Kabul Detayı`);
            $('#detayModalIcerik').html(`
                <div class="row">
                    <div class="col-md-6">
                        <p><strong>Sevk ID:</strong> ${rowData.sevkID || '-'}</p>
                        <p><strong>Onay ID:</strong> ${rowData.onayId || '-'}</p>
                        <p><strong>SAT Seri No:</strong> ${malzeme.satSeriNo || '-'}</p>
                        <p><strong>SAT Sıra No:</strong> ${malzeme.satSiraNo || '-'}</p>
                        <p><strong>SAT Oluşturma Tarihi:</strong> ${satTarihi}</p>
                        <p><strong>Onay Tarihi:</strong> ${onayTarihi}</p>
                    </div>
                    <div class="col-md-6">
                        <p><strong>Proje Kodu:</strong> ${malzeme.projeKodu || '-'}</p>
                        <p><strong>Malzeme Kodu:</strong> ${malzeme.malzemeKodu || '-'}</p>
                        <p><strong>Malzeme Adı:</strong> ${malzeme.malzemeIsmi || '-'}</p>
                    </div>
                    <div class="col-12 mt-3">
                        <p><strong>Orijinal Miktar:</strong> ${malzeme.malzemeOrijinalTalepEdilenMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Onaylanan Miktar:</strong> ${rowData.islenenMiktar?.toLocaleString('tr-TR') || '0'}</p>
                        <p><strong>Talep Edilen Miktar:</strong> ${rowData.talepEdilenMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Sevk Edilen Miktar:</strong> ${rowData.toplamSevkEdilenMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Kalan Miktar:</strong> ${rowData.kalanMiktar?.toLocaleString('tr-TR') || '-'}</p>
                        <p><strong>Departman:</strong> ${departmanBilgisi}</p>
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
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Geçersiz tip!'
        });
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
    let text;
    if (type === 'malzeme') {
        text = 'Bu talebi';
    } else if (type === 'depo') {
        text = 'Bu depo kaydını';
    } else if (type === 'uretim') {
        text = 'Bu üretim kaydını';
    } else if (type === 'kalite') {
        text = 'Bu kalite kontrol kaydını';
    }
    
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
            } else if (type === 'depo') {
                refreshDepoHazirlamaTable();
            } else if (type === 'uretim') {
                refreshUretimMalKabulTable();
            } else if (type === 'kalite') {
                refreshKaliteKontrolTable();
            }
        }
    });
}

/**
 * Son İşlemi Geri Al butonu işlemi
 */
function handleSonIslemGeriAl() {
    // Onay mesajı göster
    Swal.fire({
        title: 'Emin misiniz?',
        text: 'Son işlemi geri almak istediğinizden emin misiniz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#f39c12',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Geri Al',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/MalzemeTalepEtSonIslemGeriAl',
                type: 'POST',
                contentType: 'application/json',
                success: function (response) {
                    console.log('Son İşlem Geri Al Response:', response);
                    
                    if (response.isSuccess) {
                        const successMessage = response.value || 'Son işlem başarıyla geri alındı!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#28a745'
                        }).then(() => {
                            // Malzeme Talep tablosunu yenile
                            refreshMalzemeTalepTable();
                        });
                    } else {
                        let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                        
                        if (response.message && typeof response.message === 'string') {
                            errorMessage = response.message;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Son İşlem Geri Al API Hatası:', error);
                    
                    let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * Depo Hazırlama Son İşlemi Geri Al
 */
function handleDepoHazirlamaSonIslemGeriAl() {
    // Onay mesajı göster
    Swal.fire({
        title: 'Emin misiniz?',
        text: 'Son hazırlama işlemini geri almak istediğinizden emin misiniz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#f39c12',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Geri Al',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/DepoHazirlamaSonIslemGeriAl',
                type: 'POST',
                contentType: 'application/json',
                success: function (response) {
                    console.log('Depo Hazırlama Son İşlem Geri Al Response:', response);
                    
                    if (response.isSuccess) {
                        const successMessage = response.value || 'Son hazırlama işlemi başarıyla geri alındı!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#28a745'
                        }).then(() => {
                            // Depo Hazırlama tablosunu yenile
                            refreshDepoHazirlamaTable();
                        });
                    } else {
                        let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                        
                        if (response.message && typeof response.message === 'string') {
                            errorMessage = response.message;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Depo Hazırlama Geri Al API Hatası:', error);
                    
                    let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * Üretim Mal Kabul Son İşlemi Geri Al
 */
function handleUretimMalKabulSonIslemGeriAl() {
    // Onay mesajı göster
    Swal.fire({
        title: 'Emin misiniz?',
        text: 'Son mal kabul/iade işlemini geri almak istediğinizden emin misiniz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#f39c12',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Geri Al',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/UretimMalKabulSonIslemGeriAl',
                type: 'POST',
                contentType: 'application/json',
                success: function (response) {
                    console.log('Üretim Mal Kabul Son İşlem Geri Al Response:', response);
                    
                    if (response.isSuccess) {
                        const successMessage = response.value || 'Son mal kabul/iade işlemi başarıyla geri alındı!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#28a745'
                        }).then(() => {
                            // Üretim Mal Kabul tablosunu yenile
                            refreshUretimMalKabulTable();
                        });
                    } else {
                        let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                        
                        if (response.message && typeof response.message === 'string') {
                            errorMessage = response.message;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Üretim Mal Kabul Geri Al API Hatası:', error);
                    
                    let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * Kalite Kontrol Son İşlemi Geri Al
 */
function handleKaliteKontrolSonIslemGeriAl() {
    // Onay mesajı göster
    Swal.fire({
        title: 'Emin misiniz?',
        text: 'Son kalite onay/hasarlı işlemini geri almak istediğinizden emin misiniz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#f39c12',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Geri Al',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/KaliteKontrolSonIslemGeriAl',
                type: 'POST',
                contentType: 'application/json',
                success: function (response) {
                    console.log('Kalite Kontrol Son İşlem Geri Al Response:', response);
                    
                    if (response.isSuccess) {
                        const successMessage = response.value || 'Son kalite onay/hasarlı işlemi başarıyla geri alındı!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#28a745'
                        }).then(() => {
                            // Kalite Kontrol tablosunu yenile
                            refreshKaliteKontrolTable();
                        });
                    } else {
                        let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                        
                        if (response.message && typeof response.message === 'string') {
                            errorMessage = response.message;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Kalite Kontrol Geri Al API Hatası:', error);
                    
                    let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * Hazırlandı butonu işlemi
 */
function handleHazirlandi() {
    // Seçili checkbox'ları bul
    const selectedCheckboxes = $('.depo-row-checkbox:checked');

    if (selectedCheckboxes.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen en az bir kayıt seçiniz!'
        });
        return;
    }

    // Her seçili satır için hazırlanan miktarı topla
    const items = [];
    let hasError = false;
    let invalidMiktarCount = 0;

    selectedCheckboxes.each(function() {
        const malzemeTalepSurecTakipID = $(this).data('surecid');
        const row = $(this).closest('tr');
        const hazirlananMiktarInput = row.find('.hazirlanan-miktar-input');
        const hazirlananMiktar = parseInt(hazirlananMiktarInput.val()) || 0;
        const hazirlanabilecekMiktar = parseInt(hazirlananMiktarInput.data('max')) || 0;

        // Hazırlanan miktar kontrolü
        if (!hazirlananMiktar || hazirlananMiktar <= 0) {
            invalidMiktarCount++;
        } else if (hazirlananMiktar > hazirlanabilecekMiktar) {
            invalidMiktarCount++;
        }

        items.push({
            malzemeTalepSurecTakipID: malzemeTalepSurecTakipID,
            hazirlananMiktar: hazirlananMiktar
        });
    });

    // Geçersiz miktar varsa uyarı göster
    if (invalidMiktarCount > 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: `${invalidMiktarCount} kayıtta geçersiz veya hazırlanabilecek miktarı aşan değer bulunmaktadır. Lütfen kontrol ediniz!`
        });
        return;
    }

    // Request data hazırla
    const requestData = {
        items: items
    };

    // Onay mesajını oluştur
    const confirmMessage = selectedCheckboxes.length === 1
        ? 'Bu kaydı hazırlandı olarak işaretlemek istediğinizden emin misiniz?'
        : `${selectedCheckboxes.length} kaydı hazırlandı olarak işaretlemek istediğinizden emin misiniz?`;

    // Onay sor
    Swal.fire({
        title: 'Emin misiniz?',
        text: confirmMessage,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#ff5f00',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Hazırlandı',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API'ye POST isteği gönder
            $.ajax({
                url: '/panel/TopluMalzemeleriHazirla',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(requestData),
                success: function(response) {
                    console.log('TopluMalzemeleriHazirla Response:', response);

                    if (response.isSuccess) {
                        // Servisten gelen mesajı göster (value string olarak dönüyor)
                        const successMessage = response.value || 'Kayıtlar başarıyla hazırlandı olarak işaretlendi!';

                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#007bff'
                        }).then(() => {
                            // Checkbox'ları temizle
                            $('.depo-row-checkbox').prop('checked', false);
                            $('#selectAllDepoHazirlama').prop('checked', false);

                            // Tabloyu yenile
                            refreshDepoHazirlamaTable();
                        });
                    } else {
                        // Hata durumunda servisten gelen mesajı göster
                        let errorMessage = 'İşlem başarısız!';

                        if (response.value && typeof response.value === 'string') {
                            errorMessage = response.value;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }

                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function(error) {
                    console.error('Hazırlandı İşlemi - Error Response:', error);

                    let errorMessage = 'İşlem gerçekleştirilemedi!';

                    // Servisten string mesaj dönebilir
                    if (error.responseJSON) {
                        if (error.responseJSON.value && typeof error.responseJSON.value === 'string') {
                            errorMessage = error.responseJSON.value;
                        } else if (error.responseJSON.errors && error.responseJSON.errors.length > 0) {
                            errorMessage = error.responseJSON.errors.join(', ');
                        }
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
        }
    });
}

/**
 * Mal Kabul butonu işlemi
 */
function handleMalKabul() {
    // Seçili checkbox'ları bul
    const selectedCheckboxes = $('.uretim-row-checkbox:checked');
    
    if (selectedCheckboxes.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen en az bir kayıt seçiniz!'
        });
        return;
    }

    // Seçili Süreç ID'lerini topla
    const selectedSurecIds = [];
    selectedCheckboxes.each(function() {
        selectedSurecIds.push($(this).data('surecid'));
    });

    // Onay mesajını oluştur
    const confirmMessage = selectedCheckboxes.length === 1
        ? 'Bu kaydı mal kabul olarak işaretlemek istediğinizden emin misiniz?'
        : `${selectedCheckboxes.length} kaydı mal kabul olarak işaretlemek istediğinizden emin misiniz?`;

    // Onay sor
    Swal.fire({
        title: 'Emin misiniz?',
        text: confirmMessage,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#007bff',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Mal Kabul',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/TopluMalKabulEt',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ malzemeTalepSurecTakipIDler: selectedSurecIds }),
                success: function (response) {
                    console.log('TopluMalKabulEt Response:', response);
                    
                    if (response.isSuccess) {
                        // Servisten gelen mesajı göster
                        const successMessage = response.value || 'Kayıtlar başarıyla mal kabul olarak işaretlendi!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#007bff'
                        }).then(() => {
                            // Checkbox'ları temizle
                            $('.uretim-row-checkbox').prop('checked', false);
                            $('#selectAllUretimMalKabul').prop('checked', false);
                            
                            // Tabloyu yenile
                            refreshUretimMalKabulTable();
                        });
                    } else {
                        // Hata durumunda servisten gelen mesajı göster
                        let errorMessage = 'Mal kabul işlemi sırasında bir hata oluştu!';
                        
                        if (response.message && typeof response.message === 'string') {
                            errorMessage = response.message;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Mal Kabul API Hatası:', error);
                    
                    let errorMessage = 'Mal kabul işlemi sırasında bir hata oluştu!';
                    
                    if (xhr.responseJSON) {
                        if (xhr.responseJSON.message && typeof xhr.responseJSON.message === 'string') {
                            errorMessage = xhr.responseJSON.message;
                        } else if (xhr.responseJSON.errors && xhr.responseJSON.errors.length > 0) {
                            errorMessage = xhr.responseJSON.errors.join(', ');
                        }
                    } else if (xhr.responseText) {
                        try {
                            const errorData = JSON.parse(xhr.responseText);
                            if (errorData.message) {
                                errorMessage = errorData.message;
                            }
                        } catch (e) {
                            errorMessage = xhr.responseText;
                        }
                    }
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * İade Et butonu işlemi
 */
function handleIadeEt() {
    // Seçili checkbox'ları bul
    const selectedCheckboxes = $('.uretim-row-checkbox:checked');
    
    if (selectedCheckboxes.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen bir kayıt seçiniz!'
        });
        return;
    }

    // Sadece yeşil (onaylanmış) satırların iade edilebileceğini kontrol et
    let hasInvalidSelection = false;
    selectedCheckboxes.each(function() {
        const row = $(this).closest('tr');
        if (!row.hasClass('onaylandi-satir')) {
            hasInvalidSelection = true;
            return false; // break the loop
        }
    });

    if (hasInvalidSelection) {
        Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Sadece onaylanmış (yeşil) kayıtlar iade edilebilir!'
        });
        return;
    }

    // Birden fazla seçiliyse uyarı göster ve sadece ilkini kullan
    if (selectedCheckboxes.length > 1) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'İade işlemi için sadece bir kayıt seçilebilir. İlk seçili kayıt kullanılacaktır.',
            confirmButtonText: 'Tamam'
        }).then(() => {
            // Sadece ilk checkbox'ı seçili bırak, diğerlerini kaldır
            selectedCheckboxes.not(':first').prop('checked', false);
            $('#selectAllUretimMalKabul').prop('checked', false);
            
            // İlk kayıt için işleme devam et
            const selectedSurecId = selectedCheckboxes.first().data('surecid');
            
            // Modal'a Süreç ID'yi set et
            $('#iadeMalzemeTalepSurecTakipID').val(selectedSurecId);
            
            // Bildirim tipleri dropdown'ını yükle
            loadBildirimTipleri();
            
            // Formu temizle
            $('#iadeEtNot').val('');
            
            // Modal'ı aç
            $('#iadeEtModal').modal('show');
        });
        return;
    }

    // Seçili Süreç ID'yi al
    const selectedSurecId = selectedCheckboxes.first().data('surecid');
    
    // Modal'a Süreç ID'yi set et
    $('#iadeMalzemeTalepSurecTakipID').val(selectedSurecId);
    
    // Bildirim tipleri dropdown'ını yükle
    loadBildirimTipleri();
    
    // Formu temizle
    $('#iadeEtNot').val('');
    
    // Modal'ı aç
    $('#iadeEtModal').modal('show');
}

// Bildirim tiplerini yükle
function loadBildirimTipleri() {
    $.ajax({
        url: '/panel/SurecStatuleriBildirimTipleriList/4',
        method: 'GET',
        dataType: 'json',
        success: function(response) {
            const dropdown = $('#surecStatuBildirimTipiID');
            dropdown.empty();
            dropdown.append('<option value="">Seçiniz</option>');
            
            if (response.isSuccess && response.value && response.value.length > 0) {
                response.value.forEach(function(item) {
                    dropdown.append(`<option value="${item.tabloID}">${item.bildirimTipiTanimlama}</option>`);
                });
            }
            
            // Select2'yi yeniden initialize et
            if (dropdown.data('select2')) {
                dropdown.select2('destroy');
            }
            dropdown.select2({
                dropdownParent: $('#iadeEtModal')
            });
        },
        error: function(xhr, status, error) {
            console.error('Bildirim tipleri yüklenirken hata:', error);
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Bildirim tipleri yüklenirken hata oluştu!'
            });
        }
    });
}

// İade Et modal kaydet buton eventi
$(document).on('click', '#btnIadeKaydet', function() {
    const malzemeTalepSurecTakipID = $('#iadeMalzemeTalepSurecTakipID').val();
    const surecStatuBildirimTipiID = $('#surecStatuBildirimTipiID').val();
    const surecStatuGirilenNot = $('#iadeEtNot').val().trim();
    
    // Form validasyonu
    if (!surecStatuBildirimTipiID) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen bildirim tipini seçiniz!'
        });
        return;
    }
    
    if (!surecStatuGirilenNot) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen not giriniz!'
        });
        return;
    }
    
    // İade Et API çağrısı
    $.ajax({
        url: '/api/MalzemeTalepGenelBilgiler/MalzemeIadeEt',
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        data: JSON.stringify({
            malzemeTalepSurecTakipID: malzemeTalepSurecTakipID,
            surecStatuBildirimTipiID: surecStatuBildirimTipiID,
            surecStatuGirilenNot: surecStatuGirilenNot
        }),
        success: function(response) {
            $('#iadeEtModal').modal('hide');
            
            Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: 'Malzeme başarıyla iade edildi!',
                confirmButtonColor: '#007bff'
            }).then(() => {
                // Checkbox'ları temizle
                $('.uretim-row-checkbox').prop('checked', false);
                
                // Tabloyu yenile
                refreshUretimMalKabulTable();
            });
        },
        error: function(xhr, status, error) {
            console.error('İade işlemi sırasında hata:', error);
            
            let errorMessage = 'İade işlemi sırasında hata oluştu!';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            
            Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: errorMessage
            });
        }
    });
});

/**
 * Hasarlı butonu işlemi
 */
function handleHasarli() {
    // Seçili checkbox'ları bul
    const selectedCheckboxes = $('.kalite-row-checkbox:checked');
    
    if (selectedCheckboxes.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen bir kayıt seçiniz!'
        });
        return;
    }

    if (selectedCheckboxes.length > 1) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen sadece bir kayıt seçiniz!'
        });
        return;
    }

    // Seçili Süreç ID'yi al
    const selectedSurecId = selectedCheckboxes.first().data('surecid');
    
    // Modal'a Süreç ID'yi set et
    $('#hasarliMalzemeTalepSurecTakipID').val(selectedSurecId);
    
    // Formu temizle
    $('#hasarliNot').val('');
    
    // Modal'ı aç
    $('#hasarliModal').modal('show');
}

// Hasarlı işlemi için bildirim tipleri yükleme fonksiyonu kaldırıldı

// Hasarlı modal kaydet buton eventi
$(document).on('click', '#btnHasarliKaydet', function() {
    const malzemeTalepSurecTakipID = $('#hasarliMalzemeTalepSurecTakipID').val();
    const surecStatuGirilenNot = $('#hasarliNot').val().trim();
    
    // Form validasyonu
    if (!surecStatuGirilenNot) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen not giriniz!'
        });
        return;
    }
    
    // Hasarlı olarak işaretle API çağrısı
    $.ajax({
        url: '/api/MalzemeTalepGenelBilgiler/HasarliOlarakIsaretle',
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        data: JSON.stringify({
            malzemeTalepSurecTakipID: malzemeTalepSurecTakipID,
            surecStatuGirilenNot: surecStatuGirilenNot
        }),
        success: function(response) {
            $('#hasarliModal').modal('hide');
            
            if (response && response.isSuccess) {
                Swal.fire({
                    icon: 'success',
                    title: 'Başarılı',
                    text: 'Malzeme hasarlı olarak işaretlendi!',
                    confirmButtonColor: '#007bff'
                }).then(() => {
                    // Checkbox'ları temizle
                    $('.kalite-row-checkbox').prop('checked', false);
                    
                    // Tabloyu yenile
                    refreshKaliteKontrolTable();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Hata',
                    text: response?.message || 'Hasarlı işaretleme sırasında bir hata oluştu!',
                    confirmButtonColor: '#dc3545'
                });
            }
        },
        error: function(xhr, status, error) {
            console.error('Hasarlı işaretleme sırasında hata:', error);
            
            let errorMessage = 'Hasarlı işaretleme sırasında hata oluştu!';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
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
 * Kalite Mal Kabul butonu işlemi
 */
function handleKaliteMalKabul() {
    // Seçili checkbox'ları bul
    const selectedCheckboxes = $('.kalite-row-checkbox:checked');
    
    if (selectedCheckboxes.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen bir kayıt seçiniz!'
        });
        return;
    }

    if (selectedCheckboxes.length > 1) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen sadece bir kayıt seçiniz!'
        });
        return;
    }

    // Seçili Süreç ID'yi al
    const selectedSurecId = selectedCheckboxes.first().data('surecid');

    // Onay sor
    Swal.fire({
        title: 'Emin misiniz?',
        text: `Bu kaydı mal kabul olarak işaretlemek istediğinizden emin misiniz?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#007bff',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Mal Kabul',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: `/api/MalzemeTalepGenelBilgiler/MalKabulEt/${selectedSurecId}`,
                type: 'POST',
                contentType: 'application/json',
                success: function (response) {
                    if (response && response.isSuccess) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: 'Kayıt mal kabul olarak işaretlendi!',
                            confirmButtonColor: '#007bff'
                        }).then(() => {
                            // Checkbox'ları temizle
                            $('.kalite-row-checkbox').prop('checked', false);
                            
                            // Tabloyu yenile
                            refreshKaliteKontrolTable();
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: response?.message || 'Mal kabul işlemi sırasında bir hata oluştu!',
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Mal Kabul API Hatası:', error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: 'Mal kabul işlemi sırasında bir hata oluştu!',
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * Depo Kabul butonu işlemi
 */
function handleDepoKabul() {
    // Seçili checkbox'ları bul
    const selectedCheckboxes = $('.depo-kabul-row-checkbox:checked');
    
    if (selectedCheckboxes.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen en az bir kayıt seçiniz!'
        });
        return;
    }

    // Seçili Süreç ID'lerini topla
    const selectedSurecIds = [];
    selectedCheckboxes.each(function() {
        selectedSurecIds.push($(this).data('surecid'));
    });

    // Onay mesajını oluştur
    const confirmMessage = selectedCheckboxes.length === 1
        ? 'Bu kaydı kabul etmek istediğinizden emin misiniz?'
        : `${selectedCheckboxes.length} kaydı kabul etmek istediğinizden emin misiniz?`;

    // Onay sor
    Swal.fire({
        title: 'Emin misiniz?',
        text: confirmMessage,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Kabul Et',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/TopluDepoKabul',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ malzemeTalepSurecTakipIDler: selectedSurecIds }),
                success: function (response) {
                    console.log('TopluDepoKabul Response:', response);
                    
                    if (response.isSuccess) {
                        const successMessage = response.value || 'Kayıtlar başarıyla kabul edildi!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#28a745'
                        }).then(() => {
                            // Checkbox'ları temizle
                            $('.depo-kabul-row-checkbox').prop('checked', false);
                            $('#selectAllDepoKabul').prop('checked', false);
                            
                            // Tabloyu yenile
                            refreshDepoKabulTable();
                        });
                    } else {
                        let errorMessage = 'Kabul işlemi sırasında bir hata oluştu!';
                        
                        if (response.message && typeof response.message === 'string') {
                            errorMessage = response.message;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Depo Kabul API Hatası:', error);
                    
                    let errorMessage = 'Kabul işlemi sırasında bir hata oluştu!';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * Depo Red butonu işlemi
 */
function handleDepoRed() {
    // Seçili checkbox'ları bul
    const selectedCheckboxes = $('.depo-kabul-row-checkbox:checked');
    
    if (selectedCheckboxes.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen en az bir kayıt seçiniz!'
        });
        return;
    }

    // Seçili Süreç ID'lerini topla
    const selectedSurecIds = [];
    selectedCheckboxes.each(function() {
        selectedSurecIds.push($(this).data('surecid'));
    });

    // Onay mesajını oluştur
    const confirmMessage = selectedCheckboxes.length === 1
        ? 'Bu kaydı reddetmek istediğinizden emin misiniz?'
        : `${selectedCheckboxes.length} kaydı reddetmek istediğinizden emin misiniz?`;

    // Onay sor
    Swal.fire({
        title: 'Emin misiniz?',
        text: confirmMessage,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Reddet',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/TopluDepoRed',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ malzemeTalepSurecTakipIDler: selectedSurecIds }),
                success: function (response) {
                    console.log('TopluDepoRed Response:', response);
                    
                    if (response.isSuccess) {
                        const successMessage = response.value || 'Kayıtlar başarıyla reddedildi!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#28a745'
                        }).then(() => {
                            // Checkbox'ları temizle
                            $('.depo-kabul-row-checkbox').prop('checked', false);
                            $('#selectAllDepoKabul').prop('checked', false);
                            
                            // Tabloyu yenile
                            refreshDepoKabulTable();
                        });
                    } else {
                        let errorMessage = 'Red işlemi sırasında bir hata oluştu!';
                        
                        if (response.message && typeof response.message === 'string') {
                            errorMessage = response.message;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Depo Red API Hatası:', error);
                    
                    let errorMessage = 'Red işlemi sırasında bir hata oluştu!';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * Depo Karar Geri Al butonu işlemi
 */
function handleDepoKararGeriAl() {
    // Onay sor
    Swal.fire({
        title: 'Emin misiniz?',
        text: 'En son yaptığınız depo kabul/red işlemi geri alınacak. Devam etmek istiyor musunuz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ffc107',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Evet, Geri Al',
        cancelButtonText: 'İptal'
    }).then((result) => {
        if (result.isConfirmed) {
            // API çağrısı yap
            $.ajax({
                url: '/panel/DepoKararSonIslemGeriAl',
                type: 'POST',
                contentType: 'application/json',
                success: function (response) {
                    console.log('DepoKararSonIslemGeriAl Response:', response);
                    
                    if (response.isSuccess) {
                        const successMessage = response.value || 'İşlem başarıyla geri alındı!';
                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: successMessage,
                            confirmButtonColor: '#28a745'
                        }).then(() => {
                            // Tabloyu yenile
                            refreshDepoKabulTable();
                        });
                    } else {
                        let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                        
                        if (response.message && typeof response.message === 'string') {
                            errorMessage = response.message;
                        } else if (response.errors && response.errors.length > 0) {
                            errorMessage = response.errors.join(', ');
                        }
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: errorMessage,
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Depo Karar Geri Al API Hatası:', error);
                    
                    let errorMessage = 'Geri alma işlemi sırasında bir hata oluştu!';
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Hata',
                        text: errorMessage,
                        confirmButtonColor: '#dc3545'
                    });
                }
            });
        }
    });
}

/**
 * URL'den aktif tab'ı yükle
 */
function loadActiveTabFromUrl() {
    // URL'deki hash'i al
    let hash = window.location.hash;
    
    // Hash varsa ve geçerli bir tab ise
    if (hash) {
        // Tab'ın var olup olmadığını kontrol et
        const tabExists = $('a[href="' + hash + '"]').length > 0;
        
        if (tabExists) {
            // İlgili tab'ı aktif et
            $('a[href="' + hash + '"]').tab('show');
        } else {
            // Geçersiz hash ise varsayılan tab'ı aç
            $('a[href="#malzemeTalepTabContent"]').tab('show');
        }
    } else {
        // Hash yoksa varsayılan tab'ı aç (Malzeme Talep Et)
        $('a[href="#malzemeTalepTabContent"]').tab('show');
    }
}

/**
 * Browser'ın geri/ileri butonları için
 */
$(window).on('popstate', function() {
    loadActiveTabFromUrl();
});
