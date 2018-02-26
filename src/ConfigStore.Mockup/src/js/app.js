let libs = require('./libs');

$(document).ready(function () {


    document.querySelector('.img__btn').addEventListener('click', function() {
        document.querySelector('.cont').classList.toggle('s--signup');
    });

    let timerId;

    $('.app-name-reg').bind('input',function(e){
        $('.done').css('display', 'none');
        $('.not-done').css('display', 'none');
        $('.load').css('display', 'block');
        clearTimeout(timerId);
        timerId = setTimeout(function() {
                console.log('hi');
                $.ajax({
                    url: 'https://configstorage-api.azurewebsites.net/api/Application/canRegister',
                    type: 'POST',
                    data: JSON.stringify({name: $('.app-name-reg').val()}),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    async: false,
                    success: function(data){
                        console.log(data.canRegisterApplication);
                        if (data.canRegisterApplication) {
                            $('.not-done').css('display', 'none');
                            $('.load').css('display', 'none');
                            $('.done').css('display', 'block')
                        } else {
                            $('.done').css('display', 'none');
                            $('.load').css('display', 'none');
                            $('.not-done').css('display', 'block')
                        }
                    }
                })
            }
            ,2000);
        });


    $('.btn-registr').click(function () {
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
        })
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
