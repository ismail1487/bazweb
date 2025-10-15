class DropDownModel {
    tabloID: number;
    tanim: string;
}
class DropDownModel2 extends kendo.data.ObservableArray{
    constructor(value?: any[]) {
        super(value)
        this.init(value);
    }
    tabloID: number;
    tanim: string;
}
