


$(document).ready(function () {
   
var id = document.getElementById("EmployeeId").textContent;
var c = document.getElementById("hbar_chart");
var ctx = c.getContext("2d");
var tData = $.getValues(`/Admin/Task/GetMonthlyTasks?id=${id}`);
const allData = tData.datasets.map(getData);
var myHBarChart = new Chart(ctx, {
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
        indexAxis: 'y',
        layout: {
            padding: {
                top: 10,
            }
        },
        plugins: {
            title: {
                display: true,
                text: "Employee's Tasks - last 30 days "
            }
        },
        maintainAspectRatio: false,
        scales: {
            xAxes: [{
                ticks: {
                    beginAtZero: true 
                }
            }],
            yAxes: [{
                stacked: true
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