﻿$(document).ready(function () {
    $('.MyDataTables').DataTable({
        responsive: true,
        lengthMenu: [[7, 15, 25, 50], [7, 10, 25, 50]],
        language: {

            "decimal": "",
            "emptyTable": "No data available in table",
            "info": "Mostrando _START_ registro de _END_ em um total de _TOTAL_ entrandas",
            "infoEmpty": "Showing 0 to 0 of 0 entries",
            "infoFiltered": "(filtered from _MAX_ total entries)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ entradas",
            "loadingRecords": "Loading...",
            "processing": "",
            "search": "Procurar:",
            "zeroRecords": "No matching records found",
            "paginate": {
                "first": "Primeira",
                "last": "Última",
                "next": "Próximo",
                "previous": "Anterior"
            },
            "aria": {
                "sortAscending": ": activate to sort column ascending",
                "sortDescending": ": activate to sort column descending"
            }
        }
    });

    setTimeout((messageTemporayry) => {
        $(".alert").fadeOut("slow", () => {
            $(this).alert("close");
        })
    }, 15000)

    var footer = $('#dynamicFooter');
    var colors = ['#FF5733', '#33FF9E']; // Cores para alternância: vermelho e verde
    var currentColorIndex = 0;

    function changeFooterColor() {
        footer.fadeOut('slow', function () {
            footer.css('background-color', colors[currentColorIndex]).fadeIn('slow');
        });
        currentColorIndex = (currentColorIndex + 1) % colors.length;
    }

    setInterval(changeFooterColor, 2000); // Altera a cor a cada 2 segundos (2000ms)
})

//preloader
//$(window).on('load', function () {
//    $("#preloader").delay(200).fadeOut("slow");
//});

// Verifica o tamanho do conteúdo e aplica classe CSS ao rodapé


//$(document).ready(function() {
//    $('#Emprestimos').DataTable({
//        paging: false,
//        scrollCollapse: true,
//        scrollY: '200px'
//    });
//});
