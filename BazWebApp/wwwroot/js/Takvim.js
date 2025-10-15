var TakvimEl = document.getElementById('Takvim');
var dataKanyakRezerve = [];
var successTakvim;
var HataCounter = 0;
var calendar;
var currentInfo = null;
var rezerveDivEklendi = false;
var idler = [];
var dataOfRezervasyon = [];
var pendingDrop = null; // drop ile gelen veri
var objectid = 0;
var pendingDragged = null;
var selectedKaynakId = null;
var GirisIzni = false;
var selectedGunTipi;
var VerialmaID;
const doluKapasiteModel = [];

var TakvimdeBulunanEventLer = [];


document.addEventListener('DOMContentLoaded', function () {
    VeriYukle();
    $(document).ready(function () {
        DragOutlineEkle();
        DragOutlineEkleAy();
        VeriGetirPopUp();
        $("#RezerveTanimAy").html(RezerveDivAy());
    });

    $(document).on('submit', '#PopupDivAy', function (e) {
        e.preventDefault();

        if (!pendingDrop || !pendingDrop.draggedId) {
            console.error("Hata: Sürüklenen event bilgisi bulunamadı!");
            modalKapatVeTemizleAy(this);
            return;
        }

        var startDateStr = pendingDrop.dateStr;
        var endDateStr = pendingDrop.dateStr;

        var startTimeStr = $('#SaatPopupAy').val() || "09:00";
        var endTimeStr = $('#SaatPopupENDAy').val() || "10:00";
        var color = $('#RenkPopupAy').val() || "#ff9933"; // Renk değerini al

        var startDateTimeStr = startDateStr + 'T' + startTimeStr;
        var endDateTimeStr = endDateStr + 'T' + endTimeStr;

        const eventBaslangicTarih = new Date(startDateTimeStr);
        const eventBitisTarih = new Date(endDateTimeStr);
        const eventKaynakId = pendingDrop.draggedId;

        if (isNaN(eventBaslangicTarih.getTime()) || isNaN(eventBitisTarih.getTime())) {
            Swal.fire('Geçersiz Değer', 'Geçersiz tarih veya saat değeri. Lütfen doğru formatta girin.', 'error');
            return;
        }

        // MOLA SAATİ KONTROLÜ
        if (molaSaatleriyleCakisiyorMu(eventBaslangicTarih, eventBitisTarih, eventKaynakId)) {
            Swal.fire('Mola Saati', 'Bu saat aralığında mola saati bulunuyor. Lütfen başka bir saat seçin.', 'warning');
            return;
        }

        // KAPASİTE KONTROLÜ
        if (!KapasiteKontrolu(eventBaslangicTarih, eventBitisTarih, eventKaynakId)) {
            Swal.fire('Kapasite Aşıldı', 'Bu saat aralığında en fazla kapasite kadar rezervasyon yapılabilir.', 'error');
            return;
        }

        var secilenKaynak = dataKanyakRezerve.find(x => x.tabloID == pendingDrop.draggedId);
        var gunTipi = secilenKaynak ? secilenKaynak.kaynakRezerveTanimVM.uygunGunTipleri : null;

        if (gunTipi === null) {
            console.error("Hata: Seçilen kaynağın gün tipi bulunamadı.");
            Swal.fire('Eksik Bilgi', 'Geçerli bir gün tipi seçiniz.', 'warning');
            modalKapatVeTemizleAy(this);
            return;
        }

        var baslangicTarihiDate = new Date(startDateStr);
        var baslangicGunu = baslangicTarihiDate.getDay();

        if (gunTipi === 1) { // Hafta İçi
            if (baslangicGunu === 0 || baslangicGunu === 6) {
                Swal.fire('Uygun Olmayan Gün', 'Hafta içi rezervasyonlar hafta içi başlamalıdır.', 'warning');
                return;
            }
        } else if (gunTipi === 2) { // Hafta Sonu
            if (baslangicGunu !== 0 && baslangicGunu !== 6) {
                Swal.fire('Uygun Olmayan Gün', 'Hafta sonu rezervasyonlar hafta sonu başlamalıdır.', 'warning');
                return;
            }
        }

        var model = {
            tanim: $("#TanimPopupAy").val().trim() || pendingDrop.draggedTitle || "Yeni Event",
            aciklama: $("#AciklamaPopupAy").val().trim(),
            saat: startTimeStr,
            saatbitis: endTimeStr,
            renk: color // Renk bilgisini modele ekle
        };

        // TAKVİME EVENT EKLEME
        // Renk parametresini ekleyerek fonksiyonu çağır
        takvimeEventEkle(model.tanim, model.aciklama, startDateStr, model.saat, model.saatbitis, pendingDrop.draggedId, endDateStr, model.renk);

        modalKapatVeTemizleAy(this);
    });
});

function sadeceSeciliMainIdSaatGorunumundeGoster() {
    if (!calendar || calendar.view?.type !== 'timeGridDay') return;

    var selected = $('#kaynakList').val();
    calendar.getEvents().forEach(ev => {
        const evMainId = ev.extendedProps?.mainId;
        const match = !selected || selected === '0' || String(evMainId) === String(selected);
        ev.setProp('display', match ? 'auto' : 'none');
    });
}

function tumEventleriGorunurYap() {
    if (!calendar) return;
    calendar.getEvents().forEach(ev => ev.setProp('display', 'auto'));
}

function VeriYukle() {
    $.ajax({
        url: '/KaynakRezerve/KaynakRezerveTakvimVeriGetir',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data.isSuccess == true) {
                dataKanyakRezerve = data.value;
                console.log(dataKanyakRezerve);
                TakvimYukle();
                calendar.render();
                EventleriListele();

            }
            else {
                console.error("Hata oluştu : Hata nedeni:, data.IsSuccess: " + data.isSuccess);
                Swal.fire('Yükleme Hatası', 'Veriler yüklenemedi. Lütfen daha sonra tekrar deneyin.', 'error');
                HataCounter++;
            }
        },
        error: function (xhr, status, error) {
            console.error('Veri yükleme hatası:', error, xhr);
            Swal.fire('Yükleme Hatası', 'Veriler yüklenemedi. Lütfen daha sonra tekrar deneyin.', 'error');
        }
    });
}

