function Receive(text) {
    console.log(text);
}

// invoke = فراخوانی کردن
function SendMessage(event) {
    event.preventDefault();
    const text = document.getElementById("messageText").value;
    connection.invoke("SendMessage", text);
}

function AppendGroup(groupName, token, imageName) {
    if (groupName == "Error") {
        alert("ERROR");
    } else {
        $(".rooms #user-groups ul").append(`
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
                                         <li data-user-id='${data[i].token}'>
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