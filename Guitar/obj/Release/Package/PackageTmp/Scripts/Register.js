window.onload = function () {

    //短信验证码  
    var InterValObj; //timer变量，控制时间    
    var count = 60; //间隔函数，1秒执行    
    var curCount;//当前剩余秒数    
    var code = ""; //验证码    
    var codeLength = 6;//验证码长度   

    $("#getcode").click(function () {

        //获取输入的手机号码
        var phoNum = $("#phonum").val();
        //alert(phoNum);
        curCount = count;

        //用正则表达式验证手机号是否合法
        //var re = /(^1[3|5|8][0-9]{9}$)/;
        //略
        // 产生随记验证码    
        for (var i = 0; i < codeLength; i++) {
            code += parseInt(Math.random() * 9).toString();
        }

        // 设置按钮显示效果，倒计时   
        $("#getcode").attr("disabled", "true");
        $("#getcode").val("请在" + curCount + "秒内输入验证码");
        InterValObj = window.setInterval(SetRemainTime, 1000); // 启动计时器，1秒执行一次    

        // 向后台发送处理数据    
        $.ajax({
            type: "POST", // 用POST方式传输    
            dataType: "text", // 数据格式:JSON    
            url: "/Register/GetCode", // 目标地址    
            data: { "Code": code, "phoNum": phoNum },
            error: function (msg) {
                alert(msg);
            },
            success: function (data) {
                //前台给出提示语
                if (data == "true") {
                    $("#telephonenameTip").html("<font color='#339933'>√ 短信验证码已发到您的手机,请查收(30分钟内有效)</font>");
                } else if (data == "false") {
                    $("#telephonenameTip").html("<font color='red'>× 短信验证码发送失败，请重新发送</font>");
                    return false;
                }
            }
        });

    });

    //timer处理函数    
    function SetRemainTime() {
        if (curCount == 0) {
            window.clearInterval(InterValObj);// 停止计时器    
            $("#getcode").removeAttr("disabled");// 启用按钮    
            $("#getcode").val("重新发送验证码");
            code = ""; // 清除验证码。如果不清除，过时间后，输入收到的验证码依然有效    
        } else {
            curCount--;
            $("#getcode").val("请在" + curCount + "秒内输入验证码");
        }
    }

    //提交注册按钮
    $("#submit").click(function () {
        var CheckCode = $("#codename").val();
        // 向后台发送处理数据    
        $.ajax({
            url: "/Register/CheckCode",
            data: { "CheckCode": CheckCode },
            type: "POST",
            dataType: "text",
            success: function (data) {
                if (data == "true") {
                    $("#codenameTip").html("<font color='#339933'>√</font>");
                } else {
                    $("#codenameTip").html("<font color='red'>× 短信验证码有误，请核实后重新填写</font>");
                    return;
                }
            }
        });
    });
}