
(function () {
    var isRequestPending = false;
//    var ajaxStartIndex = 0;

    $("body").bind("ajaxStart", function () {
        isRequestPending = true;
    })
    .bind("ajaxStop", function () {
////        ajaxStartIndex++;
        isRequestPending = false;
    });

    window.SizSelCsZzz_IsRequestPending = function () {
        return isRequestPending;
    };

//    window.SizSelCsZzz_GetAjaxStartCount = function () {
//        return ajaxStartIndex;
//    }
//    
})();

