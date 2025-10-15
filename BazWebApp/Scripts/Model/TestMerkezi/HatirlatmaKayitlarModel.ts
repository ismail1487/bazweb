class HatirlatmaKayitlarModel extends kendo.data.Model {
    constructor(value?: any) {
        super(value)
    }
    fields = {
        HatirlatmaZamani: {
            type: "date",
            defaultValue: "",
            validation: {
                required: true
            }
        },
        HatirlatmaMetni: {
            type: "string",
            defaultValue: "",
            validation: {
                required: true
            }
        },
        HatirlatmaEpostaYollayacakMi: {
            type: "boolean",
            defaultValue: false
        },
        HatirlatmaSmsyollayacakMi: {
            type: "boolean",
            defaultValue: false
        },
        HatirlatmaTipi: {
            type: "string",
            defaultValue: ""
        }
    }
}