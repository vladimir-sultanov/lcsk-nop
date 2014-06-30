var myHub = $.connection.chatHub;

var LCSKChat = function () {
    var chatKey = 'lcsk-chatId';
    var requestChat = false;
    var chatId = '';
    var chatEditing = false;
    var isClientDataFilled = false;

    var options = [];

    var chatClientName = '';
    var chatClientEmail = '';

    var offlineEmailForm;
    var clientDataForm;
    var closeButton;

    function setDefaults() {
        options.position = 'fixed';
        options.placement = 'bottom-right';

        options.headerPaddings = '3px 10px 3px 10px';
        options.headerBackgroundColor = '#0376ee';
        options.headerTextColor = 'white';
        options.headerBorderColor = '#0354cb';
        options.headerGradientStart = '#058bf5';
        options.headerGradientEnd = '#015ee6';
        options.headerFontSize = '15px';

        options.headerTooltipClose = 'Close the chat';

        options.boxBorderColor = '#0376ee';
        options.boxBackgroundColor = '#fff';

        options.width = 450;

        //Ui options
        options.offlineTitle = 'Contact us!';
        options.onlineTitle = 'Chat with us!';
        options.waitingForOperator = 'Thanks, give us 1 minute to accept the chat...';
        options.emailSent = 'Your email was sent, thanks we\'ll get back to you asap.';
        options.emailFailed = 'Doh! The email could not be sent at the moment.<br /><br />Sorry about that.';
        options.buttonValueContactus = 'Contact us';
        options.buttonValueStartChat = 'Start chat';
        options.lableTextName = 'Name';
        options.lableTextEmail = 'Email';
        options.lableTextYouremail = 'Your email';
        options.lableTextYourMessage = 'Your message';
        options.infoTextHaveaQuestion = 'Have a question? Let\'s chat!';
        options.infoTextAddYourQuestion = 'Add your question on the field below and press ENTER.';
        options.infoTextContinuingYourChatWith = 'Continuing your chat with';
        options.infoTextYouAreNowChattingWith = 'You are now chatting with';

        options.recaptcharPublicKey = '';
        options.recaptcharElementId = 'recaptcha_div';
        options.reCaptchaTheme = '';

        options.captchaErrorMessage = 'Not valid captcha!';
        options.captchaRequiredMessage = 'This field is required!';

        

        options.chatClientIp = '';
        options.browserType = '';
        options.browserPlatform = '';

    }

    function config(args) {
        setDefaults();

        if (args != null) {
            for (key in options) {
                if (args[key]) {
                    options[key] = args[key];
                }
            }
        }
    }

    function getPlacement() {
        if (options.placement == 'bottom-right') {
            return 'bottom:0px;right:0px;';
        }
        return '';
    }

    function init() {
        
        closeButton = '<span id="close-chat" style="display:block;background-image: url(/Plugins/Widgets.LiveChat/Content/images/chat-sprite.png);background-repeat: no-repeat; vertical-align: middle; float: right; backgroud-position: 0 0; width: 25px; height: 25px;" data-title="' + options.headerTooltipClose + '">&nbsp;</span>';
        $('body').append(
            //'<div id="chat-box-header" style="display: block;position:' + options.position + ';' + getPlacement() + 'width:' + options.width + 'px;padding:' + options.headerPaddings + ';color:' + options.headerTextColor + ';font-size:' + options.headerFontSize + ';cursor:pointer;background:' + options.headerBackgroundColor + ';filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=\'' + options.headerGradientStart + '\', endColorstr=\'' + options.headerGradientEnd + '\');background: -webkit-gradient(linear, left top, left bottom, from(' + options.headerGradientStart + '), to(' + options.headerGradientEnd + '));background: -moz-linear-gradient(top,  ' + options.headerGradientStart + ',  ' + options.headerGradientEnd + ');border:1px solid ' + options.headerBorderColor + ';box-shadow:inset 0 0 7px #0354cb;-webkit-box-shadow:inset 0 0 7px #0354cb;border-radius: 5px;">' + options.offlineTitle + '</div>' +
            //'<div id="chat-box" style="display:none;position:' + options.position + ';' + getPlacement() + 'width:' + options.width + 'px;height:300px;padding: 10px 10px 10px 10px;border: 1px solid ' + options.boxBorderColor + ';background-color:' + options.boxBackgroundColor + ';font-size:small;"></div>'
            '<div id="chat-box-header" style="display: block;position:' + options.position + ';' + getPlacement() + 'width:' + options.width + 'px;padding:' + options.headerPaddings + ';color:' + options.headerTextColor + ';font-size:' + options.headerFontSize + ';cursor:pointer;background:' + options.headerBackgroundColor + ';filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=\'' + options.headerGradientStart + '\', endColorstr=\'' + options.headerGradientEnd + '\');background: -webkit-gradient(linear, left top, left bottom, from(' + options.headerGradientStart + '), to(' + options.headerGradientEnd + '));background: -moz-linear-gradient(top,  ' + options.headerGradientStart + ',  ' + options.headerGradientEnd + ');border:1px solid ' + options.headerBorderColor + ';box-shadow:inset 0 0 7px #0354cb;-webkit-box-shadow:inset 0 0 7px #0354cb;border-top-left-radius: 5px;border-top-right-radius: 5px;">' + options.offlineTitle + '</div>' +
            '<div id="chat-box" style="display:none;position:' + options.position + ';' + getPlacement() + 'width:' + options.width + 'px;height:300px;padding: 10px 10px 10px 10px;border: 1px solid ' + options.boxBorderColor + ';background-color:' + options.boxBackgroundColor + ';opacity: 0.8;font-size:14px !important;color: black !important;"></div>'
        );

        $.connection.hub.start()
            .done(function () {
                var existingChatId = getExistingChatId(chatKey);
                myHub.server.logVisit(document.location.href, document.referrer, existingChatId);
            })
            .fail(function () { chatRefreshState(false); });

        $('body').on({
            click: function () {
                toggleChatBox();
            }
        }, '#chat-box-header');

        $('#chat-box').on({
            keydown: function (e) {
                var msg = $(this).val();
                if (e.keyCode == 13 && msg != '') {
                    e.preventDefault();
                    e.stopPropagation();

                    if (chatId == null || chatId == '') {
                        var chatClient = {
                            Name: chatClientName,
                            ConnectionId: myHub.id,
                            ConnectedAt: new Date().toISOString(),
                            Email: chatClientEmail,
                            Browser: options.browserType,
                            OperatingSystem: options.browserPlatform,
                            Url: parent.window.location.href,
                            Ip: options.chatClientIp
                        };

                        myHub.server.requestChat(chatClient, msg);

                        $('#chat-box-msg').html(options.waitingForOperator);
                    } else {
                        myHub.server.send(msg);
                    }

                    $('#chat-box-textinput').val('');
                }
            }
        }, '#chat-box-textinput');

        $('#chat-box').on({
            keydown: function () {
                chatEditing = true;
            }
        }, '.chat-editing');

        $('#chat-box').on({
            click: function () {

                offlineEmailForm.validate();

                if (!offlineEmailForm.valid()) return;
                
                var toEmail = $('#chat_box_email').val();
                var message = $('#chat-box-cmt').val();

                jQuery.getJSON('/livechat/SendEmailMessage',
                     { toEmail: toEmail, message: message },
                     function (result) {
                         $('#chat-box').html(options.emailSent);                         
                     });
                chatEditing = false;
            }
        }, '#chat-box-send');
        $('#chat-box').on({
            click: function () {

                var clientFormValidator = clientDataForm.validate();

                if (!clientDataForm.valid()) return;

                if (!Recaptcha) return;

                var captchaRequest =
                    {
                        Challenge: Recaptcha.get_challenge(),
                        Response: Recaptcha.get_response()
                    }

                jQuery.getJSON('/livechat/ValidateCaptcha',                     
                     captchaRequest,
                     function (data) {
                         if (data.result)
                         {
                             var name = $('#chat_box_name').val();
                             var email = $('#chat_box_email').val();

                             chatClientName = name;
                             chatClientEmail = email;
                             isClientDataFilled = true;
                             $('#chat-box-input').show();
                             chatRefreshState(true);
                         }
                         else
                         {                             
                             Recaptcha.reload();
                             clientFormValidator.showErrors({
                                 "recaptcha_response_field": options.captchaErrorMessage
                             });
                         }
                     });
            }
        }, '#client-data-send');

        $('#chat-box-header').on({
            click: function () {
                myHub.server.leaveChat(getExistingChatId());
                isClientDataFilled = false;
                requestChat = false;
                chatRefreshState(true);
            }
        }, '#close-chat');
    }

    function chatRefreshState(state) {
        if (state) {
            //$('#chat-box-header').text(options.onlineTitle);
            $('#chat-box-header').html(options.onlineTitle + closeButton);
            if (!requestChat) {
                var dialogBox = 
                    '<form id="clientData-form" style="text-align: left;" method="POST" action="/livechat/ValidateCaptcha">' +
                    '<p>' + options.lableTextEmail + '</p><input id="chat_box_email" type="email" name="email" style="border:1px solid #0354cb;border-radius: 3px;width: 100%;" class="chat-editing" required/>' +
                    '<p>' + options.lableTextName + '</p><input id="chat_box_name" style="border:1px solid #0354cb;border-radius: 3px;width: 100%;" class="chat-editing" required/>' +
                    '<div id="' + options.recaptcharElementId + '"></div>' +
                    '<p><input type="button" id="client-data-send" value="' + options.buttonValueStartChat + '" />' +
                    '</form>';
                if (isClientDataFilled) {
                    dialogBox = '<div id="chat-box-msg" style="height:265px;overflow:auto;text-align: left;">' +
                    '<p>' + options.infoTextHaveaQuestion + '</p><p>' + options.infoTextAddYourQuestion + '</p></div>' +
                    '<div id="chat-box-input" style="height:35px;"><textarea id="chat-box-textinput" style="width:100%;height: 32px;border:1px solid #0354cb;border-radius: 3px;" /></div>'
                }

                $('#chat-box').html(dialogBox);

                if (!isClientDataFilled) {
                    $('#chat-box-input').hide();

                    if (Recaptcha) {
                        Recaptcha.create(options.recaptcharPublicKey,
                           options.recaptcharElementId,
                           {
                               theme: options.reCaptchaTheme,
                               callback: Recaptcha.focus_response_field
                           }
                         );
                    }

                    clientDataForm = $('#clientData-form');
                    clientDataForm.validate({
                        rules: {
                            chat_box_name: {
                                required: true,
                                text: true
                            },
                            chat_box_email: {
                                required: true,
                                email: true
                            },
                            recaptcha_response_field: {
                                required: true,
                            }
                            //messages: {
                            //    recaptcha_response_field:{
                            //            required: truoptions.captchaRequiredMessage,
                            //            captcha: options.captchaErrorMessage
                            //    }
                            //}
                        }
                    });
                    
                }
                
            }
        } else {
            if (!chatEditing) {
                $('#chat-box-header').text(options.offlineTitle);
                $('#chat-box-input').hide();
                $('#chat-box').html(
                    '<form id="offline-form">' +
                    '<p>' + options.lableTextYouremail + '</p><input id="chat_box_email" type="email" name="email" style="border:1px solid #0354cb;border-radius: 3px;width: 100%;" class="chat-editing" required/>' +
                    '<p>' + options.lableTextYourMessage + '</p><textarea id="chat-box-cmt" cols="40" rows="7" class="chat-editing" style="border:1px solid #0354cb;border-radius: 3px;"></textarea>' +
                    '<p><input type="button" id="chat-box-send" value="' + options.buttonValueContactus + '" />' +
                    '</form>'
                );
                offlineEmailForm = $('#offline-form');
                offlineEmailForm.validate({
                    rules: {
                        chat_box_email: {
                            required: true,
                            email: true
                        }
                    }
                });
            }
        }
    }

    function toggleChatBox() {
        var elm = $('#chat-box-header');
        if ($('#chat-box').hasClass('chat-open')) {
            $('#chat-box').removeClass('chat-open');
            elm.css('bottom', '0px');
        } else {
            var y = 301 + elm.height();
            $('#chat-box').addClass('chat-open');
            elm.css('bottom', y);
        }
        $('#chat-box').slideToggle();
    }

    function hasStorage() {
        return typeof(Storage) !== 'undefined';
    }

    function setChatId(chatId) {
        if (hasStorage()) {
            sessionStorage.setItem(chatKey, chatId);
        }
    }

    function getExistingChatId() {
        if (hasStorage()) {
            return sessionStorage.getItem(chatKey);
        }
    }

    myHub.client.setChat = function (id, agentName, existing) {
        chatId = id;
        requestChat = true;

        setChatId(chatId);

        if (existing) {
            if (!$('#chat-box').hasClass('chat-open')) {
                toggleChatBox();
            }
            $('#chat-box-msg').html('<p>' + options.infoTextContinuingYourChatWith + ' <strong>' + agentName + '</strong></p>');
        } else {
            $('#chat-box-msg').append('<p>' + options.infoTextYouAreNowChattingWith + ' <strong>' + agentName + '</strong></p>');
        }
    };

    myHub.client.addMessage = function (from, msg) {
        if (chatId != null && chatId != '') {
            if (!requestChat) {
                if (!$('#chat-box').hasClass('chat-open')) {
                    toggleChatBox();
                }
                requestChat = true;
            }

            $('#chat-box-msg').append('<p><strong>' + from + '</strong>: ' + msg + '</p>');
            if (from == '') {
                chatId = '';
                requestChat = false;
            }

            $("#chat-box-msg").scrollTop($("#chat-box-msg")[0].scrollHeight);
        }
    }

    myHub.client.emailResult = function (state) {
        if (!state) {
            $('#chat-box').html(options.emailFailed);
        }
    };

    myHub.client.onlineStatus = function (state) {
        chatRefreshState(state);
    };

    return {
        config: config,
        init: init
    }
} ();