function CargaReportePdf (objParam) {
    var urlConfig = "_flowId=viewReportFlow&standAlone=true&decorate=no&";
     var urlHost = document.getElementById("rptUrlDes").value;
    var urlRender = "&j_username=envibol&j_password=123456789 &output=pdf";
     urlReport = objParam.ruta;
    urlReport = "reportUnit=" + urlReport.replace(/['/']/gi, '%2F');

    var urlParam = '';
    var urlFinal = '';

    if (typeof (objParam) != "undefined" && typeof (objParam) == "object") {
        var vObj = Object.getOwnPropertyNames(objParam);
        for (var i = 0; i < vObj.length; i++) {
            urlParam = urlParam + '&' + vObj[i] + '=' + objParam[vObj[i]];
        }
    }

    urlFinal = urlHost + urlConfig + urlReport + urlParam + urlRender;
     window.open(urlFinal, "SedemReport", "location=no,width=900,height=750,scrollbars=yes,top=0,left=750,resizable = no");
}

function CargaReportePop(objParam) {

    var urlConfig = "_flowId=viewReportFlow&standAlone=true&decorate=no&";
    var urlHost = document.getElementById("rptUrlDes").value;
    var urlRender = "&j_username=envibol&j_password=123456789 ";
    urlReport = objParam.ruta;
    urlReport = "reportUnit=" + urlReport.replace(/['/']/gi, '%2F');

    var urlParam = '';
    var urlFinal = '';

    if (typeof (objParam) != "undefined" && typeof (objParam) == "object") {
        var vObj = Object.getOwnPropertyNames(objParam);
        for (var i = 1; i < vObj.length; i++) {
            urlParam = urlParam + '&' + vObj[i] + '=' + objParam[vObj[i]];
        }
    }

    urlFinal = urlHost + urlConfig + urlReport + urlParam + urlRender;
    window.open(urlFinal, "SedemReport", "location=no,width=screen.width,height=screen.height,scrollbars=yes,resizable=yes");
}

function CargaReporteExcel(objParam) {
    var urlConfig = "_flowId=viewReportFlow&standAlone=true&decorate=no&";
    var urlHost = document.getElementById("rptUrlDes").value;
    var urlRender = "&j_username=jasperadmin&j_password=jasperadmin&output=xls";
    var urlReport = objParam.ruta;
    urlReport = "reportUnit=" + urlReport.replace(/['/']/gi, '%2F');

    var urlParam = '';
    var urlFinal = '';

    if (typeof (objParam) != "undefined" && typeof (objParam) == "object") {
        var vObj = Object.getOwnPropertyNames(objParam);
        for (var i = 0; i < vObj.length; i++) {
            urlParam = urlParam + '&' + vObj[i] + '=' + objParam[vObj[i]];
        }
    }

    urlFinal = urlHost + urlConfig + urlReport + urlParam + urlRender;
    window.open(urlFinal, "SedemExcel", "location=no,width=900,height=750,scrollbars=yes,top=0,left=750,resizable=no");
}


function cambiarTextoYAxis() {
    var textElements = document.querySelectorAll('.mud-charts-yaxis text');
    textElements.forEach((textElement) => {
        var numero = Number(textElement.textContent);
        if (numero >= 1000000) {
            textElement.textContent = (numero / 1000000).toFixed(2) + "M";
        } else if (numero >= 1000) {
            textElement.textContent = (numero / 1000).toFixed(2) + "K";
        }
    });
    console.log("ejecutando función cambiarTextoYAxis");
}

function CargaReporteWord(objParam) {
    // Nombre del endpoint (sin la URL completa)
    var urlHost = "proxy/jasper";
    var urlParam = `?formato=docx&ruta=${encodeURIComponent(objParam.ruta)}`;

    // Descargar el archivo directamente
    fetch(urlHost + urlParam)
        .then(response => {
            if (!response.ok) {
                throw new Error('Error en la descarga del reporte');
            }
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = 'reporte.docx'; // Nombre del archivo descargado
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(err => console.error('Error al descargar el reporte en Word:', err));
}