function TakvimYukle() {
    try {
        calendar = new FullCalendar.Calendar(TakvimEl, {
            initialView: 'dayGridMonth',
            locale: 'tr',
            timeZone: 'local',
            height: 'auto',
            themeSystem: 'bootstrap4',
            droppable: true,
            selectable: true,
            buttonText: {
                today: 'Bugün',
                month: 'Ay-Hafta',
                day: 'Gün-Saat',
            },

            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridDay'
            },
            datesSet: function (info) {

                if (info.view.type === 'timeGridDay' || info.view.type === 'timeGridWeek') {
                    document.querySelector('.fc-prev-button').style.display = 'none';
                    document.querySelector('.fc-next-button').style.display = 'none';
                } else {
                    document.querySelector('.fc-prev-button').style.display = '';
                    document.querySelector('.fc-next-button').style.display = '';
                }
                if (currentInfo?.view.type !== info.view.type) {
                    currentInfo = info;

                    if (info.view.type === 'timeGridDay') {
                        $("#RezerveTanimAy").hide();
                        $("#RezerveTanim").show().html(RezerveDiv());
                        DivIdKaynakYolla(DivIdTanimGetir(), 'timeGridDay');
                        setTimeout(sadeceSeciliMainIdSaatGorunumundeGoster, 0);
                    } else if (info.view.type === 'dayGridMonth') {
                        $("#RezerveTanim").hide();
                        $("#RezerveTanimAy").show().html(RezerveDivAy());
                        DivIdKaynakYolla(DivIdTanimGetir(), 'dayGridMonth');
                        FromDragEkleAy();
                        tumEventleriGorunurYap();
                    }
                }
            },
            drop: function (info) {
                var dropmu = true;
                const bugun = new Date();
                bugun.setHours(0, 0, 0, 0);

                if (info.date < bugun) {
                    console.warn("Geçmiş tarihlere event eklenemez.");
                    Swal.fire('Geçersiz Tarih', 'Geçmiş tarihlere rezervasyon ekleyemezsiniz.', 'warning');
                    return;
                }

                var viewType = calendar.view.type;
                var dateStr = info.dateStr || (info.date ? info.date.toISOString().slice(0, 10) : null);
                var draggedTitle = (info.draggedEl && info.draggedEl.dataset) ? info.draggedEl.dataset.title : '';
                var draggedId = (info.draggedEl && info.draggedEl.dataset) ? info.draggedEl.dataset.id : '';

                pendingDrop = {
                    dateStr: dateStr,
                    draggedTitle: draggedTitle,
                    draggedId: draggedId
                };
                if (!KapasiteKontrolu(info.date, info.date, pendingDrop.draggedId, null, true)) {
                    Swal.fire('Kapasite aşıldı', 'Bu saat aralığında en fazla kapasite kadar rezervasyon yapılabilir.', 'error');
                    return; // işlemi durdur
                }
                if (viewType === 'dayGridMonth') {
                    $("#TanimPopupAy").val(draggedTitle || "");
                    $("#AciklamaPopupAy").val("");
                    $("#SaatPopupAy").val("09:00");
                    $("#SaatPopupENDAy").val("10:00");
                    $('#popupModalAy').modal('show');
                } else if (viewType == 'timeGridDay') {
                    $("#TanimPopup").val(draggedTitle || "");
                    $("#AciklamaPopup").val("");
                    $("#SaatPopup").val("09:00");
                    $("#SaatPopupEND").val("10:00");
                    $('#popupModal').modal('show');
                } else {
                    console.warn("Beklenmeyen görünüm türü:", viewType);
                }

            },
            eventReceive: function (info) {
                info.event.remove();
            },
            navLinks: true,
            dateClick: function (info) {
                calendar.changeView('timeGridDay', info.dateStr);
            },
            selectAllow: function (selectInfo) {
                var viewType = calendar.view.type;
                if (viewType == 'timeGridDay') {
                    return SaatDenetimSelect(selectInfo);
                }
                return true;
            },
            eventAllow: function (dropInfo, draggedEvent) {
                var viewType = calendar.view.type;

                if (!draggedEvent) {
                    console.error("Sürüklenen event bilgisi boş geldi.");
                    return false;
                }

                pendingDragged = {
                    draggedEvent: draggedEvent
                };

                if (viewType == 'timeGridDay') {
                    return SaatDenetim(dropInfo, draggedEvent);
                }
                else if (viewType == 'dayGridMonth') {
                    return GunTipiDenetim(dropInfo);
                }
                return true;
            },
            eventContent: function (arg) {
                var viewType = calendar.view.type;
                let eventHtml = `
                <div style="padding:2px;">
                <span style="font-weight: bold; color: darkgray;" id="${arg.event.extendedProps.mainId}">Tanim: ${arg.event.title}</span><br>
                <span>${arg.event.title}</span>
                </div>`;
                if (viewType === 'timeGridDay') { return { html: eventHtml }; }
                if (viewType === 'dayGridMonth') {
                    let html = `<div>`;
                    html += `<span style="color: blue;">⌂ ${arg.event.title}</span>`
                    return { html: html };
                }
                ;
            },
            eventDidMount: function (info) {

            },
            eventMouseEnter: function (arg) {

                if (arg.event.extendedProps.aciklama) {
                    const tarihSaat = arg.event.start.toLocaleDateString('tr-TR', {
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric',
                        hour: '2-digit',
                        minute: '2-digit'
                    });
                    // Tooltip'i oluştur ve göster
                    const tooltip = document.createElement('div');
                    tooltip.className = 'custom-tooltip';
                    tooltip.textContent = `Başlangıç: ${tarihSaat}\n Açıklama: ${arg.event.extendedProps.aciklama}`;
                    document.body.appendChild(tooltip);

                    // Tooltip'i fare imlecinin konumuna göre yerleştir
                    arg.el.addEventListener('mousemove', (e) => {
                        tooltip.style.left = (e.pageX + 10) + 'px';
                        tooltip.style.top = (e.pageY + 10) + 'px';
                    });
                }
            },
            eventMouseLeave: function () {
                // Fare event'in üzerinden ayrıldığında tooltip'i kaldır
                const tooltip = document.querySelector('.custom-tooltip');
                if (tooltip) {
                    tooltip.remove();
                }
            },
            eventClick: function (info) {
                var TabloID = info.event.extendedProps.tabloId;
                console.log(TabloID);
                EventVeriGetir(TabloID);

            },
            // ...
            dayCellDidMount: function (info) {
                const bugun = new Date();
                bugun.setHours(0, 0, 0, 0);

                const gun = info.date.getDay();

                if (gun >= 1 && gun <= 5) {
                    info.el.style.backgroundColor = "#e6f2ff";
                    info.el.style.border = "1px solid #a3c2f0";
                }
                else if (gun === 0 || gun === 6) {
                    info.el.style.backgroundColor = "#ffe6e6";
                    info.el.style.border = "1px solid #f5a3a3";
                }

                if (info.date < bugun) {
                    info.el.classList.add('gecmis-gun');
                }
                else if (formatTarih(info.date) === formatTarih(new Date())) {
                    info.el.style.backgroundColor = "#ccffcc";
                    info.el.style.fontWeight = "bold";
                }


                if (info.date >= bugun) {
                    dataKanyakRezerve.forEach(item => {
                        const GunTipleri = item.kaynakRezerveTanimVM.uygunGunTipleri;
                        let uygun = false;
                        let GirisIzni = false;

                        if (GunTipleri == 1 && gun >= 1 && gun <= 5) { uygun = true; GirisIzni = true; }
                        else if (GunTipleri == 2 && (gun === 0 || gun === 6)) { uygun = true; GirisIzni = true; }
                        else if (GunTipleri == 3) { uygun = true; GirisIzni = true; }

                        if (uygun) {
                            const div = document.createElement('div');
                            div.id = `tablo-${item.tabloID}`;
                            div.className = "DivTanimlari";
                            div.textContent = item.kaynakTanimVM.kaynakAdi;

                            const contentEl = info.el.querySelector('.fc-daygrid-day-frame');
                            if (contentEl) {
                                contentEl.appendChild(div);
                            } else {
                                info.el.appendChild(div);
                            }
                        }
                        else { console.log(uygun); }
                    });
                }

            },
            // --- yeni ekleme ---
            eventsSet: function (events) {
                // Eventler yüklendikten sonra kapasiteyi kontrol et
                document.querySelectorAll(".fc-daygrid-day").forEach(cell => {
                    let dateStr = cell.getAttribute("data-date");
                    let date = new Date(dateStr);

                    dataKanyakRezerve.forEach(item => {
                        const isFull = SayfadaKapasiteKontrolEt(item.tabloID, date);
                        if (!isFull) {
                            // ikon ekle (sadece kapasite dolu ise)
                            let div = cell.querySelector(`#tablo-${item.tabloID}`);
                            if (div && !div.querySelector(`#icon-${item.tabloID}`)) {
                                const imgElement = document.createElement('img');
                                imgElement.id = 'icon-' + item.tabloID;
                                imgElement.classList = 'Iconimg';
                                imgElement.src = '/images/kapasiteIcon16x16.png';
                                imgElement.alt = 'Kapasite Dolu';
                                imgElement.style.width = '20px';
                                imgElement.style.height = '20px';
                                imgElement.style.float = 'right';
                                imgElement.style.marginRight = '5px';
                                div.appendChild(imgElement);
                            }
                        }
                    });
                });
            }

        });
        successTakvim = true;
        calendar.render();
    }
    catch (e) {
        successTakvim = false;
        console.log("Takvim Yüklenmedi isSuccess: false", e); HataCounter++;
    }
}
var dateList = []
function DatesToList(info) {
    dateList.push(info.date);
    return dateList;
}

function GunTipiDenetim(dropInfo) {
    // Kaynak seçili mi kontrol et
    if (!selectedKaynakId || selectedGunTipi === null) {
        console.warn("Lütfen önce bir kaynak seçin.");
        return false;
    }

    // Drop yapılan günün numarasını al (0 = Pazar, 6 = Cumartesi)
    var dropDay = dropInfo.start.getDay();
    var isHaftaIci = dropDay >= 1 && dropDay <= 5;
    var isHaftaSonu = dropDay === 0 || dropDay === 6;

    // Gün tipine göre kontrol
    if (selectedGunTipi == 1 && isHaftaIci) { // Sadece hafta içi
        return true;
    }
    if (selectedGunTipi == 2 && isHaftaSonu) { // Sadece hafta sonu
        return true;
    }
    if (selectedGunTipi == 3) { // Tüm hafta
        return true;
    }

    // Yukarıdaki koşullardan hiçbiri sağlanmazsa, izin verme
    console.warn("Bu kaynak sadece belirlenen gün tiplerinde rezerve edilebilir.");
    return false;
}

