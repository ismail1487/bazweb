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

// İlk yükleme kontrolü
window.isInitialLoad = false;

$(document).ready(function () {
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
        "order": [[8, "desc"]], // Oluşturma tarihine göre sırala
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
        "order": [[9, "desc"]], // Oluşturma tarihine göre sırala
        "data": [], // Başlangıçta boş veri
        "columns": [
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    return `<input type="checkbox" class="depo-row-checkbox" data-id="${row.malzemeTalebiEssizID}" data-surecid="${row.malzemeTalepSurecTakipID}" />`;
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
                "data": "talepEdilenMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '-';
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
        "order": [[9, "desc"]], // Oluşturma tarihine göre sırala
        "data": [], // Başlangıçta boş veri
        "rowCallback": function(row, data) {
            // paramTalepSurecStatuID === 7 ise satırı kahverengi yap
            if (data.paramTalepSurecStatuID === 7) {
                $(row).addClass('iade-edilmis-satir');
                $(row).attr('style', 'background-color: #d4a574 !important; color: #5a3825 !important;');
                $(row).find('td').css({
                    'background-color': '#d4a574',
                    'color': '#5a3825'
                });
            } else {
                $(row).removeClass('iade-edilmis-satir');
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
                "data": "talepEdilenMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '-';
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
        "order": [[9, "desc"]], // Oluşturma tarihine göre sırala
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
                "data": "talepEdilenMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '-';
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
        "order": [[9, "desc"]], // Oluşturma tarihine göre sırala
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
                "data": "malzemeTalep.malzemeOrijinalTalepEdilenMiktar",
                "render": function (data, type, row) {
                    return data || '0';
                }
            },
            {
                "data": "talepEdilenMiktar",
                "render": function (data, type, row) {
                    return data ? data.toLocaleString('tr-TR') : '-';
                }
            },
            {
                "data": "toplamSevkEdilenMiktar",
                "render": function (data, type, row) {
                    return data || '0';
                }
            },
            {
                "data": "kalanMiktar",
                "render": function (data, type, row) {
                    return data || '0';
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
                        <button class="btn btn-sm btn-info btn-detay" data-id="${row.malzemeTalep.malzemeTalebiEssizID}" data-type="depo-kabul" title="Detay">
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
        handleTabChange(targetTab);
    });

    // Tekil checkbox'lar değiştiğinde - sadece 1 tane seçilebilir
    $(document).on('change', '.row-checkbox', function () {
        if ($(this).is(':checked')) {
            // Diğer tüm checkbox'ları kaldır
            $('.row-checkbox').not(this).prop('checked', false);
        }
    });

    // Depo checkbox'ları değiştiğinde - sadece 1 tane seçilebilir
    $(document).on('change', '.depo-row-checkbox', function () {
        if ($(this).is(':checked')) {
            // Diğer tüm checkbox'ları kaldır
            $('.depo-row-checkbox').not(this).prop('checked', false);
        }
    });

    // Üretim checkbox'ları değiştiğinde - sadece 1 tane seçilebilir
    $(document).on('change', '.uretim-row-checkbox', function () {
        if ($(this).is(':checked')) {
            // Diğer tüm checkbox'ları kaldır
            $('.uretim-row-checkbox').not(this).prop('checked', false);
        }
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
                });
                
                // Select2'yi güncelle ama change event'ini tetikleme
                select.trigger('change.select2');
                depoSelect.trigger('change.select2');
                uretimSelect.trigger('change.select2');
                kaliteSelect.trigger('change.select2');
                depoKabulSelect.trigger('change.select2');
                
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
    
    // Formu temizle
    $('#sevkEdilenMiktar').val('');
    $('#surecStatuGirilenNot').val('');
    
    // Modalı aç
    $('#talepEtModal').modal('show');
}

/**
 * Talep kaydet butonu
 */
$(document).on('click', '#btnTalepKaydet', function() {
    // Form validasyonu
    const sevkEdilenMiktar = parseInt($('#sevkEdilenMiktar').val());
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
    
    // Request data hazırla
    const requestData = {
        malzemeTalebiEssizID: malzemeTalebiEssizID,
        sevkEdilenMiktar: sevkEdilenMiktar,
        sevkTalepEdenKisiID: 1,
        malzemeSevkTalebiYapanDepartmanID: 1,
        malzemeSevkTalebiYapanKisiID: 1,
        surecStatuGirilenNot: surecStatuGirilenNot
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
                    confirmButtonColor: '#007bff'
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
    const projeKoduValue = parseInt($('#uretimProjeFilter').val()) || 0;
    
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
    const projeKoduValue = parseInt($('#kaliteProjeFilter').val()) || 0;
    
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
    const projeKoduValue = parseInt($('#depoKabulProjeFilter').val()) || 0;
    
    // Depo Kabul için sabit parametreler
    // talepSurecStatuIDs: [6] (Depo Kabul statüsü)
    // malzemeTalepEtGetir: false
    const requestData = {
        projeKodu: projeKoduValue === 0 ? 0 : projeKoduValue, // Tümü için 0, seçiliyse projeKodu gönder
        talepSurecStatuIDs: [6], // Depo Kabul için her zaman 6
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
        } else {
            // Depo Hazır Edim için başlık satırı
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

        // Sütun genişliklerini ayarla
        const colWidths = [
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
            const tarih = malzeme.satOlusturmaTarihi ? new Date(malzeme.satOlusturmaTarihi).toLocaleString('tr-TR') : '-';
            
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
 * Hazırlandı butonu işlemi
 */
function handleHazirlandi() {
    // Seçili checkbox'ları bul
    const selectedCheckboxes = $('.depo-row-checkbox:checked');
    
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
        text: `Bu kaydı hazırlandı olarak işaretlemek istediğinizden emin misiniz?`,
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
                url: `/panel/MalzemeleriHazirla/${selectedSurecId}`,
                type: 'POST',
                contentType: 'application/json',
                success: function(response) {
                    if (response.isSuccess) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Başarılı',
                            text: 'Kayıt hazırlandı olarak işaretlendi!',
                            confirmButtonColor: '#007bff'
                        }).then(() => {
                            // Checkbox'ları temizle
                            $('.depo-row-checkbox').prop('checked', false);
                            
                            // Tabloyu yenile
                            refreshDepoHazirlamaTable();
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Hata',
                            text: response.errors && response.errors.length > 0 
                                ? response.errors.join(', ') 
                                : 'İşlem başarısız!',
                            confirmButtonColor: '#dc3545'
                        });
                    }
                },
                error: function(error) {
                    console.error('Hazırlandı İşlemi - Error Response:', error);
                    
                    let errorMessage = 'İşlem gerçekleştirilemedi!';
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
                            $('.uretim-row-checkbox').prop('checked', false);
                            
                            // Tabloyu yenile
                            refreshUretimMalKabulTable();
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
    
    // Bildirim tipleri dropdown'ını yükle
    loadHasarliBildirimTipleri();
    
    // Formu temizle
    $('#hasarliNot').val('');
    
    // Modal'ı aç
    $('#hasarliModal').modal('show');
}

// Hasarlı işlemi için bildirim tiplerini yükle
function loadHasarliBildirimTipleri() {
    $.ajax({
        url: '/panel/SurecStatuleriBildirimTipleriList/4',
        method: 'GET',
        dataType: 'json',
        success: function(response) {
            console.log('Hasarlı Bildirim Tipleri Response:', response);
            
            const dropdown = $('#hasarliSurecStatuBildirimTipiID');
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
                dropdownParent: $('#hasarliModal')
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

// Hasarlı modal kaydet buton eventi
$(document).on('click', '#btnHasarliKaydet', function() {
    const malzemeTalepSurecTakipID = $('#hasarliMalzemeTalepSurecTakipID').val();
    const surecStatuBildirimTipiID = $('#hasarliSurecStatuBildirimTipiID').val();
    const surecStatuGirilenNot = $('#hasarliNot').val().trim();
    
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
            surecStatuBildirimTipiID: surecStatuBildirimTipiID,
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
