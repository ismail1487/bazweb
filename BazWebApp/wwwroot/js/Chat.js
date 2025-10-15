console.log("ChatJS is Loaded Begin");
const OFFLIE_MESSAGE_TAG = "chatMessages";
const OFFLIE_USER_TAG = "chatUsers";

const uid = document.getElementsByTagName("body")[0].getAttribute("data-id");
//const sessionId = '';// document.getElementById("site-id").value;
//const sessionName = '';// document.getElementById("site-name").value;
//const myUser = { id: uid, name: sessionName };
//const chatHubUrl = "http://localhost:51309/chatHub?sessionId=" + globalUserId;

var UnreadList = [];

$(document).ready(function () {
    console.log("ChatJS Doc Ready Begin");



    const sessionId = document.getElementById("site-id").value;
    const sessionName = document.getElementById("site-name").value;
    const myUser = { id: uid, name: sessionName };
    const chatHubUrl = $("#site-urlSocket").val() + "/chatHub?sessionId=" + sessionId;
    var globalUserId = $('body').data('id'); // session ID ile bağlantı kuruyordu.
    let chatHubConnected = false;

    //const chatHubUrl = "http://localhost:51309/chatHub?sessionId=" + globalUserId;


    const chatConnection = new signalR.HubConnectionBuilder().withUrl(chatHubUrl).configureLogging(signalR.LogLevel.Information).build();

    chatConnection.serverTimeoutInMilliseconds = 1000 * 60 * 120; // 1 second * 60 * 1 = 1 minutes.
    chatConnection.keepAliveIntervalInMilliseconds = 1000 * 60 * 60; // 1 second * 60 * 1 = 1 minutes.
    chatConnection.on("UpdateUsers", function (list) {
        pushOfflineUsers(list);
        updateUserList(list);
        GetUnreadedMessages();
    });

    //Event: UpdateKurumUserList
    chatConnection.on("UpdateKurumUserList", function (list) {
        var userlist = $('#chat-user-list');
        userlist.children().remove();
        var html = '';
        for (var i = 0; i < list.length; i++) {
            console.log(list[i]);
            if (list[i].Id != globalUserId) {
                html += chatUserItem(list[i]);
            }
        }
        userlist.html(html);
        playSound();
        SetUserSelectEvent();
    });

    //Event: ReceiveMessage
    chatConnection.on("ReceiveMessage", function (fromUser, message) {

        console.log("Mesaj geldi");
        console.log(fromUser);
        console.log(message);
        var div = document.getElementById("imessages-" + fromUser.Id);
        if (div == undefined) {
            GetUnreadedMessages();
            playSound();
            // Bu durumda userın olduğu yere bir baget atacağız ve okunmadı sayısını attıracağız.
            return false;
        }
        $(div).append(messaageLeft({ message, user: fromUser }));
        var vb = document.getElementsByClassName('direct-chat-messages')[0];
        $(vb).animate({ scrollTop: vb.scrollHeight }, 500, 'swing', function () { });
        chatConnection.invoke("wasSeen", message.id);

    });

    chatConnection.on("SuccessMessage", function (toUser, message) {
        console.log(toUser);
        console.log(message);
        appendMyMessage(toUser, message);
        $("#message-box-text").val('');
    });
    chatConnection.on("ReceiveHistory", function (toUser, messages) {
        ReceiveHistory(toUser, messages);
    });
    chatConnection.on("WasSeenReceive", function (id) {
        $("#message-" + id + " i").addClass("fa-check-double");
    });


    function UnreadMessageRender() {
        console.log("UnreadMessageRender Begin");

        var totalCount = 0;
        for (var i = 0; i < UnreadList.length; i++) {
            totalCount += UnreadList[i].Count;
            var badge = $("#ur-count-" + UnreadList[i].Id);
            $(badge).html(UnreadList[i].Count);
            if (Number(UnreadList[i].Count) > 0) {

                $(badge).show();
            } else {
                $(badge).hide();
            }
        }
        $('#notifiCountSpan').html(totalCount);
        totalCount > 0 ? $('#notifiCountSpan').show() : $('#notifiCountSpan').hide();
        //SetUserMessageCount(3089, 0);
        console.log("UnreadMessageRender End");
    }

    chatConnection.on("ReceiveUnreadedMessages", function (list) {
        console.log("ReceiveUnreadedMessages Begin");
        UnreadList = list;
        UnreadMessageRender();
        var html = "";
        var totalCount = 0
        for (var i = 0; i < list.length; i++) {
            if (list[i].Id != uid) {
                // Liste update
                totalCount += list[i].Count;
                //html += unreadMesage(list[i]);
            }
        }

        console.log("ReceiveUnreadedMessages End");
        if (html == "") {
            html += siteLang.Yenibirmesajınızbulunmamaktadır;
            $("#chatUnreadMessages").html(html);
        }
        if (totalCount == 0)
            $("#chatUnreadDivCount").css("display", "none");
        else
            $("#chatUnreadDivCount").css("display", "block");

        $("#chatUnreadMessages").html(html);
        $("#chatUnreadMessagesCount").html(totalCount);


    });

    async function chatConnectionstart() {
        chatHubConnected = false;

        try {
            await chatConnection.start();
            chatHubConnected = true;
            CheckOfflineMessages();
        } catch (err) {
            setTimeout(chatConnectionstart, 5000);
            updateUserList(await getOfflineUsers());
        }
        chatConnection.invoke("GetKurumUserList");
    };
    chatConnection.onclose(chatConnectionstart);


    console.log("ChatJS Doc Ready End");

    function chatUserItem(obj) {
        var m =
            `
            <li class="nav-item" data-id="${obj.Id}" data-name="${obj.Name}">
                <a href="#" class="nav-link text-dark" data-id="${obj.Id}" data-name="${obj.Name}">
                <strong class="ur-span mr-2">
                <span title="Yeni Mesaj" class="badge badge-${(obj.IsOnline ? "warning" : "danger")} ur-baget" id="ur-count-${obj.Id}">0</span>
                <i class="fas fa-user mr-1 text-${(obj.IsOnline ? "warning" : "danger")}"></i></strong>
                ${obj.Name}<!-- <span class="float-right badge bg-primary small">1</span> -->
                    <span class="float-right text-sm text-${(obj.IsOnline ? "warning" : "danger")}"><i class="fas fa-signal"></i> </span>
                </a>
            </li>
            `;
        return m;
    }

    function OpenChatArea(to, name) {
        $('#message-send-btn').unbind('click');
        $("#message-box-text").unbind('keyup');
        $('#message-container').empty();
        var m = `
            <div class="card direct-chat-primary">
                    <div class="card-header bg-dark ui-sortable-handle" style="cursor: move;">
                        <h3 class="card-title">${name} - Mesajlar</h3>
                    </div>
                    <div class="card-body">
                        <div class="direct-chat-messages">
                            <div class="imessages" id="imessages-${to.id}">

                          </div>
                        </div>
                    </div>
                    <div class="card-footer">
                        
                            <div class="input-group">
                                <button class="btn" id="fileUploadBtn"><i class="fas fa-paperclip"></i></button>
                                <input type="text" id="message-box-text" name="message-box" placeholder="Type Message ..." class="form-control" autocomplete="off">
                                <span class="input-group-append">
                                    <button id="message-send-btn" type="button" class="btn btn-primary">Send</button>
                                </span>
                            </div>
                        
                    </div>

                </div>`;
        $('#message-container').append(m);
        $("#message-send-btn").on('click', (e) => {
            var Mesaj = $("#message-box-text").val();
            SendMessage(to.id, Mesaj);
        });

        $("#message-box-text").on('keyup', function (e) {
            if (e.key === 'Enter' || e.keyCode === 13) {
                var Mesaj = $("#message-box-text").val();
                SendMessage(to.id, Mesaj);
            }
        });
        $("#fileUploadBtn").on("click", (e) => {
            $("#uploadFileControl").click();
            $("#uploadFileControl").on("change", function () {
                if (document.getElementById("uploadFileControl").files.length) {

                    var fd = new FormData();
                    var files = $('#uploadFileControl')[0].files[0];
                    fd.append('file', files);
                    $.ajax({
                        url: "/panel/resimyukle/",
                        type: 'post',
                        data: fd,
                        contentType: false,
                        processData: false,
                        success: function (response) {

                            if (response.isSuccess && response.value) {
                                var data = response.value;
                                console.log(data.tabloID);
                                console.log(data.medyaAdi);
                                SendMessage(to.id, data.medyaAdi);

                            } else {
                                toastr.error("Dosya gönderilemedi");
                            }

                            console.log(response);
                            /*
                            var data = JSON.parse(response);
                            if (data.status) {

                                

                            } else {
                                $("#EkleModal").modal('hide');
                                toastr.error(data.message);
                            } */
                        }
                    });

                };
            });
        });



        GetHistory(to.id);
    };

    function AppendChatArea(to, name) {
        if ($("#chat-" + to).length) {
            $("#chat-" + to).removeAttr("style")
            if ($("#chat-" + to + ".collapsed-card").length) {
                $("#chat-" + to + " [data-card-widget='collapse']").click();
            }
            return;
        }

        var m = `
        <div id="chat-${to}" to="${to}" class="card direct-chat direct-chat-danger col-lg-3">
          <div class="card-header">
            <h3 class="card-title">${name}</h3>
            <div class="card-tools">
              <button type="button" class="btn btn-tool" data-card-widget="collapse"><i class="fas fa-minus"></i></button>
              <button type="button" class="btn btn-tool" data-card-widget="remove"><i class="fas fa-times"></i>
              </button>
            </div>
          </div>
          <div class="card-body">
            <div id="messages-${to}" class="direct-chat-messages">

            </div>
          </div>
          <div class="card-footer">
              <div class="input-group">
                <input type="text" id="message-text-${to}" name="message" placeholder="..." class="form-control msg-area">
                <span class="input-group-append">
                  <button type="button" class="btn btn-info upload-doc"><i class="fas fa-paperclip"></i></button>
                  <button type="button" class="btn btn-danger send-message"><i class="fas fa-rocket"></i></button>
                </span>
              </div>
          </div>
          <input type="file" hidden="hidden" class="msgdocupload">
        </div>
        `

        $("#chat-container").append(m)
        GetHistory(to);
    }


    /*
    function messageStatus(status) {
        var s = "clock";
        if (status === true) {
            s = "check-double"
        } else if (status === false) {
            s = "check"
        }
        return s;
    }
    */
    /*
    function unreadMesage(obj) {
        var m = `
    <a class="dropdown-item chatuser" data-id="${obj.Id}" data-name="${obj.Name}">
      <div class="media">
        <div class="media-body">
          <h3 class="dropdown-item-title">
            ${obj.Name}
            <span class="float-right text-sm text-danger">${obj.Count} <i class="far fa-comment"></i></span>
          </h3>
          <p class="text-sm text-muted"><i class="far fa-clock mr-1"></i>${obj.LastMessageDate}</p>
        </div>
      </div>
    </a>
    `
        return m;
    }
    */
    /*
    function chatUser(obj) {
        var m = `
    <a class="dropdown-item chatuser" data-id="${obj.Id}" data-name="${obj.Name}">
        <div class="media">
            <div class="media-body">
                <h3 class="dropdown-item-title">
                    ${obj.Name}
                    <span class="float-right text-sm text-${(obj.IsOnline ? "success" : "danger")}"><i class="fas fa-signal"></i></span>
                </h3>
            </div>
        </div>
    </a>
    `
        return m;
    }
    */
    async function GetUnreadedMessages() {
        console.log("GetUnreadedMessages function")
        if (chatHubConnected) {
            console.log("GetUnreadedMessages function Request")
            chatConnection.invoke("GetUnreadedMessages");

        }
        console.log("GetUnreadedMessages not run")
    }
    async function GetHistory(toId) {
        if (chatHubConnected) {
            console.log("GetHistory Request begin");
            console.log(toId);
            chatConnection.invoke("getHistory", Number(toId));
        } else {
            var messages = await getOfflineMessages();
            var toMessages = messages.filter(o => o.to == toId)
            if (toMessages.length) {
                var toUser = { id: toMessages[0].to }
                var messages = toMessages.map(o => ({ text: o.message, fromID: uid, date: "" }));
                ReceiveHistory(toUser, messages);
            }
        }
    }
    function appendMyMessage(toUser, message) {
        var div = document.getElementById("imessages-" + toUser.Id);
        $(div).append(messageRight({ message, user: myUser }))

        var vb = document.getElementsByClassName('direct-chat-messages')[0];
        $(vb).animate({ scrollTop: vb.scrollHeight }, 500, 'swing', function () { });

        //$(div).append(function ({ message, user: myUser }) {
        //    var ms = `<p class="from-me">${obj.message.text}<span class="saat-span">${obj.message.date}</span></p>`;
        //    return ms;
        //});
    }
    function appendMyMessageA(toUser, message) {
        //var div = document.getElementById("messages-" + toUser.id);
        $(div).append(messageRight({ message, user: myUser }))
        div.scrollTop = div.scrollHeight;
        $("#message-text-" + toUser.id).val("")
        //LSTM
    }

    async function ReceiveHistory(toUser, messages) {
        console.log("RECEIVE HISTORY");
        var div = document.getElementById("imessages-" + toUser.Id);
        for (var i = 0; i < messages.length; i++) {
            var message = messages[i];
            if (message.fromID == uid) {
                $(div).append(messageRight({ message, user: myUser }))
            } else {
                $(div).append(messaageLeft({ message, user: toUser }))
            }
            chatConnection.invoke("wasSeen", message.id);
        }
        //div.scrollTop = div.scrollHeight;
        var vb = document.getElementsByClassName('direct-chat-messages')[0];
        $(vb).animate({ scrollTop: vb.scrollHeight }, 500, 'swing', function () { });
        ilkmesaj = true;
        ilkmesaj1 = true;
    }

    function playSound() {
        var audio = document.getElementById("notificationSound");
        audio.muted = false;
        audio.play();
    }
    async function pushOfflineMessage(to, message) {
        var list = await getOfflineMessages()
        list.push({ to, message })
        OfflineCache.put(OFFLIE_MESSAGE_TAG, JSON.stringify(list));
    }

    async function getOfflineMessages() {
        var j = await OfflineCache.get(OFFLIE_MESSAGE_TAG);
        return j || [];
    }

    async function deleteOfflineMessages() {
        await OfflineCache.delete(OFFLIE_MESSAGE_TAG);
    }

    async function pushOfflineUsers(list) {
        OfflineCache.put(OFFLIE_USER_TAG, JSON.stringify(list));
    }

    async function getOfflineUsers() {
        var j = await OfflineCache.get(OFFLIE_USER_TAG);
        return j || [];
    }



    async function CheckOfflineMessages() {
        var list = await getOfflineMessages();
        if (list) {
            deleteOfflineMessages();
            for (var i = 0; i < list.length; i++) {
                SendMessage(list[i].to, list[i].message);
            }
            var toList = list.map(o => o.to).uniqueObjects();
            for (var i = 0; i < toList.length; i++) {
                if ($("#messages-" + toList[i]).length) {
                    GetHistory(toList[i]);
                }
            }
        }
    }

    async function SendMessage(to, message) {
        if (message == "") {
            return;
        }
        message = Linkify(message);
        //-- Buradan aşağısını değiştirebilirsin --*/
        if (chatHubConnected) {
            chatConnection.invoke("SendMessage", Number(to), message);
        }


        /*
        if (chatHubConnected) {
            console.log(message);
            chatConnection.invoke("sendMessage", to, message);
        } else {
            await pushOfflineMessage(to, message);
            var toUser = { id: to }
            var message = { text: message, date: "" };
            appendMyMessage(toUser, message);
        } */
    }

    async function updateUserList(list) {
        /*
        var html = "";
        var onlineCount = 0
        for (var i = 0; i < list.length; i++) {
            if (list[i].Id != uid) {
                if (list[i].IsOnline) {
                    ++onlineCount
                }
                html += chatUser(list[i]);
            }
        }
        if (onlineCount == 0) {
            $("#usercountDiv").css("display", "none");
        } else {
            $("#usercountDiv").css("display", "block");
        }
        $("#chatUsers").html(html);
        $("#chatUsersCount").html(onlineCount);
        userSearchRender(); */
    }



    function userSearchRender() {
        /*
        var text = $(".search-user").val().toLowerCase();
        $(".chatuser").removeAttr("hidden");
        if (text != "") {
            $(".chatuser").each(function () {
                var name = $(this).data("name").toLowerCase();
                if (!name.includes(text)) {
                    $(this).attr("hidden", "");
                }
            })
        } 
        */
    }
    /*
    $(document).on("click", ".send-message", function () {
        var btn = this;
        var to = parseInt($(btn).closest("[to]").attr("to"));
        var message = $("#message-text-" + to).val();
        SendMessage(to, message);
    });
    
    $(document).on('keyup', ".msg-area", function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            var btn = this;
            var to = parseInt($(btn).closest("[to]").attr("to"));
            var message = $("#message-text-" + to).val();
            SendMessage(to, message);
        }
    });
    */
    /*
    $(document).on("click", ".chatuser", function () {
        var btn = this;
        var id = $(btn).data("id")
        var name = $(btn).data("name")
        AppendChatArea(id, name)
    })
    */
    /*
    $(".search-user").on("keyup", function () {
        userSearchRender();
    })

    $(document).on("click", ".upload-doc", function () {
        $('.msgdocupload').click();
    });
    $(document).on("change", ".msgdocupload", function () {
        var btn = this;
        var to = parseInt($(btn).closest("[to]").attr("to"));
        uploadDocument(to, btn);
    });
    function sendDocument() {
    }
    function uploadDocument(to, e) {
        var formData = new FormData();
        var elem = $(e)[0];
        var file = elem.files[0];
        formData.append("file", file);
        $.ajax({
            url: "/panel/resimyukle/",
            processData: false,
            contentType: false,
            type: "POST",
            data: formData,
            success: function (result) {
                if (result.isSuccess) {
                    var link = result.value.medyaUrl;
                    let message = $("#site-urlMedya").val() + link;
                    SendMessage(to, message);
                } else {
                    alertim.toast(siteLang.Hata, "error");
                }
            },
            error: function (e) {
                alertim.toast(siteLang.Hata, "error");
            }
        });
    } */





    /* MESAJ ARAYÜZ FONKSIYONLARI */
    var sayac = 0;
    var ilkmesaj = true;
    function messaageLeft(obj) {
        sayac = 1;
        var oldm = `
        <div class="direct-chat-msg">
          <div class="direct-chat-infos clearfix">
            <span class="direct-chat-name float-left">${obj.user.name}</span>
            <span class="direct-chat-timestamp float-right">${obj.message.date}</span>
          </div>
          <img class="direct-chat-img" src="/images/blankprofile.png">
          <div id="message-${obj.message.id}" class="direct-chat-text">
            ${obj.message.text}
          </div>
        </div>
        `
        console.log(obj);

        var m = '';
        if (ilkmesaj) {
            m = `<p class="from-them" id="message-${obj.message.id}"> <span class="profile-span-left text-muted"><i class="fas fa-user mr-1 text-sm""></i>${obj.user.Name}</span>${obj.message.text} <span class="saat-span-left text-muted"><i class="far fa-clock mr-1"></i>${obj.message.date}</span></p>`;
            ilkmesaj = false;
            sayacAlici = 0;
        } else {
            if (sayacAlici > 0) {
                sayacAlici = 0;
                m = `<p class="from-them" id="message-${obj.message.id}"> <span class="profile-span-left text-muted"><i class="fas fa-user mr-1 text-sm""></i>${obj.user.Name}</span>${obj.message.text} <span class="saat-span-left text-muted"><i class="far fa-clock mr-1"></i>${obj.message.date}</span></p>`;

            } else if (sayacAlici == 0) {
                m = `<p class="from-them" id="message-${obj.message.id}">${obj.message.text}<span class="saat-span-left text-muted"><i class="far fa-clock mr-1"></i>${obj.message.date}</span></p>`;
            }
        }

        return m;
    }
    var sayacAlici = 0;
    var ilkmesaj1 = true;

    function messageRight(obj) {
        // var status = messageStatus(obj.message.wasSeen);
        sayacAlici = 1;
        var oldm = `
        <div class="direct-chat-msg right">
          <div class="direct-chat-infos clearfix">
            <span class="direct-chat-name float-right">${obj.user.name}</span>
            <span class="direct-chat-timestamp float-left">${obj.message.date}</span>
          </div>
          <img class="direct-chat-img" src="/images/blankprofile.png">
          <div id="message-${obj.message.id}" class="direct-chat-text">
            <i class="fas fa-${status} chat-status"></i>
            ${obj.message.text}
          </div>
        </div>
        `;
        var m = '';
        if (ilkmesaj1) {
            m = `<p class="from-me" id="message-${obj.message.id}"><span class="profile-span text-muted"><i class="fas fa-user mr-1 text-sm"></i><strong class="small"></strong>${obj.user.name}</span>${obj.message.text}<span class="saat-span text-muted"><i class="far fa-clock mr-1"></i>${obj.message.date}</span></p>`;
            ilkmesaj1 = false;
            sayac = 0;
        } else {
            if (sayac > 0) {
                sayac = 0;
                m = `<p class="from-me" id="message-${obj.message.id}"><span class="profile-span text-muted"><i class="fas fa-user mr-1 text-sm"></i><strong class="small"></strong>${obj.user.name}</span>${obj.message.text}<span class="saat-span text-muted"><i class="far fa-clock mr-1"></i>${obj.message.date}</span></p>`;
            } else if (sayac == 0) {
                m = `<p class="from-me" id="message-${obj.message.id}">${obj.message.text}<span class="saat-span text-muted"><i class="far fa-clock mr-1"></i>${obj.message.date}</span></p>`;
            }
        }
        
        return m;
        //<i class="fas fa-user mr-1 text-sm"></i><strong class="small">${obj.user.name}</strong><br/>
    }
    function SetUserMessageCount(BadgeUserId, Count) {
        var badge = $("#ur-count-" + BadgeUserId);
        $(badge).html(Count > 0 ? Count : "");
        Count > 0 ? $(badge).show() : $(badge).hide();
    }
    function SetUserSelectEvent() {
        $("#chat-user-list a").on('click', function (e) {
            SetUserMessageCount($(e.currentTarget).attr("data-id"), 0);
            OpenChatArea(
                {
                    id: $(e.currentTarget).attr("data-id"),
                    name: $(e.currentTarget).attr("data-name")
                }
                , $(e.currentTarget).attr("data-name"));
        });
    }

    chatConnectionstart();

}); // Doc Ready End

console.log("ChatJS is Loaded End");
