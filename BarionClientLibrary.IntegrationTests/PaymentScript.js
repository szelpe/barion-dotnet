var system = require('system');
var page = require('webpage').create();

page.open(system.args[1], function (status) {
    page.evaluate(function (loginName, password) {
        $('#openPayWithBarion').click();
        $('#LoginName').val(loginName);
        $('#Password').val(password);
        $('#loginBtn').click();
    }, system.args[2], system.args[3]);

    setTimeout(function () {
        page.evaluate(function () {
            $('.payWithRegisteredCard').click();
        });

        setTimeout(function () {
            page.render("after_submit.png");
            phantom.exit();
        }, 8000);
    }, 3000);
});