function renkliSaatAlanlari() {
    var KaynakListID = $("#kaynakList").val();
    var kaynak = dataKanyakRezerve.find(x => x.tabloID == parseInt(KaynakListID));
    if (!kaynak) return;

    var baslangicSaat = kaynak.kaynakRezerveTanimVM.rezerveSaatBaslangicDegeri || "00:00";
    var bitisSaat = kaynak.kaynakRezerveTanimVM.rezerveSaatBitisDegeri || "23:59";

    // Takvimde saat slotlarını seç
    $(".fc-timegrid-slot").each(function () {
        var slotText = $(this).find(".fc-timegrid-slot-label-cushion").text(); // örn "08:00"

        if (!saatAraliginda(slotText, baslangicSaat, bitisSaat)) {
            $(this).css("background-color", "#f8d7da"); // Açık kırmızı, izin dışı saatler
        } else {
            $(this).css("background-color", ""); // izin verilen saatler temizle
        }
    });
}

function saatAraliginda(saat, baslangic, bitis) {
    // Saatleri dakika cinsinden hesapla
    function dakika(s) {
        var parts = s.split(':');
        return parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10);
    }
    var s = dakika(saat);
    var b = dakika(baslangic);
    var e = dakika(bitis);
    return s >= b && s <= e;
}

function formatTarih(date) {
    return date.getFullYear() + '-' +
        String(date.getMonth() + 1).padStart(2, '0') + '-' +
        String(date.getDate()).padStart(2, '0');
}

function RezerveDiv() {
    var rezervediv = ` 
<div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <label class="Lbl">Kaynak Adı:</label>
                        <select id="kaynakList" class="form-control" style="width: 100%;" data-placeholder="@localizator["kaynak Seçiniz"].Value" required>
                        <option value="0">Tüm Kaynaklar</option>
                        </select>
                        <label for="kaynakList" id="kaynakListHata" class="validationMessage">Kaynak Seçiniz</label>
                    </div>
                </div>
            </div>
            <div class="row ">
                <div class="col-md-12">
                    <div class="form-group">
                        <label class="Lbl">Kaynak Tanımı:</label>
                        <label style="font-size: 20px;" id="KaynakAdi">Adtest</label>
                    </div>
                </div>
            </div>
                      <div class="row">
                    <div class="col-md-12">
                        <div id="rightPanel">
                            <label class="Lbl">Çalışma Saatleri:</label><br />
                            <label style="font-size: 20px;" id="eventCalismaSaat"></label>
                        </div>
                    </div>
                </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <label class="Lbl">Açıklama:</label><br />
                        <label style="font-size: 20px;" id="kaynakAciklama">AçıklamTest</label>
                    </div>
                </div>
            </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div id="rightPanel">
                        <div id="eventFormContainer" style="display: none;"></div>
                    </div>
                </div>
            </div>

            <div class="conatinerOfRezervasyon" style="display: none;">
                <div class="row">
                    <div class="col-md-12">
                        <div id="rightPanel">
                                            <label class="Lbl">Rezervasyon Başlık:</label><br />
                            <div id="eventTitle"></div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div id="rightPanel">
                                            <label class="Lbl">Rezervasyon Açıklama:</label><br />
                            <div id="eventAciklama"></div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div id="rightPanel">
                                            <label class="Lbl">Rezervasyon Saati:</label><br />
                            <div id="eventStartEnd"></div>
                        </div>
                    </div>
                </div>

            </div>
`;

   return rezervediv;
}
function RezerveDivAy() {
    var RezerveDiv = ` 
            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <label class="Lbl">Kaynak Adı</label>
                        <select id="kaynakListAy" class="form-control" style="width: 100%;" data-placeholder="@localizator["kaynak Seçiniz"].Value" required>
                        <option value="0">Tüm Kaynaklar</option>
                        </select>
                        <label for="kaynakList" id="kaynakListHata" class="validationMessage">Kaynak Seçiniz</label>
                    </div>
                </div>
            </div>
            <div class="row ">
                <div class="col-md-12">
                    <div class="form-group">
                        <label class="Lbl">Kaynak Tanımı:</label>
                        <label style="font-size: 20px;" id="KaynakAdiAy">Adtest</label>
                    </div>
                </div>
            </div>
            <div class="row">
                    <div class="col-md-12">
                        <div id="rightPanel">
                            <label class="Lbl">Çalışma Saatleri:</label><br />
                            <label style="font-size: 20px;" id="eventCalismaSaatAy"></label>
                        </div>
                    </div>
                </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <label class="Lbl">Açıklama:</label><br />
                        <label style="font-size: 20px;" id="kaynakAciklamaAy">AçıklamTest</label>
                    </div>
                </div>
            </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div id="rightPanel">
                        <div id="eventFormContainerAy" style="display: none;"></div>
                    </div>
                </div>
            </div>
            
            <div class="conatinerOfRezervasyonAy" style="display: none;">
                <div class="row">
                    <div class="col-md-12">
                        <div id="rightPanel">
                                            <label class="Lbl">Rezervasyon Başlık:</label><br />
                            <div id="eventTitleAy"></div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div id="rightPanel">
                                            <label class="Lbl">Rezervasyon Açıklama:</label><br />
                            <div id="eventAciklamaAy"></div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div id="rightPanel">
                                            <label class="Lbl">Rezervasyon Saati:</label><br />
                            <div id="eventStartEndAy"></div>
                        </div>
                    </div>
                </div>

            </div>
`;

    return RezerveDiv;
}
function DragOutlineEkle() {
    var popupDiv = `<div class="modal fade" id="popupModal" tabindex="-1" aria-labelledby="popupModalLabel" aria-hidden="true">
<div class="modal-dialog">
<div class="modal-content">
    <div class="modal-header">
    <h5 class="modal-title" id="popupModalLabel">Bilgilerini Gir</h5>
    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
    </div>
    <div class="modal-body">
    <form id="PopupDiv">
        <div class="mb-3">
        <label for="TanimPopup" class="form-label">Başlık</label>
        <input type="text" class="form-control" id="TanimPopup" required>
        </div>
        <div class="mb-3">
        <label for="AciklamaPopup" class="form-label">Açıklama</label>
        <input type="text" class="form-control" id="AciklamaPopup">
        </div>
        <div class="mb-3" style="display: none;">
        <label for="SaatPopup" class="form-label">Saat/Başlangıç (HH:MM)</label>
        <input type="time" class="form-control" id="SaatPopup" value="09:00" required>
        </div>
        <div class="mb-3">
        <label for="SaatPopupEND" class="form-label">Saat/Bitiş (HH:MM)</label>
        <input type="time" class="form-control" id="SaatPopupEND" value="09:00" required>
        </div>
        <button type="submit" class="btn btn-success">Kaydet</button>
    </form>
    </div>
</div>
</div>
</div>`;

    $("body").append(popupDiv);
}

function DragOutlineEkleAy() {
    var popupDiv = `<div class="modal fade" id="popupModalAy" tabindex="-1" aria-labelledby="popupModalLabel" aria-hidden="true">
<div class="modal-dialog">
<div class="modal-content">
    <div class="modal-header">
    <h5 class="modal-title" id="popupModalLabel">Bilgilerini Gir</h5>
    </div>
    <div class="modal-body">
    <form id="PopupDivAy">
        <div class="mb-3">
        <label for="TanimPopupAy" class="form-label">Başlık</label>
        <input type="text" class="form-control" id="TanimPopupAy" required>
        </div>
        <div class="mb-3">
        <label for="AciklamaPopupAy" class="form-label">Açıklama</label>
        <input type="text" class="form-control" id="AciklamaPopupAy">
        </div>
        <div class="mb-3">
        <label for="SaatPopupAy" class="form-label">Saat/Başlangıç (HH:MM)</label>
        <input type="time" class="form-control" id="SaatPopupAy" value="09:00" required>
        </div>
        <div class="mb-3">
        <label for="SaatPopupENDAy" class="form-label">Saat/Bitiş (HH:MM)</label>
        <input type="time" class="form-control" id="SaatPopupENDAy" value="09:00" required>
        </div>
        <button type="submit" class="btn btn-success">Kaydet</button>
    </form>
    </div>
</div>
</div>
</div>`;

    $("body").append(popupDiv);
}
function RezerveEkle() {
    $("#RezerveTanim").show().html(RezerveDiv());


}
function RezerveEkleAy() {

    $("#RezerveTanimAy").show().html(RezerveDivAy());

}

