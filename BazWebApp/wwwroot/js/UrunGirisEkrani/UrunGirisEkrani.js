var UrunKategoriList = [];
let UrunModelVM = {};
var ParaBirimiList = [];
var kayitEdilenMedyaTabloID = {};
var quil;
var UrunMarkaList = [];
var UrunParametreleri = [];
var UrunIcerikBlokKategoriList = [];
var IcerikKutuphanesiList = [];

$(document).ready(function () {
    $(".tab-content").hide(); // Hepsini gizle
    $(".tablinks.active").each(function () {
        const hedefID = $(this).attr("onclick").match(/'(.*?)'/)[1];
        $("#" + hedefID).show(); // Aktif tabı göster
    });
    //
    IcerikKutuphanesiGetir();
    IcerikBlokKategorilerGetir();
    ParamParabirimiGetir();
    UrunAnaKategorileriGetir();
    MedyaEkle();
    BlokMedyaEkle();
    ParamUrunMarkalarınıGetir();
    UrunParametreDegerGetir();
    //editor
    var toolbarOptions = [
        ['bold', 'italic', 'underline'],        // kalın, italik, altı çizili
        [{ 'header': 1 }, { 'header': 2 }],     // başlık 1 ve 2
        [{ 'list': 'ordered' }, { 'list': 'bullet' }]  // sıralı ve sırasız liste
    ];

    quill = new Quill('#editor_1', {
        theme: 'snow',
        modules: {
            toolbar: toolbarOptions
        }
    });
});

