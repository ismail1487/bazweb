class LoginService extends kendo.data.ObservableObject {
    _repository: Repository;
    constructor(value?: any) {
        super(value);
        super.init(this);
        this._repository = new Repository(ServiceConfiguration.CreateDefault("/panel"));
    }
    static initalize() {
        kendo.bind($(".form"), new LoginService());
    }
}
$(document).ready(function () {
    LoginService.initalize();
})