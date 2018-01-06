let libs = require('./libs');

$(document).ready(function () {


    document.querySelector('.img__btn').addEventListener('click', function() {
        document.querySelector('.cont').classList.toggle('s--signup');
    });

    let registrBtn = $('.btn-registr');
    registrBtn.click(function () {
        $('.hint').val('');
        let appNameReg = $('.app-name-reg').val();
        $.ajax({
            url: 'https://configstorage-api.azurewebsites.net/api/Application/canRegister',
            type: 'POST',
            data: JSON.stringify({name: appNameReg}),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            success: function(data){
                console.log(data.canRegisterApplication);
                if (!data.canRegisterApplication){
                    $('.hint').val('This application name is already taken')
                } else {
                    getKey(appNameReg)
                }

            }
        });
    });
    function getKey(appName){
        $.ajax({
            url: 'https://configstorage-api.azurewebsites.net/api/Application/register',
            type: 'POST',
            data: JSON.stringify({name: appName}),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            success: function(data){
                if (data.code === 1){
                    console.log(data.applicationKey);
                    $('.group-key').css('display', 'block');
                    $('.app-key-reg').val(data.description);
                }
                else {
                    console.log(data.applicationKey);
                    $('.group-key').css('display', 'block');
                    $('.app-key-reg').val(data.applicationKey);
                }
            }
        });
    }

});