function DivIdTanimGetir() {
    idler = [];
    $(".DivTanimlari").each(function () {
        idler.push(this.id); // this daha hızlı çalışır
    });
    idler = idler.filter((value, index, self) => self.indexOf(value) === index);
    console.log(idler);
    return idler;
}
var SelectidSecici = 0;
var check;
var selectList = [];
function DivIdKaynakYolla(idList, viewType) {
    var ids = idList.map(x => parseInt(x.replace("tablo-", "")));
    console.log(viewType)
    if (viewType === 'dayGridMonth') {

        $.ajax({
            url: '/KaynakRezerve/KaynakSelectVeriGetir',
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(ids),
            success: function (result) {
                console.log(result.value);
                var select = $("#kaynakListAy");
                var KaynakAdi = $("#KaynakAdiAy");
                var KaynakAciklama = $("#kaynakAciklamaAy");
                select.empty();
                selectList = result.value;
                console.log(selectList);
                $.each(ids, function (index, item) {
                    var filteredItem = selectList.find(x => x.tabloID == item);
                    if (filteredItem) {
                        var option = $('<option></option>')
                            .val(filteredItem.tabloID)
                            .text(filteredItem.kaynakTanimVM.kaynakAdi)
                            .attr("id", SelectidSecici);
                        select.append(option);
                        SelectidSecici++;
                    }
                });
                select.off('change');
                select.on('change', VeriDoldurSelectDiv);

                VeriDoldurSelectDiv();
            },
            error: function (xhr, err) {
                console.error("Hata:", xhr);
            }
        });
    }
    else {
        $.ajax({
            url: '/KaynakRezerve/KaynakSelectVeriGetir',
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify(ids),
            success: function (result) {
                console.log(result.value);
                var select = $("#kaynakList");
                var KaynakAdi = $("#KaynakAdi");
                var KaynakAciklama = $("#kaynakAciklama");
                select.empty();
                selectList = result.value;
                console.log(selectList);
                $.each(ids, function (index, item) {
                    var filteredItem = selectList.find(x => x.tabloID == item);
                    if (filteredItem) {
                        var option = $('<option></option>')
                            .val(filteredItem.tabloID)
                            .text(filteredItem.kaynakTanimVM.kaynakAdi)
                            .attr("id", SelectidSecici);
                        select.append(option);
                        SelectidSecici++;
                    }
                });
                select.off('change');
                select.on('change', VeriDoldurSelectDiv);

                VeriDoldurSelectDiv();
            },
            error: function (xhr, err) {
                console.error("Hata:", xhr);
            }
        });
    }
}

function VeriDoldurSelectDiv() {
    var bugun = new Date();
    var yil = bugun.getFullYear();
    var ay = String(bugun.getMonth() + 1).padStart(2, '0');
    var gun = String(bugun.getDate()).padStart(2, '0');
    var formatliTarih = `${yil}-${ay}-${gun}`;
    var viewType = calendar.view.type;
    var select, KaynakAdi, KaynakAciklama, newEventDiv, saat, mola;

    if (viewType === 'dayGridMonth') {
        select = $("#kaynakListAy");
        KaynakAdi = $("#KaynakAdiAy");
        KaynakAciklama = $("#kaynakAciklamaAy");
        saat = $("#eventCalismaSaatAy");
        newEventDiv = $("#RezerveTanimAy #NewEvent");
    } else { // timeGridDay
        select = $("#kaynakList");
        KaynakAdi = $("#KaynakAdi");
        KaynakAciklama = $("#kaynakAciklama");
        saat = $("#eventCalismaSaat");
        newEventDiv = $("#RezerveTanim #NewEvent");
    }

    var selectedID = parseInt(select.val());
    var secilenItem = selectList.find(x => x.tabloID === selectedID);

    if (secilenItem) {
        selectedKaynakId = selectedID;
        // Yeni eklenen satır: Seçilen kaynağın gün tipini global değişkene ata
        selectedGunTipi = secilenItem.kaynakRezerveTanimVM.uygunGunTipleri;
        saat.text(secilenItem.kaynakRezerveTanimVM.rezerveSaatBaslangicDegeri + " - " + secilenItem.kaynakRezerveTanimVM.rezerveSaatBitisDegeri);
        KaynakAdi.text(secilenItem.kaynakTanimVM.kaynakAdi);
        KaynakAciklama.text(secilenItem.kaynakTanimVM.kaynakAciklama);
        newEventDiv.text(secilenItem.kaynakTanimVM.kaynakAdi);
        newEventDiv.attr('data-id', selectedID);
        newEventDiv.attr('data-title', secilenItem.kaynakTanimVM.kaynakAdi);
        newEventDiv.show(); // Gerekirse görünür hale getir

        var baslangicSaati = secilenItem.kaynakRezerveTanimVM.rezerveSaatBaslangicDegeri;
        var bitisSaati = secilenItem.kaynakRezerveTanimVM.rezerveSaatBitisDegeri;

        if (viewType === 'dayGridMonth') {
            $("#SaatPopupAy").val(baslangicSaati).attr('min', baslangicSaati).attr('max', bitisSaati);
            $("#SaatPopupENDAy").val(bitisSaati).attr('min', baslangicSaati).attr('max', bitisSaati);
            //$("#TarihPopupEND").val(formatliTarih).attr('min', formatliTarih); // SİLİNDİ
        } else {
            $("#SaatPopup").val(baslangicSaati).attr('min', baslangicSaati).attr('max', bitisSaati);
            $("#SaatPopupEND").val(bitisSaati).attr('min', baslangicSaati).attr('max', bitisSaati);
        }
    } else {
        KaynakAdi.text('');
        KaynakAciklama.text('');
        newEventDiv.text('Rezervasyon seçiniz');
        newEventDiv.attr('data-id', '');
        newEventDiv.attr('data-title', '');
        selectedKaynakId = null;
        selectedGunTipi = null;
    }
    renkliSaatAlanlari();
    if (calendar?.view?.type === 'timeGridDay') {
        sadeceSeciliMainIdSaatGorunumundeGoster();
    } else {
        tumEventleriGorunurYap();
    }
}
function FromDragEkle() {
    //$("#eventFormContainer").html(`  
    //    <h4>Yeni Event Ekle</h4>
    //    <form id="eventForm">
    //        <div id="external-events">
    //            <div class="fc-event fc-h-event fc-daygrid-event fc-daygrid-block-event" id="NewEvent" data-id="" data-title="">Rezervasyon seçiniz</div>
    //        </div>
    //    </form>
    //        `);
    //$("#eventFormContainer").css("display", "block");

    //var Draggable = FullCalendar.Draggable;

    //new Draggable(document.getElementById('external-events'), {
    //    itemSelector: '.fc-event',
    //    eventData: function (eventEl) {
    //        return {
    //            title: eventEl.getAttribute('data-title'),
    //            extendedProps: {
    //                kaynakId: eventEl.getAttribute('data-id')
    //            }
    //        };
    //    },
    //    dragStart: function (event) {
    //        event.el.style.border = "2px solid blue";
    //        event.el.style.backgroundColor = "#3788d8";
    //        event.el.style.color = "#FFFFFFFF";
    //        event.el.style.maxWidth = "400px";
    //        event.el.style.width = "auto";
    //    },
    //});
}
function FromDragEkleAy() {
    $("#eventFormContainerAy").html(`  
        <h4>Yeni Event Ekle</h4>
        <form id="eventForm">
            <div id="external-events-day" class="text-light">
                <div class="fc-event fc-h-event fc-daygrid-event fc-daygrid-block-event" style="width:auto; max-width: 130px;" id="NewEvent" data-id="" data-title="">Rezervasyon seçiniz</div>
            </div>
        </form>
            `);
    $("#eventFormContainerAy").css("display", "block");

    var Draggable = FullCalendar.Draggable;

    new Draggable(document.getElementById('external-events-day'), {
        itemSelector: '.fc-event',
        eventData: function (eventEl) {
            return {
                title: eventEl.getAttribute('data-title'),
                extendedProps: {
                    kaynakId: eventEl.getAttribute('data-id')
                }
            };
        },
        dragStart: function (event) {
            event.el.style.border = "2px solid blue";
            event.el.style.backgroundColor = "#3788d8";
            event.el.style.color = "#FFFFFFFF";
            event.el.style.maxWidth = "400px";
            event.el.style.width = "auto";
        },
    });
}
function takvimeEventEkle(tanim, aciklama, tarihStr, saat, saatBitis, kaynakId, bitisTarih) {
    var eventId = `obj_${objectid}`;
    var mainID = kaynakId;

    if (!tarihStr || !saat || !saatBitis || !bitisTarih) {
        console.error("Eksik bilgi: takvimeEventEkle fonksiyonu için tüm parametreler gereklidir.");
        return;
    }

    var startStr = tarihStr.length > 10 ? tarihStr : tarihStr + 'T' + saat + ':00';
    var endStr = bitisTarih + 'T' + saatBitis + ':00';

    var startDate = new Date(startStr);
    var endDate = new Date(endStr);

    if (isNaN(startDate.getTime()) || isNaN(endDate.getTime())) {
        console.error("Tarih oluşturulamadı: Geçersiz tarih veya saat formatı.", startStr, endStr);
        return;
    }

    if (endDate < startDate) {

        Swal.fire('Dikkat', 'Bitiş tarihi, başlangıç tarihinden önce olamaz.', 'warning');
        return;
    }
    if (molaSaatleriyleCakisiyorMu(startDate, endDate, mainID)) {
        Swal.fire('Mola Saati', 'Bu saat aralığında mola saati bulunuyor. Lütfen başka bir saat seçin.', 'warning');
        return;
    }

    // Yeni eklenecek event objesini oluştur
    var yeniEventObjesi = {
        id: eventId,
        title: tanim,
        start: startDate.toISOString(),
        end: endDate.toISOString(),
        allDay: false,
        extendedProps: { aciklama: aciklama, mainId: mainID }
    };
/*     KAPASİTE KONTROLÜ*/
    if (!KapasiteKontrolu(startDate, endDate, mainID, yeniEventObjesi, false)) {
        Swal.fire(
            'Kapasite Aşıldı',
            'Bu saat aralığında en fazla kapasite kadar rezervasyon yapılabilir.',
            'error'
        );
        // YENİ EKLENEN KONTROL: Saat çakışması var mı?
        if (saatCakisiyorMu(yeniEventObjesi)) {
            Swal.fire('Çakışma Durumu', 'Bu saat aralığında Başka Rezervasyon bulunuyor. Lütfen başka bir saat seçin.', 'warning');
            return; // Çakışma varsa işlemi durdur
        }
        return;
    }

    console.log("Event ekleniyor:", tanim, startDate.toISOString(), endDate.toISOString());

    calendar.addEvent(yeniEventObjesi);
    TakvimdeBulunanEventLer.push(yeniEventObjesi); // Event'i diziye ekle
    objectid++;

    // Event verisini backend'e gönder
    EventlariKayitEt(tanim, aciklama, tarihStr.split('T')[0], bitisTarih, saat, saatBitis, mainID);
}

