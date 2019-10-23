var system = require('system');
var page = require('webpage').create();

page.open(system.args[1], function (status) {
    page.evaluate(function (loginName, password) {
        $('[data-target="#PaymentMethodBarion"]').click();
        $('#LoginName').val(loginName);
        $('#Password').val(password);
        $('button[type="submit"]').click();
    }, system.args[2], system.args[3]);

    setTimeout(function () {
        page.evaluate(function () {
            $('button[type="submit"]').click();
        });

        setTimeout(function () {
            page.render("after_submit.png");
            phantom.exit();
        }, 10000);
    }, 5000);
});
