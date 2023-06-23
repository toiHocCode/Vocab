// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function configEditor(className) {
    var addClass = `add-${className}`;
    var removeClass = `remove-${className}`;

    $("." + className).find("button").each(function (i) {
        if (i == 0) {
            $(this).addClass(addClass).text("+");
        } else {
            $(this).addClass(removeClass).text("-");
        }
    })

    $("." + className).first().find("button").click(function () {
        var el = $(this).closest(`.${className}`);
        var newEl = el.clone();
        newEl.find("label").remove();
        newEl.find("*[id]").removeAttr("id");
        newEl.find("button").last().text("-").removeClass(addClass).addClass(removeClass);

        var index = el.parent().children(`.${className}`).length;
        newEl.find(":input[name]").each(function () {
            var names = $(this).attr("name").match(/(.+)\d+(.+)/);
            $(this).attr("name", `${names[1]}${index}${names[2]}`).val("");
        });

        newEl.find("input[type='hidden']").val("0"); // TODO

        newEl.insertAfter(el.parent().children(`.${className}`).last());
    });

    $("body").on("click", "." + removeClass, function () {
        var el = $(this).closest(".form-row");
        var matches = el.find(":input[name]").attr("name").match(/\d+/);
        var curIndex = parseInt(matches[0]);

        el.nextAll().each(function (i) {
            $(this).find(":input[name]").each(function () {
                var names = $(this).attr("name").match(/(.+)\d+(.+)/);
                if (names && names.length == 3) {
                    $(this).attr("name", `${names[1]}${curIndex + i}${names[2]}`);
                }
            });
        });

        el.remove();
    });
}