function modalKapatVeTemizle(formEl) {
    $('#popupModal').modal('hide');
    if (formEl) formEl.reset();
    pendingDrop = null;
    pendingDragged = null;
    dataOfRezervasyon = [];
}
function modalKapatVeTemizleAy(formEl) {
    $('#popupModalAy').modal('hide');
    if (formEl) formEl.reset();
    pendingDrop = null;
    pendingDragged = null;

    dataOfRezervasyon = [];
}
function SaatDenetim(dropInfo, draggedEvent) {
    var KaynakListID = selectedKaynakId;
    if (!KaynakListID) return false;

    // Kaynak verisi(tabloID eşleşmeli)
    var kaynak = dataKanyakRezerve.find(x => x.tabloID == parseInt(KaynakListID));
    if (!kaynak) return false;

    // Burada kaynak içindeki saat aralığını alıyoruz
    // Örnek isimler, sen data modeline göre değiştir
    var baslangicSaat = kaynak.kaynakRezerveTanimVM.rezerveSaatBaslangicDegeri || "00:00";
    var bitisSaat = kaynak.kaynakRezerveTanimVM.rezerveSaatBitisDegeri || "23:59";

    function parseSaat(saatStr) {
        var parts = saatStr.split(':');
        var d = new Date();
        d.setHours(parseInt(parts[0], 10));
        d.setMinutes(parseInt(parts[1], 10));
        d.setSeconds(0);
        d.setMilliseconds(0);
        return d;
    }

    var kaynakBaslangic = parseSaat(baslangicSaat);
    var kaynakBitis = parseSaat(bitisSaat);

    var eventBaslangic = new Date(dropInfo.start);
    var eventBitis = new Date(dropInfo.end);

    // 1970 yılına sabitle sadece saat karşılaştırması
    kaynakBaslangic.setFullYear(1970, 0, 1);
    kaynakBitis.setFullYear(1970, 0, 1);
    eventBaslangic.setFullYear(1970, 0, 1);
    eventBitis.setFullYear(1970, 0, 1);

    if (!KaynakListID || !gunTipi) return false;
    var izinVer = (eventBaslangic >= kaynakBaslangic && eventBitis <= kaynakBitis);

    var dropDay = dropInfo.start.getDay(); // 0=Pazar, 1=Pazartesi, ..., 6=Cumartesi
    var isHaftaIci = dropDay >= 1 && dropDay <= 5;
    var isHaftaSonu = dropDay === 0 || dropDay === 6;

    // Gün tipine göre ek kontrol
    if (gunTipi === 1 && !isHaftaIci) { // Sadece hafta içi
        izinVer = false;
        console.warn("Bu kaynak sadece hafta içi rezerve edilebilir.");
    }
    if (gunTipi === 2 && !isHaftaSonu) { // Sadece hafta sonu
        izinVer = false;
        console.warn("Bu kaynak sadece hafta sonu rezerve edilebilir.");
    }
    console.log("Event izin durumu:", izinVer, "Event:", eventBaslangic.toTimeString(), "-", eventBitis.toTimeString(), "Kaynak:", kaynakBaslangic.toTimeString(), "-", kaynakBitis.toTimeString());

    return izinVer;
}
//Aynı Filtreleme
function SaatDenetimSelect(selectInfo) {
    var KaynakListID = selectedKaynakId;
    console.log("Seçilen KaynakListID (string):", KaynakListID);

    if (!KaynakListID) {
        console.warn("KaynakListID boş, seçim engellendi.");
        return false; // Kaynak seçili değilse seçim engel
    }

    var filter = dataKanyakRezerve.find(x => x.tabloID == parseInt(KaynakListID));

    if (!filter) {
        console.warn("Seçilen kaynak bulunamadı, seçim engellendi.");
        return false;
    }

    // Kaynak rezervasyon saat aralığını al (örnek alan isimleri, gerçek isimlerle değiştir)
    var baslangicSaatStr = filter.kaynakRezerveTanimVM.rezerveSaatBaslangicDegeri;
    var bitisSaatStr = filter.kaynakRezerveTanimVM.rezerveSaatBitisDegeri;

    // selectInfo.start ve selectInfo.end Date objesi olarak geliyor
    var secilenBaslangicSaat = selectInfo.start.getHours() + ":" + String(selectInfo.start.getMinutes()).padStart(2, '0');
    var secilenBitisSaat = selectInfo.end.getHours() + ":" + String(selectInfo.end.getMinutes()).padStart(2, '0');

    // Saatleri karşılaştırmak için Date objesi oluşturalım (sadece saat/dakika önemli)
    function parseSaat(saatStr) {
        var parts = saatStr.split(':');
        var d = new Date();
        d.setHours(parseInt(parts[0], 10));
        d.setMinutes(parseInt(parts[1], 10));
        d.setSeconds(0);
        d.setMilliseconds(0);
        return d;
    }

    var kaynakBaslangic = parseSaat(baslangicSaatStr);
    var kaynakBitis = parseSaat(bitisSaatStr);

    var secilenBaslangic = parseSaat(secilenBaslangicSaat);
    var secilenBitis = parseSaat(secilenBitisSaat);
    if (!KaynakListID || !gunTipi) {
        console.warn("Kaynak seçili değil, seçim engellendi.");
        return false;
    }

    // Seçilen aralık, kaynağın saat aralığı içinde olmalı
    var uygun = (secilenBaslangic >= kaynakBaslangic) && (secilenBitis <= kaynakBitis);
    var selectedDay = selectInfo.start.getDay(); // Seçimin başlangıç günü
    var isHaftaIci = selectedDay >= 1 && selectedDay <= 5;
    var isHaftaSonu = selectedDay === 0 || selectedDay === 6;

    if (gunTipi === 1 && !isHaftaIci) {
        uygun = false;
        console.warn("Bu kaynak sadece hafta içi rezerve edilebilir.");
    }
    if (gunTipi === 2 && !isHaftaSonu) {
        uygun = false;
        console.warn("Bu kaynak sadece hafta sonu rezerve edilebilir.");
    }
    // gunTipi 3 ise (tüm hafta), ek bir kısıtlama yok.
    if (!uygun) {
        console.warn("Seçilen saat aralığı kaynak saat aralığı dışında:", baslangicSaatStr, "-", bitisSaatStr);
    }

    return uygun;
}
// Eventleri kaydetme
function EventlariKayitEt(tanim, aciklama, baslangicTarihi, bitisTarihi, baslangicSaat, bitisSaat, kaynakId) {
    // Sunucuya göndereceğimiz veri objesini oluşturalım
    var eventData = {
        RezervasyonBaslik: tanim,
        RezervasyonAciklama: aciklama,
        RezerveBaslangicTarihi: baslangicTarihi,
        //RezerveBitisTarihi: bitisTarihi, // SİLİNDİ
        RezerveBaslangicZamani: baslangicSaat,
        RezerveBitisZamani: bitisSaat,
        KaynakTanimId: kaynakId
    };

    $.ajax({
        url: '/KaynakRezerve/EventKaydet',
        type: "POST",
        contentType: 'application/json',
        data: JSON.stringify(eventData),
        success: function (response) {
            console.log("Event başarıyla kaydedildi.", response);
            location.reload();
            Swal.fire('Başarılı!', 'Rezervasyonunuz başarıyla oluşturuldu.', 'success');
        },
        error: function (xhr, status, error) {
            console.error("Event kaydedilirken bir hata oluştu:", error);
        }
    });
}

