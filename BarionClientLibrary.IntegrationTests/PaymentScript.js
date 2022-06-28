var system = require('system');
var page = require('webpage').create();

page.open(system.args[1], function (status) {
    page.evaluate(function (loginName, password) {
        document.querySelector('[data-target="#PaymentMethodBarion"]').click();
        document.getElementById('LoginName').value = loginName;
        document.getElementById('Password').value = password;
        document.querySelector('button[type="submit"]').click();
    }, system.args[2], system.args[3]);

    setTimeout(function () {
        page.evaluate(function () {
            document.getElementById('StartPayment').click();
        });

        setTimeout(function () {
            page.render("after_submit.png");
            phantom.exit();
        }, 10000);
    }, 5000);
});
