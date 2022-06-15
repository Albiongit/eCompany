var timeOutId = 0;
//we define our function and STORE it in a var

//$(document).ready(function () {

//    ajaxFn();
//});

 function ajaxFn() {
    $.ajax({
        url: '/Customer/Task/CheckTasks',
        type: 'POST',
        success: function (response) {
            if (response.taskCount > 0) {//YAYA
                $("#taskCount").html("<sup>" + response.taskCount+"</sup>");
                timeOutId = setTimeout(ajaxFn, 30000);//stop the timeout
            } else {//Fail check?
                timeOutId = setTimeout(ajaxFn, 30000);//set the timeout again
            }
        }
    });
};


timeOutId = setTimeout(ajaxFn, 0);