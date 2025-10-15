/// <reference path="../model/messagerecordssearchmodel.ts" />

class MessageRecordsSearchService extends kendo.data.ObservableObject {
    _repository: Repository;
    constructor(value?: any) {
        super(value);
        super.init(this);
        this._repository = new Repository(ServiceConfiguration.CreateDefault(""));
    }

    data = new MessageRecordsSearchModel({
        MesajIcerigi: "",
        GonderimZamani: "",
        GonderenKisiId: 0,
        GonderenKisiAdi: "",
        HedefKisiId: 0,
        HedefKisiAdi: "",
        MesajKaynagiTipi: ""
    });
    Search() {
        let query = $('#query').val();
        var conf = AjaxConfiguration.getDafault();
        conf.url = "/panel/MesajlarIcindeArama/" + query;
        conf.async = false;
        conf.dataType = "json";
        conf.contentType = "application/json; charset=utf-8"
        conf.success = function (result) {
            if (result.value) {
                messageRecordsSearchService.SonucDoldur(result.value);
            } else {
                $('#uygulamaMesaj').html(noResult);
            }
            $('#query').val("");
        };
        conf.error = function (e) {
            $('#query').val("");
            errorHandler(e);
        }
        this._repository.getData(null, conf);
    }
    initialize() {
        $(document).on('keyup', "#query", function (e) {
            if (e.key === 'Enter' || e.keyCode === 13) {
                messageRecordsSearchService.Search();
            }
        });
        kendo.bind($(".model-bind"), messageRecordsSearchService);
    }
    SonucDoldur(list) {
        var uygulamaMsj = "";
        var uygulamaList = [];
        uygulamaList = list.filter(a => a.mesajKaynagiTipi == "uygulama");
        for (var item of uygulamaList) {
            uygulamaMsj += `<div class="row col-md-12 mt-4 message-row">
                            <div class="row col-md-12" style="justify-content: space-between;">
                                <div class="col-md-4">
                                    <div class="txt-nomargin">${siteLang.Gönderen}:</div>
                                    <div class="Txt13-nomargin">${item.gonderenKisiAdi}</div>
                                </div>
                                <div class="col-md-4">
                                    <div class="txt-nomargin">${siteLang.HedefKişi}:</div>
                                    <div class="Txt13-nomargin">${item.hedefKisiAdi}</div>
                                </div>
                            </div>
                            <div class="row col-md-12" style="justify-content: space-between;">
                                <div class="col-md-8">
                                    <div class="txt-nomargin">${siteLang.Mesaj}:</div>
                                    <div class="Txt13-nomargin">${item.mesajIcerigi}</div>
                                </div>
                                <div class="col-md-4">
                                    <div class="txt-nomargin">${siteLang.Tarih}:</div>
                                    <div class="Txt13-nomargin">${CSStringDateToStringddmmyyyyhhmm(item.gonderimZamani)}</div>
                                </div>
                            </div>
                        </div>`;
        }
        if (uygulamaList.length == 0) {
            uygulamaMsj = noResult;
        }
        $('#uygulamaMesaj').html(uygulamaMsj);
    }
}
var messageRecordsSearchService = new MessageRecordsSearchService();
var CSStringDateToStringddmmyyyyhhmm: any;
var noResult = `<div class="row col-md-12">
                                <div class="col-md-12">
                                    <div class="txt-nomargin">${siteLang.Aramanızadairsonuçbulunamamıştır}</div>
                                </div>
                            </div>`;