//Listeleme
function EventleriListele() {
    var bugun = 
    $.ajax({
        url: '/KaynakRezerve/EventListele',
        type: "GET",
        success: function (response) {
            var val = [];
            val = response.value;
            console.log(val);
            if (Array.isArray(val)) {
                val.forEach(event => {
                    // Tarih stringini 'T' karakterinden ayırarak sadece YYYY-MM-DD kısmı
                    var baslangicTarihiKisa = event.rezerveBaslangicTarihi ? event.rezerveBaslangicTarihi.split('T')[0] : '';
                    var bitisTarihiKisa = event.rezerveBitisTarihi ? event.rezerveBitisTarihi.split('T')[0] : '';

                    var temizBitisZamaniStr = event.rezerveBitisZamani;
                    if (temizBitisZamaniStr && temizBitisZamaniStr.split(':').length > 2) {
                        // Sadece saat ve dakikayı al
                        var zamanParcalari = temizBitisZamaniStr.split(':');
                        temizBitisZamaniStr = `${zamanParcalari[0]}:${zamanParcalari[1]}`;
                    }

                    takvimeEventListele(
                        event.rezervasyonBaslik,
                        event.rezervasyonAciklama,
                        baslangicTarihiKisa, // Artık YYYY-MM-DD formatında
                        event.rezerveBaslangicZamani,
                        temizBitisZamaniStr,
                        event.kaynakTanimID,
                        bitisTarihiKisa,
                        event.tabloID // Artık YYYY-MM-DD formatında
                    );

                    if (calendar?.view?.type === 'timeGridDay') {
                        sadeceSeciliMainIdSaatGorunumundeGoster();
                    }

                });
            } else {
                console.error("Hata: Event verisi bir dizi formatında değil. Gelen veri:", val);
            }
        },
        error: function (xhr, status, error) {
            console.error("Eventler alınırken bir hata oluştu:", error);
        }
    });
}
function takvimeEventListele(tanim, aciklama, tarihStr, saat, saatBitis, kaynakId, bitisTarih, tabloID) {
    const bugun = new Date();
    bugun.setHours(0, 0, 0, 0);

    const eventTarihi = new Date(tarihStr);
    eventTarihi.setHours(0, 0, 0, 0);

    if (eventTarihi < bugun) {
        console.warn("Geçmiş tarihli event listelenmiyor:", tanim, tarihStr);
        return;
    }

    var eventId = `obj_${objectid}`;
    var mainID = kaynakId;
    var event = calendar.getEventById(eventId);
    if (event) {
        event.remove();
    }

    if (!tarihStr || !saat || !saatBitis) {
        console.error("Eksik bilgi:", tarihStr, saat, saatBitis);
        return;
    }

    var startStr = tarihStr + 'T' + saat;
    var endStr = tarihStr + 'T' + saatBitis + ':00';

    var startDate = new Date(startStr);
    var endDate = new Date(endStr);

    if (isNaN(startDate.getTime()) || isNaN(endDate.getTime())) {
        console.error("Tarih oluşturulamadı:", startStr, endStr);
        return;
    }

    if (endDate < startDate) {
        console.error("Hata: Bitiş tarihi, başlangıç tarihinden önce olamaz.");
        return;
    }

    console.log("Event ekleniyor:", tanim, startDate.toISOString(), endDate.toISOString());

    calendar.addEvent({
        id: eventId,
        title: tanim,
        start: startDate.toISOString(),
        end: endDate.toISOString(),
        allDay: false,
        extendedProps: { aciklama: aciklama, mainId: mainID, tabloId: tabloID },
        color: '#ff0000'
    });

    var CalendarModel = {
        id: eventId,
        title: tanim,
        start: startDate.toISOString(),
        end: endDate.toISOString(),
        allDay: false,
        extendedProps: { aciklama: aciklama, mainId: mainID, tabloId: tabloID },
        color: '#ff0000'
    };
    TakvimdeBulunanEventLer.push(CalendarModel);
    console.log("Eklenen Model :", TakvimdeBulunanEventLer);
    objectid++;
}

// Event Veri Getir()