function TabShow(button, Tab) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tab-content");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(Tab).style.display = "block";
    button.className += "active";

    document.querySelectorAll(".tab button").forEach(btn => {
        btn.addEventListener("click", function () {
            document.querySelectorAll(".tab button").forEach(b => b.classList.remove("active"));
            this.classList.add("active");
        });
     });
}
function UrunAnaKategorileriGetir() {
    $.ajax({
        type: "GET",
        url: "/UrunGiris/UrunKategorileriGetir",
        dataType: "json",
        data: null,
        success: function (result) {
            if (result.isSuccess == true) {
                UrunKategoriList = result.value;
                console.log(result.value);
                var anakategori = UrunKategoriList.filter(x => x.ustId == 0);
                ParamDDLDoldur("AnaKategori", anakategori);
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function UrunKategorileriGetir(value) {
    var altkategoriler = UrunKategoriList.filter(x => x.ustId == parseInt(value));
    ParamDDLDoldur("Kategori", altkategoriler);
}
function UrunAltKategoriGetir(value) {
    var altkategoriler = UrunKategoriList.filter(x => x.ustId == parseInt(value));
    ParamDDLDoldur("Alt-Kategori", altkategoriler);
}
function ParamUrunMarkalarınıGetir() {
    $.ajax({
        type: "GET",
        url: "/UrunGiris/ParamUrunMarkalarınıGetir",
        dataType: "json",
        data: null,
        success: function (result) {
            if (result.isSuccess == true) {
                UrunMarkaList = result.value;
                console.log(result.value);
                var marka = UrunMarkaList.filter(x => x.ustId == 0);
                ParamDDLDoldur("MarkaKategori", marka);
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function UrunModelgetir(value) {
    var model = UrunMarkaList.filter(x => x.ustId == parseInt(value));
    ParamDDLDoldur("ModelKategori", model);
}
function UrunSeriGetir(value) {
    var seri = UrunMarkaList.filter(x => x.ustId == parseInt(value));
    ParamDDLDoldur("SeriKategori", seri);
}
var UrunParamKontorl = [];
function UrunParametreDegerGetir() {
    $.ajax({
       type: "GET",
        url: "/UrunGiris/UrunParametreDegerGetir",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data.isSuccess == true) {
                console.log(data.value);
                UrunParametreleri = data.value;
                var hiz = UrunParametreleri.filter(x => x.paramKod == $("#HizBirim").data("id"));
                ParamDDLDoldur("HizBirim", hiz);
                var hacim = UrunParametreleri.filter(x => x.paramKod == $("#HacimBirim").data("id"));
                ParamDDLDoldur("HacimBirim", hacim);
                var olcum = UrunParametreleri.filter(x => x.paramKod == $("#OlcumBirim").data("id"));
                ParamDDLDoldur("OlcumBirim", olcum);
            }
            else {
            alertim.toast(siteLang.Hata, alertim.types.danger);
        }
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function IcerikBlokAltKategoriGetir(value) {
    var model = UrunIcerikBlokKategoriList.filter(x => x.ustId == parseInt(value));
    ParamDDLDoldur("IcerikBlokAlt-Kategori", model);
}
var IcerikKutuphanesiID;
function IcerikKutuphanesiGetir() {
    $.ajax({
        type: "GET",
        url: "/UrunGiris/IcerikKutuphanesiGetir",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data.isSuccess == true) {
                var input = $("#IcerikKutuphanesi");
                input.empty();

                input.append($('<option>', {
                    value: '',
                    text: 'Seçiniz'
                }));

                $.each(data.value, function (index, item) {
                    input.append($('<option>', {
                        value: item.tabloID,
                        text: item.icerikBaslik
                    }));
                });
                input.trigger('change');
            }
            else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function UrunOlcumBirimiData() {
    $.ajax({
        type: "GET",
        url: "/UrunGiris/UrunOlcumBirimiData",
        dataType: "json",
        data: null
    });
}
function IcerikBlokKategorilerGetir() {
    $.ajax({
        type: "GET",
        url: "/UrunGiris/IcerikBlokKategorilerGetir",
        dataType: "json",
        data: null,
        success: function (data) {
            if (data.isSuccess == true) {
                UrunIcerikBlokKategoriList = data.value;
                var ana = UrunIcerikBlokKategoriList.filter(x => x.ustId == 0);
                ParamDDLDoldur("IcerikBlokKategori", ana);
            }
            else {
                alertim.toast(siteLang.Hata, alertim.types.danger);

            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}
function ParamParabirimiGetir() {
    $.ajax({
        type: "GET",
        url: "/UrunGiris/UrunParaBirimiGetir",
        dataType: "json",
        data: null,
        success: function (result) {
            if (result.isSuccess == true) {
                console.log(result.value);
                var gelenData = result.value;
                ParaBirimiList = gelenData;
                var controlresult = ParaBirimiList.filter(x => x.aktifMi == 1 && x.silindiMi == 0);
                ParaBirimiVeriDoldur(controlresult);
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function ParaBirimiVeriDoldur(gelenData) {
    let options = '<option value="0">Seçiniz</option>';
    for (let i = 0; i < gelenData.length; i++) {
        console.log("gelen PARALAR: " + gelenData[i]);
        var TabloId = gelenData[i].tabloID;
        var Tanim = gelenData[i].paramTanim;
        options += `<option value="${TabloId}">${Tanim}</option>`;
    }
    $('#urunParabirimi').html(options);
    console.log($('#urunParabirimi').val());
}
var tekrarliMedyaId = -1;
var tekarliBlokMedyaId = -1;
function TekrarliMedyaEkle() {
    let showRemove = tekrarliMedyaId !== 0; // ilk spot için false
    let medyaHtml = '<div class="row">\
        <div class="col-md-4" >\
                <div class="form-group">\
                    <label class="Lbl">Medya Tipi</label>\
                    <select id="medyaTipi'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    </select>\
                    <label for="medyaTipi" id="MedyaTipiHata'+ tekrarliMedyaId + '" class="validationMessage">Medya Tipi Seçiniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Medya Adı</label>\
                    <input id="medyaAdi'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    <label for="medyaAdi" id="MedyaAdiHata'+ tekrarliMedyaId + '" class="validationMessage">Medya Adı Giriniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Gösterim Adı</label>\
                    <input id="gosterimAdi'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    <label for="gosterimAdi" id="gosterimAdiHata'+ tekrarliMedyaId + '" class="validationMessage">Gösterim Adı Giriniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Sıra</label>\
                    <select id="sira'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    </select>\
                    <label for="sira" id="siraHata'+ tekrarliMedyaId + '" class="validationMessage">Sıra Seçiniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Alt Metin</label>\
                    <input id="altMetin'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    <label for="altMetin" id="altMetinHata'+ tekrarliMedyaId + '" class="validationMessage">Alt Metin Seçiniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Dosya Ekle</label>\
                    <input type="file" id="dosyaEkle' + tekrarliMedyaId + '" class="form-control" style="width: 100%;" required/>\
                    <label for="dosyaEkleHata" id="dosyaEkleHata'+ tekrarliMedyaId + '" class="validationMessage">Dosya Seçiniz</label>\
                </div>\
         </div>\
            <div class="col-md-8">\
                <div class="form-group">\
                    <label class="Lbl">Açıklama</label>\
                    <textarea id="medyaAciklama'+ tekrarliMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required></textarea>\
                    <label for="medyaAciklama" id="medyaAciklamaHata'+ tekrarliMedyaId + '" class="validationMessage">Açıklama</label>\
                </div>\
            </div>\
            <div class="col-md-4" id="dosyaPreview'+ tekrarliMedyaId + '" class="dosya-preview" style="margin-top:5px;"></div>\
            <div class="d-flex justify-content-end">\
        <button id = "addSpot" onclick = "MedyaEkle(this)" class="btn mb-0" data - accordion - id="${datacordion}" > + Yeni Medya Ekle</button >\
        <div class="d-flex align-items-center MedyaRemoveButonWrapper" style="display:' + (showRemove ? 'flex' : 'none') + '">\
            <div class="MedyaRemoveButonGrp">\
                <button type="button" class="btn btn-xs MedyaRemoveButton" onclick="MedyaSil(this)">\
                    <i class="fas fa-minus-circle accent-danger"></i>\
                </button>\
            </div>\
        </div>\
        </div>\
      </div>';
    return medyaHtml;


}
function TekrarliBlokMedyaEkle() {
    let showRemove = tekarliBlokMedyaId !== 0; // ilk spot için false
    let medyaHtml = '<div class="row">\
        <div class="col-md-4" >\
                <div class="form-group">\
                    <label class="Lbl">Medya Tipi</label>\
                    <select id="blokmedyaTipi'+ tekarliBlokMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    </select>\
                    <label for="blokmedyaTipi" id="blokMedyaTipiHata'+ tekarliBlokMedyaId + '" class="validationMessage">Medya Tipi Seçiniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Sıra</label>\
                    <select id="bloksira'+ tekarliBlokMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required>\
                    </select>\
                    <label for="bloksira" id="bloksiraHata'+ tekarliBlokMedyaId + '" class="validationMessage">Sıra Seçiniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Üst Metin</label>\
                    <textarea id="blokmedyaustmetin'+ tekarliBlokMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required></textarea>\
                    <label for="blokmedyaustmetin" id="blokmedyaustmetinHata'+ tekarliBlokMedyaId + '" class="validationMessage">Üst Metin seçiniz</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Açıklama</label>\
                    <textarea id="blokmedyaAciklama'+ tekarliBlokMedyaId + '" class="form-control" style="width: 100%;" onchange="" data-placeholder="" required></textarea>\
                    <label for="blokmedyaAciklama" id="blokmedyaAciklamaHata'+ tekarliBlokMedyaId + '" class="validationMessage">Açıklama</label>\
                </div>\
            </div>\
            <div class="col-md-4">\
                <div class="form-group">\
                    <label class="Lbl">Dosya Ekle</label>\
                    <input type="file" id="blokdosyaEkle' + tekarliBlokMedyaId + '" class="form-control" style="width: 100%;" required/>\
                    <label for="blokdosyaEkle" id="blokdosyaEkleHata'+ tekarliBlokMedyaId + '" class="validationMessage">Dosya Seçiniz</label>\
                </div>\
         </div>\
            <div class="d-flex justify-content-end">\
        <div class="col-md-4" id="blokdosyaPreview'+ tekarliBlokMedyaId + '" class="dosya-preview" style="margin-top:5px;"></div>\
        <button id = "addSpot" onclick = "BlokMedyaEkle(this)" class="btn mb-0" data - accordion - id="${datacordion}" > + Yeni Medya Ekle</button >\
        <div class="d-flex align-items-center MedyaRemoveButonWrapper" style="display:' + (showRemove ? 'flex' : 'none') + '">\
            <div class="MedyaRemoveButonGrp">\
                <button type="button" class="btn btn-xs MedyaRemoveButton" onclick="BlokMedyaSil(this)">\
                    <i class="fas fa-minus-circle accent-danger"></i>\
                </button>\
            </div>\
        </div>\
        </div>\
      </div>';
    return medyaHtml;
}
function BlokMedyaEkle() {
    tekarliBlokMedyaId++;
    let html = TekrarliBlokMedyaEkle();
    $("#tekrarliMedyaBlok").append($(html));
    var medyaTipList = ParamMedyaTipleriGetir();
    var str = "<option value='0'>Seçiniz</option>";
    for (var i of medyaTipList) {
        var tanim = i.title == undefined ? i.tanim : i.title;
        str += "<option value='" + i.id + "'>" + i.title + "</option>";
    }
    $("#blokmedyaTipi" + tekarliBlokMedyaId).html(str);
    $('#blokmedyaTipi' + tekarliBlokMedyaId).select2();
    SiraSelectDoldur('bloksira' + tekarliBlokMedyaId, 1, 100);
}
// Dinamik olarak oluşturulan dosya inputları için event delegation kullan:
$(document).on("change", "[id^='dosyaEkle']", function () {
    var input = this;  // değişen input
    var idBul = input.id.replace("dosyaEkle", "");
    if (input.files.length) {
        var fd = new FormData();
        fd.append('file', input.files[0]);

        $.ajax({
            url: "/panel/resimyukle/",
            type: 'post',
            data: fd,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.isSuccess && response.value) {
                    var data = response.value;
                    var previewDiv = document.getElementById('dosyaPreview' + idBul);
                    if (previewDiv) {
                        previewDiv.setAttribute('data-media-id', data.tabloID);
                        var imageUrl = data.medyaUrl || null;
                        if (imageUrl) {
                            previewDiv.innerHTML = `<img src="http://localhost:51305${imageUrl}" style="width: 100px;" />`;
                        }
                        console.log("Yüklenen dosyanın tabloID'si:", data.tabloID);
                        kayitEdilenMedyaTabloID[idBul] = data.tabloID;
                        console.log(kayitEdilenMedyaTabloID);
                    } else {
                        toastr.error("Dosya gönderilemedi");
                    }
                }
            }
        });
    }
});

$(document).on("change", "[id^='blokdosyaEkle']", function () {
    var input2 = this;  // değişen input
    var idBul2 = input2.id.replace("blokdosyaEkle", "");
    if (input2.files.length) {
        var fd2 = new FormData();
        fd2.append('file', input2.files[0]);

        $.ajax({
            url: "/panel/resimyukle/",
            type: 'post',
            data: fd2,
            contentType: false,
            processData: false,
            success: function (response2) {
                if (response2.isSuccess && response2.value) {
                    var data2 = response2.value;
                    var previewDiv2 = document.getElementById('blokdosyaPreview' + idBul2);
                    if (previewDiv2) {
                        previewDiv2.setAttribute('data-media-id', data2.tabloID);
                        var imageUrl = data2.medyaUrl || null;
                        if (imageUrl) {
                            previewDiv2.innerHTML = `<img src="http://localhost:51305${imageUrl}" style="width: 100px;" />`;
                        }
                        console.log("Yüklenen dosyanın tabloID'si:", data2.tabloID);
                        kayitEdilenMedyaTabloID[idBul2] = data2.tabloID;
                        console.log(kayitEdilenMedyaTabloID);
                    } else {
                        toastr.error("Dosya gönderilemedi");
                    }
                }
            }
        });
    }
});
function BlokMedyaSil(button) {
    // Eğer sadece 1 tane spot kaldıysa silme
    if ($("#tekrarliMedyaBlok .row").length <= 1) return;

    // Butona basıldığı satırı sil
    $(button).closest(".row").remove();
    BlokMedyaIdleriGuncelle();
}
function BlokMedyaIdleriGuncelle() {
    $("#tekrarliMedyaBlok .row").each(function (index) {
        $(this).find('#medyaTipi' + (index + 1)).attr('id', 'medyaTipi' + index);
        $(this).find('#sira' + (index + 1)).attr('id', 'sira' + index);
        $(this).find('#medyaAciklama' + (index + 1)).attr('id', 'altMetin' + index);
    });
}
function SiraSelectDoldur(selectId, min = 1, max = 100) {
    let options = '<option value="0">Seçiniz</option>';
    for (let i = min; i <= max; i++) {
        options += `<option value="${i}">${i}</option>`;
    }
    $('#' + selectId).html(options);
}
function MedyaEkle() {
    tekrarliMedyaId++;
    let html = TekrarliMedyaEkle();
    $("#tekrarliMedya").append($(html));
    var medyaTipList = ParamMedyaTipleriGetir();
    var str = "<option value='0'>Seçiniz</option>";
    for (var i of medyaTipList) {
        var tanim = i.title == undefined ? i.tanim : i.title;
        str += "<option value='" + i.id + "'>" + i.title + "</option>";
    }
    $("#medyaTipi" + tekrarliMedyaId).html(str);
    $('#medyaTipi' + tekrarliMedyaId).select2();
    SiraSelectDoldur('sira' + tekrarliMedyaId, 1, 100);
}
function MedyaSil(button) {
    // Eğer sadece 1 tane spot kaldıysa silme
    if ($("#tekrarliMedya .row").length <= 1) return;

    // Butona basıldığı satırı sil
    $(button).closest(".row").remove();
    MedyaIdleriGuncelle();
}
function MedyaIdleriGuncelle() {
    $("#tekrarliMedya .row").each(function (index) {
        $(this).find('#medyaTipi' + (index + 1)).attr('id', 'medyaTipi' + index);
        $(this).find('#medyaAdi' + (index + 1)).attr('id', 'medyaAdi' + index);
        $(this).find('#gosterimAdi' + (index + 1)).attr('id', 'gosterimAdi' + index);
        $(this).find('#sira' + (index + 1)).attr('id', 'sira' + index);
        $(this).find('#altMetin' + (index + 1)).attr('id', 'altMetin' + index);
        $(this).find('#dosyaEkle' + (index + 1)).attr('id', 'dosyaEkle' + index);
    });
}
function ParamMedyaTipleriGetir() {
    var data = {
        ModelName: "ParamMedyaTipleri",
        UstId: 0,
        KurumId: 0,
        TabloID: 0,
        Tanim: "test",
        DilID: 1,
        EsDilID: 0
    }
    var list = [];
    $.ajax({
        data: data,
        url: "/panel/param/listparam",
        type: "POST",
        dataType: "json",
        async: false,
        success: function (x) {
            for (var item of x.valueOrDefault) {
                list.push({
                    id: item.tabloID,
                    title: item.tanim,
                });
            }
            //licenseService.ParaBirimleriSayfayaBas(list);
        },
        error: function (e) {


            console.log(e);
        }
    })
    return list;
}

function VerileriModeleBas() {
    // Temel Alanlar

    UrunModelVM["ParamAnaKategoriID"] = $("#AnaKategori").val();
    UrunModelVM["ParamUrunKategoriID"] = $("#Kategori").val();
    UrunModelVM["ParamAltKategoriID"] = $("#Alt-Kategori").val();
    UrunModelVM["ParamUrunTipiID"] = 0;
    UrunModelVM["UrunAdi"] = $("#UrunAdi_1").val();
    UrunModelVM["UrunAciklama"] = quill && quill ? quill.root.innerHTML : "";
    if ($("#TaslakMiCheckbox").is(":checked") == true) {
        UrunModelVM["TaslakMi"] = 1;
    }
    else {
        UrunModelVM["TaslakMi"] = 0;
    }
    UrunModelVM["UrunFiyatVM"] = [];
    UrunModelVM["UrunFiyatVM"].push({
        UrunFiyat: $("#urunFiyat").val(),
        ParaBirimiID: $('#urunParabirimi').val()
    });
    //UrunParametreleri
    UrunModelVM["UrunParametre"] = [];
    UrunModelVM["UrunParametre"].push({
        UrunMarkaID: $("#MarkaKategori").val(),
        UrunModelID: $("#ModelKategori").val(),
        UrunSeriID: $("#SeriKategori").val(),
        UrunHiz: $("#HizKategori").val(),
        UrunHizBirimID: $("#HizBirim").val(),
        UrunHacim: $("#HacimKategori").val(),
        UrunHacimBirimID: $("#HacimBirim").val(),
        UrunOlcum: $("#OlcumKategori").val(),
        UrunOlcumBirimID: $("#OlcumBirim").val()
    });
    UrunModelVM["IcerikBloklar"] = [];
    UrunModelVM["IcerikBloklar"].push({
        IcerikBlokTanim: $("#IcerikBlokTanim").val(),
        ParamBlokKategoriID: $("#IcerikBlokKategori").val(),
        ParamBlokAltKategoriID: $("#IcerikBlokAlt-Kategori").val(),
        IcerikKutuphanesiID: $("#IcerikKutuphanesi").val()
    });

    // Medyalar (Tekrarli Medya)
    UrunModelVM["Medyalar"] = [];
    $('#tekrarliMedya > div').each(function () {
        let container = $(this);
        let idSuffix = container.find('input[type="file"]').attr("id").replace("dosyaEkle", "");

        UrunModelVM["Medyalar"].push({
            MeydaKayitID: $("#dosyaPreview" + idSuffix).attr("data-media-id") || null,
            ParamMedyaTipiID: $("#medyaTipi" + idSuffix).val(),
            MedyaAdi: $("#medyaAdi" + idSuffix).val(),
            GosterimAdi: $("#gosterimAdi" + idSuffix).val(),
            SiraNo: $("#sira" + idSuffix).val(),
            AltMetin: $("#altMetin" + idSuffix).val(),
            MedyaID: $("#dosyaPreview" + idSuffix).attr("data-media-id") || null,
            MedyaUrl: null,
            MedyaTipiAdi: null,
            MedyaAciklama: $("#medyaAciklama" + idSuffix).val(), //string?
        });
    });

    UrunModelVM["IcerikMedyalar"] = [];
    $('#tekrarliMedyaBlok > div').each(function () {
        let container = $(this);
        let idSuffix = container.find('input[type="file"]').attr("id").replace("blokdosyaEkle", "");

        UrunModelVM["IcerikMedyalar"].push({
            MeydaKayitID: $("#blokdosyaPreview" + idSuffix).attr("data-media-id") || null,
            ParamMedyaTipiID: $("#blokmedyaTipi" + idSuffix).val(),
            SiraNo: $("#bloksira" + idSuffix).val(),
            UstMetin: $("#blokmedyaustmetin" + idSuffix).val(),
            MedyaID: $("#blokdosyaPreview" + idSuffix).attr("data-media-id") || null,
            MedyaUrl: null,
            MedyaTipiAdi: null,
            MedyaAciklama: $("#blokmedyaAciklama" + idSuffix).val(), //string?
        });
    });
    console.log(UrunModelVM);
    return UrunModelVM;
}

function UrunModeliKaydet() {
    var model = VerileriModeleBas();
    console.log(JSON.stringify(model, null, 2));
    $.ajax({
        type: "POST",
        async: false,
        url: "/UrunGiris/UrunKaydet",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (result) {
            if (result.isSuccess == true) {
                var d = result.value;
                window.location.href = "/UrunGiris/UrunGirisListesi";
            } else {
                alertim.toast(siteLang.Hata, alertim.types.danger);
            }
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function HataUyarilari() {
    let Errors = false;

    // =====================
    // Ana Kategori
    // =====================
    const anaKategori = document.getElementById("AnaKategori");
    const anaKategoriHata = document.getElementById("AnaKategoriHata");
    if (!anaKategori.value || anaKategori.value === "0") {
        anaKategoriHata.style.display = "block";
        Errors = true;
    } else {
        anaKategoriHata.style.display = "none";
    }

    // =====================
    // Kategori
    // =====================
    const kategori = document.getElementById("Kategori");
    const kategoriHata = document.getElementById("KategoriHata");
    if (!kategori.value || kategori.value === "0") {
        kategoriHata.style.display = "block";
        Errors = true;
    } else {
        kategoriHata.style.display = "none";
    }

    // =====================
    // Alt Kategori
    // =====================
    const altKategori = document.getElementById("Alt-Kategori");
    const altKategoriHata = document.getElementById("Alt-KategoriHata");
    if (!altKategori.value || altKategori.value === "0") {
        altKategoriHata.style.display = "block";
        Errors = true;
    } else {
        altKategoriHata.style.display = "none";
    }

    // =====================
    // Başlık
    // =====================
    const baslik = document.getElementById("UrunAdi_1");
    const baslikHata = document.getElementById("validationMessageBaslik_1");
    if (!baslik.value.trim()) {
        baslikHata.style.display = "block";
        Errors = true;
    } else {
        baslikHata.style.display = "none";
    }

    // =====================
    // Quill editör
    // =====================
    const editorText = quill.getText().trim();
    const metinHata = document.getElementById("validationMessageAnaMetin_1");
    if (!editorText) {
        metinHata.style.display = "block";
        Errors = true;
    } else {
        metinHata.style.display = "none";
    }

    // =====================
    // Fiyat
    // =====================
    const urunFiyat = document.getElementById("urunFiyat");
    const urunFiyatHata = document.getElementById("fiyattext");
    if (!urunFiyat.value) {
        urunFiyatHata.style.display = "block";
        Errors = true;
    } else {
        urunFiyatHata.style.display = "none";
    }

    // =====================
    // Para Birimi
    // =====================
    const ParaBirimi = document.getElementById("urunParabirimi");
    const ParBirimiaHata = document.getElementById("paratext");
    if (!ParaBirimi.value) {
        ParBirimiaHata.style.display = "block";
        Errors = true;
    } else {
        ParBirimiaHata.style.display = "none";
    }

    // =====================
    // Medya Alanları
    // =====================
    const medyaCount = document.querySelectorAll('[id^="medyaTipi"]').length;
    for (let i = 0; i < medyaCount; i++) {
        const medyaTipi = document.getElementById(`medyaTipi${i}`);
        const medyaAdi = document.getElementById(`medyaAdi${i}`);
        const gosterimAdi = document.getElementById(`gosterimAdi${i}`);
        const sira = document.getElementById(`sira${i}`);
        const altMetin = document.getElementById(`altMetin${i}`);
        const aciklama = document.getElementById(`medyaAciklama${i}`);
        const dosyaEkle = document.getElementById(`dosyaEkle${i}`);

        const medyaTipiHata = document.getElementById(`MedyaTipiHata${i}`);
        const medyaAdiHata = document.getElementById(`MedyaAdiHata${i}`);
        const gosterimAdiHata = document.getElementById(`gosterimAdiHata${i}`);
        const siraHata = document.getElementById(`siraHata${i}`);
        const altMetinHata = document.getElementById(`altMetinHata${i}`);
        const dosyaEkleHata = document.getElementById(`dosyaEkleHata${i}`);
        const aciklamamHata = document.getElementById(`medyaAciklamaHata${i}`);

        if (!medyaTipi || !medyaAdi || !gosterimAdi || !sira || !altMetin || !dosyaEkle) continue;

        if (medyaTipi.value === "0") {
            medyaTipiHata.style.display = "block";
            Errors = true;
        } else {
            medyaTipiHata.style.display = "none";
        }

        if (!medyaAdi.value.trim()) {
            medyaAdiHata.style.display = "block";
            Errors = true;
        } else {
            medyaAdiHata.style.display = "none";
        }

        if (!gosterimAdi.value.trim()) {
            gosterimAdiHata.style.display = "block";
            Errors = true;
        } else {
            gosterimAdiHata.style.display = "none";
        }

        if (!sira.value || sira.value === "0") {
            siraHata.style.display = "block";
            Errors = true;
        } else {
            siraHata.style.display = "none";
        }

        if (!altMetin.value.trim()) {
            altMetinHata.style.display = "block";
            Errors = true;
        } else {
            altMetinHata.style.display = "none";
        }

        if (!aciklama.value.trim()) {
            aciklamamHata.style.display = "block";
            Errors = true;
        } else {
            aciklamamHata.style.display = "none";
        }

        if (!dosyaEkle.value) {
            dosyaEkleHata.style.display = "block";
            Errors = true;
        } else {
            dosyaEkleHata.style.display = "none";
        }
    }

    // =====================
    // Blok Medya Alanları
    // =====================
    const medyaCountblok = document.querySelectorAll('[id^="blokmedyaTipi"]').length;
    for (let i = 0; i < medyaCountblok; i++) {
        const blokmedyaTipi = document.getElementById(`blokmedyaTipi${i}`);
        const bloksira = document.getElementById(`bloksira${i}`);
        const blokaltMetin = document.getElementById(`blokmedyaustmetin${i}`);
        const blokaciklama = document.getElementById(`blokmedyaAciklama${i}`);
        const blokdosyaEkle = document.getElementById(`blokdosyaEkle${i}`);

        const blokmedyaTipiHata = document.getElementById(`blokMedyaTipiHata${i}`);
        const bloksiraHata = document.getElementById(`bloksiraHata${i}`);
        const blokaltMetinHata = document.getElementById(`blokmedyaustmetinHata${i}`);
        const blokaciklamamHata = document.getElementById(`blokmedyaAciklamaHata${i}`);
        const blokdosyaEkleHata = document.getElementById(`blokdosyaEkleHata${i}`);

        if (!blokmedyaTipi || !bloksira || !blokaltMetin || !blokaciklama || !blokdosyaEkle) continue;

        if (blokmedyaTipi.value === "0") {
            blokmedyaTipiHata.style.display = "block";
            Errors = true;
        } else {
            blokmedyaTipiHata.style.display = "none";
        }

        if (!bloksira.value || bloksira.value === "0") {
            bloksiraHata.style.display = "block";
            Errors = true;
        } else {
            bloksiraHata.style.display = "none";
        }

        if (!blokaltMetin.value.trim()) {
            blokaltMetinHata.style.display = "block";
            Errors = true;
        } else {
            blokaltMetinHata.style.display = "none";
        }

        if (!blokaciklama.value.trim()) {
            blokaciklamamHata.style.display = "block";
            Errors = true;
        } else {
            blokaciklamamHata.style.display = "none";
        }

        if (!blokdosyaEkle.value) {
            blokdosyaEkleHata.style.display = "block";
            Errors = true;
        } else {
            blokdosyaEkleHata.style.display = "none";
        }
    }

    // =====================
    // Ürün Parametreleri
    // =====================
    const Marka = document.getElementById("MarkaKategori");
    const MarkaHata = document.getElementById("MarkaKategoriHata");

    const Model = document.getElementById("ModelKategori");
    const ModelHata = document.getElementById("ModelKategoriHata");

    const Seri = document.getElementById("SeriKategori");
    const SeriHata = document.getElementById("SeriKategoriHata");

    const Hiz = document.getElementById("HizKategori");
    const HizHata = document.getElementById("HizKategoriHata");

    const Hacim = document.getElementById("HacimKategori");
    const HacimHata = document.getElementById("HacimKategoriHata");

    const Olcum = document.getElementById("OlcumKategori");
    const OlcumHata = document.getElementById("OlcumKategoriHata");

    const BirimHiz = document.getElementById("HizBirim");
    const BirimHacim = document.getElementById("HacimBirim");
    const BirimOlcum = document.getElementById("OlcumBirim");

    if (!Marka.value || Marka.value === "0") {
        MarkaHata.style.display = "block";
        Errors = true;
    } else {
        MarkaHata.style.display = "none";
    }

    if (!Model.value || Model.value === "0") {
        ModelHata.style.display = "block";
        Errors = true;
    } else {
        ModelHata.style.display = "none";
    }

    if (!Seri.value || Seri.value === "0") {
        SeriHata.style.display = "block";
        Errors = true;
    } else {
        SeriHata.style.display = "none";
    }

    if (!Hiz.value || !BirimHiz.value) {
        HizHata.style.display = "block";
        Errors = true;
    } else {
        HizHata.style.display = "none";
    }

    if (!Hacim.value || !BirimHacim.value) {
        HacimHata.style.display = "block";
        Errors = true;
    } else {
        HacimHata.style.display = "none";
    }

    if (!Olcum.value || !BirimOlcum.value) {
        OlcumHata.style.display = "block";
        Errors = true;
    } else {
        OlcumHata.style.display = "none";
    }

    return !Errors;
}
function UrunKaydet() {
    const loader = document.getElementById("loader");
    loader.style.display = "block"
    var hataVar = HataUyarilari();

    if (hataVar) {
        UrunModeliKaydet();
;
    }
    Errors = false;
}


