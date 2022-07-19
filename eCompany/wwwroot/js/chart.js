$(document).ready(function () {
    var id = document.getElementById("EmployeeId").textContent;
    var c = document.getElementById("vbar_chart");
    var ctx = c.getContext("2d");
    var tData = $.getValues(`/Admin/Task/GetTasksChart?id=${id}`);
    const allData = tData.datasets.map(getData);
    var myBarChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: tData.labels,
            datasets: [{
                label: "Number of Tasks",
                backgroundColor: tData.datasets.map(getColors),
                data: tData.datasets.map(getData),
                borderColor: tData.datasets.map(getBorderColor),
                borderWidth: 1
            }]
        },
        options: {
            layout: {
                padding: {
                    top: 10,
                }
            },
            plugins: {
                title: {
                    display: true,
                    text: "Employee's Tasks Chart "
                }
            },
            maintainAspectRatio: false,
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }

        },

    });



});



function getData(item) {
    return item.data;
}

function getColors(item) {
    return item.backgroundColor;
}

function getBorderColor(item) {
    return item.borderColor;
}