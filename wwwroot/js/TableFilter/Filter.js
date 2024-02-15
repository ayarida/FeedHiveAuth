function checkval() {
    1 == $("tbody tr:visible").length &&
        "No result found" == $("tbody tr:visible td").html()
        ? $("#rowcount").html("0")
        : $("#rowcount").html($("tr:visible").length - 1);
}

$(document).ready(function () {
    let showFilter = false;

    $("#rowcount").html($(".filterable tr").length - 1),
        $(".filterable .btn-filter").click(function () {
            console.log("hello")
            var t = $(this).parents(".filterable"),
                e = t.find(".filters input"),
                l = t.find(".table tbody");
            if (showFilter) {
                e.val("").prop("disabled", true)
                l.find(".no-result").remove();
                l.find("tr").show();
                $("#rowcount").html($(".filterable tr").length - 1)
                showFilter = !showFilter
              
            } else {
                (e.prop("disabled", false), e.first().focus())
                showFilter = !showFilter
            


            }
        
        }),
        $(".filterable .filters input").keyup(function (t) {
            if ("9" != (t.keyCode || t.which)) {
                var e = $(this),
                    l = e.val().toLowerCase(),
                    n = e.parents(".filterable"),
                    i = n.find(".filters th").index(e.parents("th")),
                    r = n.find(".table"),
                    o = r.find("tbody tr"),
                    d = o.filter(function () {
                        return (
                            -1 === $(this).find("td").eq(i).text().toLowerCase().indexOf(l)
                        );
                    });
                r.find("tbody .no-result").remove(),
                    o.show(),
                    d.hide(),
                    d.length === o.length &&
                    r
                        .find("tbody")
                        .prepend(
                            $(
                                '<tr class="no-result text-center"><td colspan="' +
                                r.find(".filters th").length +
                                '">No result found</td></tr>'
                            )
                        );
            }
            $("#rowcount").html($("tr:visible").length - 1), checkval();
        });
});
