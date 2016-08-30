$(document).ready(function () {

    $('body').on("click", ".js-download", function (e) {
        
        e.preventDefault();

        SetupDownloadButtons(true);

        var filename = $(this).attr('data-filename');
        var uri = 'api/download/';

        var url = uri;
        $.ajax({
            url: url,
            type: 'GET',
            cache: false,
            data: { filename: filename },
            success: function (data) {

                window.location.href = "DataLoad/Details?auditID=" + data.AuditID + "&TimeElapsed=" + data.TimeElapsed;
            }
        });
    })

    $('body').on("click", ".js-download-invalid", function () {

        var filename = $(this).attr('data-filename');
        var errorMessage = "the content or the name of the file " + filename + " are invalid.";

        alert(errorMessage);
    })

    /*Disabling download buttons during download
    ReEnabling at the end*/
    function SetupDownloadButtons(downloading)
    {
        if (downloading) {
            //disable all
            $('.js-download, .js-download-invalid').each(function (i, button) {
                $(this).addClass("not-active");
            });
        }
        else {
            //enable all
            $('.js-download, .js-download-invalid').each(function (i, button) {
                $(this).removeClass("not-active");
            });
        }
    }
});