function EventVeriGetir(eventId) {
    var eventid = eventId;
    console.log(eventid);
    $.ajax({
        url: '/KaynakRezerve/EventVeriGetir/' + eventid,
        type: 'GET',
        data: { Id: eventid },
        success: function (result) {
            console.log(result.value);
            VeriGetirPopUp(eventId);
            $("#poppuVeriModal").modal('show');
            PopUpVeriDoldur(result);
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function VeriGetirPopUp(id) {
    console.log("id:", id);
    $("#poppuVeriModal").remove();
    var popupDiv = `<div class="modal fade" id="poppuVeriModal" aria-labelledby="popupModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="popupModalLabel">Bilgilerini Gir</h5>
                </div>
                <div class="modal-body">
                    <form>
                        <div class="mb-3">
                            <label for="TanimPopupVeri" class="form-label">Başlık</label>
                            <input type="text" class="form-control" id="TanimPopupVeri" required>
                        </div>
                        <div class="mb-3">
                            <label for="AciklamaPopupVeri" class="form-label">Açıklama</label>
                            <input type="text" class="form-control" id="AciklamaPopupVeri">
                        </div>
                        <div class="mb-3">
                            <label for="SaatPopupVeri" class="form-label">Saat/Başlangıç (HH:MM)</label>
                            <input type="time" class="form-control" id="SaatPopupVeri" value="09:00" required>
                        </div>
                        <div class="mb-3">
                            <label for="SaatPopupENDVeri" class="form-label">Saat/Bitiş (HH:MM)</label>
                            <input type="time" class="form-control" id="SaatPopupENDVeri" value="09:00" required>
                        </div>
                        <div class="mb-3">
                            <label for="SaatPopupENDVeri" class="form-label">Kiralanıcak Gün (GG:AA:YYYY)</label>
                            <input type="date" class="form-control" id="TarihPopupVeri" value="09:00" required>
                        </div>
                    </form>
                    <button id="eventGuncelleBtn" class="btn btn-success" data-id="${id}">Guncelle</button>
                    <button id="eventSilBtn" class="btn btn-danger" data-id="${id}">Rezervasyonu İptal Et</button>
                </div>
            </div>
        </div>
    </div>`;

    $("body").append(popupDiv);

}

$(document).on('click', '#eventSilBtn', function () {
    const id = $(this).data('id');
    if (id) {
        EventSil(id);
    } else {
        Swal.fire('Hata', 'Silinecek rezervasyon ID bilgisi bulunamadı!', 'error');
    }
});

$(document).on('click', '#eventGuncelleBtn', function () {
    const id = $(this).data('id');
    if (id) {
        EventGuncelle(id);
    } else {
        Swal.fire('Hata', 'Silinecek rezervasyon ID bilgisi bulunamadı!', 'error');
    }
});

function PopUpVeriDoldur(val) {
    var value = val.value;
    var baslangicTarihiKisa = value[0].rezerveBaslangicTarihi ? value[0].rezerveBaslangicTarihi.split('T')[0] : '';
    console.log("PopUpVeriDoldur fonksiyonu çağrıldı. Gelen veri:", value);

    $("#TanimPopupVeri").val(value[0].rezervasyonBaslik);
    console.log("rezervasyonBaslik değeri:", value[0].rezervasyonBaslik);

    $("#AciklamaPopupVeri").val(value[0].rezervasyonAciklama);
    console.log("rezervasyonAciklama değeri:", value[0].rezervasyonAciklama);

    $("#SaatPopupVeri").val(value[0].rezerveBaslangicZamani);
    console.log("rezerveBaslangicZamani değeri:", value[0].rezerveBaslangicZamani);

    // rezerveBitisZamani değişkeninin `value` objesi içinde olup olmadığını kontrol edin
    // Eğer yoksa undefined hatası alabilirsiniz.
    $("#SaatPopupENDVeri").val(value[0].rezerveBitisZamani);
    console.log("rezerveBitisZamani değeri:", value[0].rezerveBitisZamani);

    $("#TarihPopupVeri").val(baslangicTarihiKisa);
    console.log("rezerveBitisZamani değeri:", baslangicTarihiKisa);

    // rezerveBitisTarihi değişkeninin `value` objesi içinde olup olmadığını kontrol edin
    // Eğer yoksa undefined hatası alabilirsiniz.

    return value[0];
}
function EventSil(id) {
    // Silme işleminden önce kullanıcıya onay sorusu sor.
    Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu rezervasyonu kalıcı olarak silmek istediğinizden emin misiniz?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Evet, Sil!',
        cancelButtonText: 'Hayır, İptal Et'
    }).then((result) => {
        if (result.isConfirmed) {
            // Silme işlemi için ID kontrolü
            if (id === undefined || id === null) {
                console.error("Hata: Silinecek Event ID'si belirtilmemiş.");
                Swal.fire('Hata', 'Silinecek rezervasyon bulunamadı!', 'error');
                return;
            }

            $.ajax({
                url: '/KaynakRezerve/EventSil/' + id,
                type: 'POST',
                success: function (sonuc) {
                    if (sonuc.isSuccess) {
                        Swal.fire(
                            'Başarılı!',
                            'Rezervasyon başarıyla silindi.',
                            'success'
                        );
                        // FullCalendar'dan ilgili event'i kaldır
                        var event = calendar.getEventById(id);
                        if (event) {
                            event.remove();
                        }
                        // Modalı kapat
                        $("#poppuVeriModal").modal('hide');
                        location.reload();
                    } else {
                        console.error("Silme işlemi hatası: ", sonuc.message);
                        Swal.fire('Hata', sonuc.message, 'error');
                    }
                },
                error: function (e) {
                    console.error("Silme işlemi hatası:", e.responseText);
                    Swal.fire('Hata', 'Rezervasyon silinirken bir sorun oluştu.', 'error');
                }
            });
        }
    });
}

// Event güncelleme fonksiyonu
function EventGuncelle(eventId) {
    var baslik = $("#TanimPopupVeri").val();
    var aciklama = $("#AciklamaPopupVeri").val();
    var baslangicSaat = $("#SaatPopupVeri").val();
    var bitisSaat = $("#SaatPopupENDVeri").val();
    var tarih = $("#TarihPopupVeri").val();

    var guncelData = {
        TabloID: eventId,
        RezervasyonBaslik: baslik,
        RezervasyonAciklama: aciklama,
        RezerveBaslangicZamani: baslangicSaat,
        RezerveBitisZamani: bitisSaat,
        RezerveBaslangicTarihi: tarih
    };

    $.ajax({
        url: `/KaynakRezerve/EventGuncelle`, 
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(guncelData),
        success: function (result) {
            if (result.isSuccess) {
                location.reload();
                Swal.fire('Başarılı!', 'Rezervasyonunuz başarıyla güncellendi.', 'success');
            } else {
            }
        },
        error: function (xhr, status, error) {
            console.error('Güncelleme işlemi hatası:', error);
            
        }
    });
}


function saatCakisiyorMu(yeniEvent) {
    const mevcutEventler = calendar.getEvents();

    console.log("--- saatCakisiyorMu kontrolü başladı ---");
    console.log("Yeni eklenecek event:", yeniEvent);

    const yeniMainIdHam = yeniEvent.extendedProps.mainId;
    const yeniMainId = Number(yeniMainIdHam ? String(yeniMainIdHam).trim() : 0);

    if (isNaN(yeniMainId) || yeniMainId === 0) {
        console.warn("Yeni event için geçerli bir Kaynak ID'si (mainId) bulunamadı veya 0. Çakışma kontrolü atlandı.");
        console.log("--- Çakışma bulunamadı, false döndürüldü ---");
        return false;
    }

    for (const mevcutEvent of mevcutEventler) {
        if (mevcutEvent.id === yeniEvent.id) {
            continue;
        }

        const mevcutMainIdHam = mevcutEvent.extendedProps.mainId;
        const mevcutMainId = Number(mevcutMainIdHam ? String(mevcutMainIdHam).trim() : 0);

        console.log(`Karşılaştırılan Eventler: Mevcut ID: ${mevcutEvent.id}, Yeni ID: ${yeniEvent.id}`);
        console.log(`mainId kontrolü: Mevcut: '${mevcutMainIdHam}' (${typeof mevcutMainIdHam}) -> Dönüştürülmüş: ${mevcutMainId} (${typeof mevcutMainId})`);
        console.log(`             Yeni: '${yeniMainIdHam}' (${typeof yeniMainIdHam}) -> Dönüştürülmüş: ${yeniMainId} (${typeof yeniMainId})`);

        if (mevcutMainId === yeniMainId) {
            console.log(`mainId'ler eşleşti (${yeniMainId}). Tarih kontrolü yapılıyor.`);

            const yeniEventBaslangic = new Date(yeniEvent.start);
            const yeniEventBitis = new Date(yeniEvent.end);
            const mevcutEventBaslangic = new Date(mevcutEvent.start);
            const mevcutEventBitis = new Date(mevcutEvent.end);

            if (isNaN(yeniEventBaslangic) || isNaN(yeniEventBitis) || isNaN(mevcutEventBaslangic) || isNaN(mevcutEventBitis)) {
                console.error("HATA: Bir tarih objesi geçersiz (Invalid Date). Formatı kontrol edin.");
                continue;
            }

            if ((yeniEventBaslangic < mevcutEventBitis) && (yeniEventBitis > mevcutEventBaslangic)) {
                console.warn(`--- ÇAKIŞMA BULUNDU! ---`);
                console.warn(`Kaynak ID ${yeniMainId} için ${mevcutEventBaslangic} - ${mevcutEventBitis} aralığında bir rezervasyon zaten mevcut.`);
                console.warn("------------------------");
                return true;
            }
        } else {
            console.log("mainId'ler eşleşmedi, bir sonraki event'e geçiliyor.");
        }
    }

    console.log("--- Çakışma bulunamadı, false döndürüldü ---");
    return false;
}
function molaSaatleriyleCakisiyorMu(eventStart, eventEnd, draggedId) {
    var secilenKaynak = dataKanyakRezerve.find(x => x.tabloID == draggedId);

    if (!secilenKaynak) {
        console.error("Hata: Seçilen kaynak bulunamadı!");
        return false;
    }

    const kaynakIstisnalari = secilenKaynak.kaynakIstisnalariVM;
    if (!kaynakIstisnalari || kaynakIstisnalari.length === 0) {
        return false;
    }

    const eventDay = new Date(eventStart.getFullYear(), eventStart.getMonth(), eventStart.getDate());

    for (const istisna of kaynakIstisnalari) {

        const istisnaBaslangicTarihi = new Date(istisna.istisnaTarihBaslangicDegeri);
        const istisnaBitisTarihi = new Date(istisna.istisnaTarihtBitisDegeri);


        if (eventDay >= istisnaBaslangicTarihi && eventDay <= istisnaBitisTarihi) {

            if (!istisna.istisnaSaatBaslangicDegeri || !istisna.istisnaSaatBitisDegeri) {
                return true;
            }

            const eventBaslangicZamani = eventStart.getHours() * 60 + eventStart.getMinutes();
            const eventBitisZamani = eventEnd.getHours() * 60 + eventEnd.getMinutes();

            const istisnaBaslangicZamani = new Date('1970/01/01 ' + istisna.istisnaSaatBaslangicDegeri).getHours() * 60 + new Date('1970/01/01 ' + istisna.istisnaSaatBaslangicDegeri).getMinutes();
            const istisnaBitisZamani = new Date('1970/01/01 ' + istisna.istisnaSaatBitisDegeri).getHours() * 60 + new Date('1970/01/01 ' + istisna.istisnaSaatBitisDegeri).getMinutes();

            if (
                (eventBaslangicZamani >= istisnaBaslangicZamani && eventBaslangicZamani < istisnaBitisZamani) ||
                (eventBitisZamani > istisnaBaslangicZamani && eventBitisZamani <= istisnaBitisZamani) ||
                (istisnaBaslangicZamani >= eventBaslangicZamani && istisnaBitisZamani <= eventBitisZamani) ||
                (eventBaslangicZamani <= istisnaBaslangicZamani && eventBitisZamani >= istisnaBitisZamani) ||
                (eventBaslangicZamani === istisnaBaslangicZamani && eventBitisZamani === istisnaBitisZamani)
            ) {
                return true;
            }
        }
    }
    return false;
}

function KapasiteKontrolu(eventStart, eventEnd, draggedId, yeniEventObjesi, dropMu) {
    if (dropMu == true) {
        var secilenKaynak = dataKanyakRezerve.find(x => x.tabloID == draggedId);
        if (!secilenKaynak) return false;

        var kapasite = secilenKaynak.kapsiteVM[0].kapasaite;

        var mevcutEventler = calendar.getEvents();

        var yeniGun = new Date(eventStart.getFullYear(), eventStart.getMonth(), eventStart.getDate());

        var ayniGunEventler = mevcutEventler.filter(ev => {
            var evStart = ev.start;
            var evGun = new Date(evStart.getFullYear(), evStart.getMonth(), evStart.getDate());
            return evGun.getTime() === yeniGun.getTime();
        });

        if (ayniGunEventler.length >= kapasite) {

            return false;
        }
        return true;
    }
    else {
        var secilenKaynak = dataKanyakRezerve.find(x => x.tabloID == draggedId);
        if (!secilenKaynak) return false;


        var kapasite = secilenKaynak.kapsiteVM[0].kapasaite;


        var mevcutEventler = calendar.getEvents();


        var yeniGun = new Date(eventStart.getFullYear(), eventStart.getMonth(), eventStart.getDate());

        // Aynı gün içinde olanları bul
        var ayniGunEventler = mevcutEventler.filter(ev => {
            var evStart = ev.start;
            var evGun = new Date(evStart.getFullYear(), evStart.getMonth(), evStart.getDate());
            return evGun.getTime() === yeniGun.getTime();
        });

        // Aynı saat aralığında çakışanları bul
        var cakisanlar = ayniGunEventler.filter(ev => {
            var evStart = new Date(ev.start);
            var evEnd = new Date(ev.end);

            return (eventStart < evEnd && eventEnd > evStart);
        });

        console.log("Gün:", yeniGun.toDateString(), "Çakışan Event Sayısı:", cakisanlar.length, "Kapasite:", kapasite);
        // Kapasiteyi aşıyorsa false dön
        if (cakisanlar.length == kapasite) {
            doluKapasiteModel.push({
                kapasite: kapasite,
                cell: yeniGun
            });
            return false;

        }
        return true;
    }
}

function TumGunIzinliMi(eventDate, draggedId) {
    var secilenKaynak = dataKanyakRezerve.find(x => x.tabloID == draggedId);
    if (!secilenKaynak || !secilenKaynak.kaynakIstisnalariVM) {
        return false;
    }

    const kaynakIstisnalari = secilenKaynak.kaynakIstisnalariVM;
    const KaynakRezerveleri = secilenKaynak.kaynakRezerveTanimVM;

    const dropDay = new Date(eventDate.getFullYear(), eventDate.getMonth(), eventDate.getDate());

    for (const istisna of kaynakIstisnalari) {
        // İstisna başlangıç ve bitiş tarihlerini Date objesine dönüştür
        const istisnaBaslangicTarihi = new Date(istisna.istisnaTarihBaslangicDegeri);
        const istisnaBitisTarihi = new Date(istisna.istisnaTarihtBitisDegeri);

        // Tarih karşılaştırmasını Date objeleri üzerinden yap
        if (dropDay >= istisnaBaslangicTarihi && dropDay <= istisnaBitisTarihi) {

            const baslangicSaati = istisna.istisnaSaatBaslangicDegeri;
            const bitisSaati = istisna.istisnaSaatBitisDegeri;

            if (!baslangicSaati || !bitisSaati || baslangicSaati === 'null' || bitisSaati === 'null') {
                return true;
            }

            if (baslangicSaati === KaynakRezerveleri.rezerveSaatBaslangicDegeri && bitisSaati === KaynakRezerveleri.rezerveSaatBitisDegeri) {
                return true;
            }
        }
    }
    return false;
}


//function SayfadaKapasiteKontrolEt(draggedId, eventDate) {

//    var secilenkaynak = dataKanyakRezerve.find(x => x.tabloID == draggedId);
//    if (!secilenkaynak || !secilenkaynak.kapsiteVM || secilenkaynak.kapsiteVM.length === 0) {
//        console.error("Hata: Kapasite bilgisi veya kaynak bulunamadı. ID:", draggedId);
//        return true;
//    }

//    var kapasite = secilenkaynak.kapsiteVM[0].kapasaite;

//    var oGuneAitEventler = TakvimdeBulunanEventLer.filter(function (event) {
//        const eventTarihi = new Date(event.start.substring(0, 10));
//        const kontrolEdilenGun = new Date(eventDate.toISOString().substring(0, 10));

//        return event.extendedProps.mainId == draggedId && eventTarihi.getTime() === kontrolEdilenGun.getTime();
//    });

//    if (oGuneAitEventler.length >= kapasite) {
//        return false;
//    }
//    return true;
//}

function SayfadaKapasiteKontrolEt(draggedId, eventDate) {

    // 1. Kaynak ve kapasite bilgisini al
    var secilenkaynak = dataKanyakRezerve.find(x => x.tabloID == draggedId);
    if (!secilenkaynak || !secilenkaynak.kapsiteVM || secilenkaynak.kapsiteVM.length === 0) {
        console.error("Hata: Kapasite bilgisi veya kaynak bulunamadı. ID:", draggedId);
        return true; // Kapasite bilgisi yoksa dolu değilmiş gibi davran
    }

    var kapasite = secilenkaynak.kapsiteVM[0].kapasaite;

    // 2. FullCalendar'dan takvimdeki TÜM event'leri al
    // Bu, AJAX tamamlandıktan sonra calendar.addEvent() ile eklenen tüm eventleri içerir.
    var mevcutEventler = calendar.getEvents();

    // 3. Kontrol edilen güne ve kaynağa ait eventleri filtrele
    var oGuneAitEventler = mevcutEventler.filter(function (event) {

        const eventTarihi = new Date(event.start.getFullYear(), event.start.getMonth(), event.start.getDate());


        const kontrolEdilenGun = new Date(eventDate.getFullYear(), eventDate.getMonth(), eventDate.getDate());


        const isSameSource = String(event.extendedProps.mainId) === String(draggedId);
        const isSameDay = eventTarihi.getTime() === kontrolEdilenGun.getTime();

        return isSameSource && isSameDay;
    });

    // 4. Kapasite kontrolünü yap
    if (oGuneAitEventler.length >= kapasite) {
        return false; // Kapasite dolu
    }
    return true; // Kapasite mevcut
}