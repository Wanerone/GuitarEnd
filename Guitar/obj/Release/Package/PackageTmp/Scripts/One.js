$(function () {
    //璁捐妗堜緥鍒囨崲
    $('.title-lists li').mouseover(function () {
        var liindex = $('.title-lists li').index(this);
        $(this).addClass('on').siblings().removeClass('on');
        $('.product-wraps div.products').eq(liindex).fadeIn(150).siblings('div.products').hide();
        var liWidth = $('.title-lists li').width();
        $('.lanrenzhijias .title-lists p').stop(false, true).animate({ 'left': liindex * liWidth + 'px' }, 300);
    });

    //璁捐妗堜緥hover鏁堟灉
    $('.product-wraps .products li').hover(function () {
        $(this).css("border-color", "#ff6600");
        $(this).find('p > a').css('color', '#ff6600');
    }, function () {
        $(this).css("border-color", "#fafafa");
        $(this).find('p > a').css('color', '#666666');
    });
});