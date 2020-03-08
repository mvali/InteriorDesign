addEventListener("load", function () { setTimeout(hideURLbar, 0); }, false);
function hideURLbar() { window.scrollTo(0, 1); }

// You can also use "$(window).load(function() {"
$(function () {
    $("#slider").responsiveSlides({
        auto: true,
        manualControls: '#slider3-pager',
    });
});
jQuery(document).ready(function ($) {
    $(".scroll").click(function (event) {
        event.preventDefault();
        $('html,body').animate({ scrollTop: $(this.hash).offset().top }, 1000);
    });
});
//-- smooth scrolling
$(document).ready(function() {
    /*
        var defaults = {
        containerID: 'toTop', // fading element id
        containerHoverID: 'toTopHover', // fading element hover id
        scrollSpeed: 1200,
        easingType: 'linear' 
        };
    */								
    $().UItoTop({ easingType: 'easeOutQuart' });
});
//smooth scrolling
(function (i, s, o, g, r, a, m) {
    i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
        (i[r].q = i[r].q || []).push(arguments)
    }, i[r].l = 1 * new Date(); a = s.createElement(o),
    m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
})(window, document, 'script', 'https://www.google-analytics.com/analytics.js', 'ga');

ga('create', 'UA-92322799-1', 'auto');
ga('send', 'pageview');
