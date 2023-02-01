$(document).ready(function () {
    if (Notification.permission !== "granted") {
        Notification.requestPermission();
    }
});

var currentGroupId = 0;
var userId = 0;
var connection = new signalR.HubConnectionBuilder().withUrl("/Chat").build();
connection.on("Welcome",
    function (id) {
        userId = id;
    });
connection.on("ReceiveMessage", Receive);
connection.on("NewGroup", AppendGroup);
connection.on("JoinGroup", joined);
connection.on("ReceiveNotification", sendNotification);

async function Start() {
    try {
        await connection.start();
        $(".disConnect").hide();
    } catch (e) {
        $(".disConnect").show();
        setTimeout(Start, 6000);
    }
}

connection.onclose(Start);
Start();

function sendNotification(chat) {
    if (Notification.permission === "granted") {
        if (currentGroupId !== chat.groupId) {
            var notification = new Notification(chat.groupName,
                {
                    body: chat.chatBody
                });
        }
    }
}

function joined(group, chats) {
    $(".header").css("display", "block");
    $(".footer").css("display", "block");
    $(".header h2").html(group.groupTitle);
    $(".header img").attr("src", `/images/groups/${group.imageName}`);
    currentGroupId = group.id;
    $(".chats").html("");
    for (var i in chats) {
        var chat = chats[i];
        if (userId == chat.userId) {
            $(".chats").append(`
                                    <div class="chat-me">
                                        <div class="chat">
                                        <span>${chat.userName}</span>
                                                    <p>${chat.chatBody}</p>
                                            <span>${chat.creationDate}</span>
                                        </div>
                                    </div>
                            `);
        } else {
            $(".chats").append(`
                                            <div class="chat-you">
                                                <div class="chat">
                                                        <span>${chat.userName}</span>
                                                                     <p>${chat.chatBody}</p>
                                                    <span>${chat.creationDate}</span>
                                                </div>
                                            </div>
                                    `);
        }
    }
}

function Receive(chat) {
    if (userId === chat.userId) {
        $(".chats").append(`
                                    <div class="chat-me">
                                        <div class="chat">
                                                <span>${chat.userName}</span>
                                                    <p>${chat.chatBody}</p>
                                            <span>${chat.creationDate}</span>
                                        </div>
                                    </div>
                            `);
    } else {
        $(".chats").append(`
                                            <div class="chat-you">
                                                <div class="chat">
                                                        <span>${chat.userName}</span>
                                                             <p>${chat.chatBody}</p>
                                                    <span>${chat.creationDate}</span>
                                                </div>
                                            </div>
                                    `);
    }
}

// invoke = فراخوانی کردن
function SendMessage(event) {
    event.preventDefault();
    const textInput = document.getElementById("messageText");
    if (textInput.value) {
        connection.invoke("SendMessage", textInput.value, currentGroupId);
        textInput.value = null;
    } else {
        alert("Error");
    }
}

function AppendGroup(groupName, token, imageName) {
    if (groupName == "Error") {
        alert("ERROR");
    } else {
        $(".rooms #user_groups ul").append(`
                     <li onclick="joinInGroup('${token}')">
                    ${groupName}
                    <img src="/images/groups/${imageName}" />
                    <span></span>
                </li>
                    `);
        $("#exampleModal").modal({show:false});
        document.getElementById("groupName").value = null;
    }
}

function insertGroup(event) {
    event.preventDefault();
    const groupName = event.target[0].value;
    const imageFile = event.target[1].files[0];
    var formData = new FormData();
    formData.append("GroupName", groupName);
    formData.append("ImageFile", imageFile);
    $.ajax({
        url: "/Home/CreateGroup",
        type: "post",
        data: formData,
        encyType: "multipart/form-data",
        processData: false,
        contentType: false
    });
}

function search() {
    const text = $("#search_input").val();
    if (text) {
        $("#search_result").show();
        $("#user_groups").hide();
        $.ajax({
            url: "/home/search?title=" + text,
            type: "get"
        }).done(function (data) {
            $("#search_result ul").html("");
            for (var i in data) {
                if (data[i].isUser) {
                    $("#search_result ul").append(`
                                         <li onclick="joinInPrivateGroup(${data[i].token})">
                                                    ${data[i].title}
                                                    <img src="/img/${data[i].imageName}" />
                                                    <span></span>
                                                </li>
                                `);
                } else {
                    $("#search_result ul").append(`
                                         <li onclick="joinInGroup('${data[i].token}')">
                                                    ${data[i].title}
                                                    <img src="/images/groups/${data[i].imageName}" />
                                                    <span></span>
                                                </li>

                                `);
                }
            }

        });
    } else {
        $("#search_result").hide();
        $("#user_groups").show();
    }
}

function joinInGroup(token) {
    connection.invoke("JoinGroup", token, currentGroupId);
}

function joinInPrivateGroup(receiverId) {
    connection.invoke("JoinPrivateGroup", receiverId, currentGroupId);
}