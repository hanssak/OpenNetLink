window.loadJavaScript = function() {
    setTimeout(function() {
        /*
        var scriptElement=document.createElement('script');
        scriptElement.type = 'text/javascript';
        scriptElement.src = "plugins/jquery/jquery.min.js";
        document.head.appendChild(scriptElement);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "plugins/jquery-mousewheel/jquery.mousewheel.js";
            document.head.appendChild(scriptElement);
        }, 150);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "plugins/raphael/raphael.min.js";
            document.head.appendChild(scriptElement);
        }, 150);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "plugins/jquery-mapael/jquery.mapael.min.js";
            document.head.appendChild(scriptElement);
        }, 150);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "plugins/jquery-mapael/maps/usa_states.min.js";
            document.head.appendChild(scriptElement);
        }, 150);
        */

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "js/adminlte/demo.js";
            document.head.appendChild(scriptElement);
        }, 50);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "plugins/chart.js/Chart.min.js";
            document.head.appendChild(scriptElement);
        }, 50);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "plugins/sparklines/sparkline.js";
            document.head.appendChild(scriptElement);
        }, 50);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "js/adminlte/pages/dashboard.js";
            document.head.appendChild(scriptElement);
        }, 50);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "js/adminlte/pages/dashboard2.js";
            document.head.appendChild(scriptElement);
        }, 50);

        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "js/adminlte/pages/dashboard3.js";
            document.head.appendChild(scriptElement);
        }, 50);

    }, 50);
};

window.loadTreeView = function() {
  const trees = $('[data-widget="treeview"]');
  trees.Treeview('init');

  $(function() {
    // See here, I have our selector set to "a", so this jQuery object will grab all a tags on the page
    $("a").on("click", function(e) {
        $(this).parent().parent().parent().parent().parent().parent().find("a").removeClass("active");
        $(this).parent().parent().parent().parent().find("a").removeClass("active");
        $(this).parent().parent().parent().parent().siblings("a").addClass("active");
        $(this).parent().parent().siblings("a").addClass("active");
        $(this).addClass("active").siblings().removeClass("active");
    });
  })

  window.external.receiveMessage((messages) => {
      var RegExp = /^(.*)(:)(.*)/gim;
      var Event = messages.replace(RegExp, "$1");
      var Args = messages.replace(RegExp, "$3");

      switch (Event) {
        case "DragNDrop":
          console.log("c# message: " + Args);
          alert("c# message: " + Args);
          break;
        default:
          console.log("Sorry, we are out of Event from ReceiveCallback!!");
      }
  })
};

window.loadChart = function () {
    /* ChartJS
     * -------
     * Here we will create a few charts using ChartJS
     */

    //--------------
    //- AREA CHART -
    //--------------

    // Get context with jQuery - using jQuery's .get() method.
    var areaChartCanvas = $('#areaChart').get(0).getContext('2d')

    var areaChartData = {
      labels  : ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
        {
          label               : 'Digital Goods',
          backgroundColor     : 'rgba(60,141,188,0.9)',
          borderColor         : 'rgba(60,141,188,0.8)',
          pointRadius          : false,
          pointColor          : '#3b8bba',
          pointStrokeColor    : 'rgba(60,141,188,1)',
          pointHighlightFill  : '#fff',
          pointHighlightStroke: 'rgba(60,141,188,1)',
          data                : [28, 48, 40, 19, 86, 27, 90]
        },
        {
          label               : 'Electronics',
          backgroundColor     : 'rgba(210, 214, 222, 1)',
          borderColor         : 'rgba(210, 214, 222, 1)',
          pointRadius         : false,
          pointColor          : 'rgba(210, 214, 222, 1)',
          pointStrokeColor    : '#c1c7d1',
          pointHighlightFill  : '#fff',
          pointHighlightStroke: 'rgba(220,220,220,1)',
          data                : [65, 59, 80, 81, 56, 55, 40]
        },
      ]
    }

    var areaChartOptions = {
      maintainAspectRatio : false,
      responsive : true,
      legend: {
        display: false
      },
      scales: {
        xAxes: [{
          gridLines : {
            display : false,
          }
        }],
        yAxes: [{
          gridLines : {
            display : false,
          }
        }]
      }
    }

    // This will get the first returned node in the jQuery collection.
    var areaChart       = new Chart(areaChartCanvas, { 
      type: 'line',
      data: areaChartData, 
      options: areaChartOptions
    })

    //-------------
    //- LINE CHART -
    //--------------
    var lineChartCanvas = $('#lineChart').get(0).getContext('2d')
    var lineChartOptions = jQuery.extend(true, {}, areaChartOptions)
    var lineChartData = jQuery.extend(true, {}, areaChartData)
    lineChartData.datasets[0].fill = false;
    lineChartData.datasets[1].fill = false;
    lineChartOptions.datasetFill = false

    var lineChart = new Chart(lineChartCanvas, { 
      type: 'line',
      data: lineChartData, 
      options: lineChartOptions
    })

    //-------------
    //- DONUT CHART -
    //-------------
    // Get context with jQuery - using jQuery's .get() method.
    var donutChartCanvas = $('#donutChart').get(0).getContext('2d')
    var donutData        = {
      labels: [
          'Chrome', 
          'IE',
          'FireFox', 
          'Safari', 
          'Opera', 
          'Navigator', 
      ],
      datasets: [
        {
          data: [700,500,400,600,300,100],
          backgroundColor : ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#3c8dbc', '#d2d6de'],
        }
      ]
    }
    var donutOptions     = {
      maintainAspectRatio : false,
      responsive : true,
    }
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    var donutChart = new Chart(donutChartCanvas, {
      type: 'doughnut',
      data: donutData,
      options: donutOptions      
    })

    //-------------
    //- PIE CHART -
    //-------------
    // Get context with jQuery - using jQuery's .get() method.
    var pieChartCanvas = $('#pieChart').get(0).getContext('2d')
    var pieData        = donutData;
    var pieOptions     = {
      maintainAspectRatio : false,
      responsive : true,
    }
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    var pieChart = new Chart(pieChartCanvas, {
      type: 'pie',
      data: pieData,
      options: pieOptions      
    })

    //-------------
    //- BAR CHART -
    //-------------
    var barChartCanvas = $('#barChart').get(0).getContext('2d')
    var barChartData = jQuery.extend(true, {}, areaChartData)
    var temp0 = areaChartData.datasets[0]
    var temp1 = areaChartData.datasets[1]
    barChartData.datasets[0] = temp1
    barChartData.datasets[1] = temp0

    var barChartOptions = {
      responsive              : true,
      maintainAspectRatio     : false,
      datasetFill             : false
    }

    var barChart = new Chart(barChartCanvas, {
      type: 'bar', 
      data: barChartData,
      options: barChartOptions
    })

    //---------------------
    //- STACKED BAR CHART -
    //---------------------
    var stackedBarChartCanvas = $('#stackedBarChart').get(0).getContext('2d')
    var stackedBarChartData = jQuery.extend(true, {}, barChartData)

    var stackedBarChartOptions = {
      responsive              : true,
      maintainAspectRatio     : false,
      scales: {
        xAxes: [{
          stacked: true,
        }],
        yAxes: [{
          stacked: true
        }]
      }
    }

    var stackedBarChart = new Chart(stackedBarChartCanvas, {
      type: 'bar', 
      data: stackedBarChartData,
      options: stackedBarChartOptions
    })
  }

window.loadFlot = function () {
    /*
     * Flot Interactive Chart
     * -----------------------
     */
    // We use an inline data source in the example, usually data would
    // be fetched from a server
    var data        = [],
        totalPoints = 100

    function getRandomData() {

      if (data.length > 0) {
        data = data.slice(1)
      }

      // Do a random walk
      while (data.length < totalPoints) {

        var prev = data.length > 0 ? data[data.length - 1] : 50,
            y    = prev + Math.random() * 10 - 5

        if (y < 0) {
          y = 0
        } else if (y > 100) {
          y = 100
        }

        data.push(y)
      }

      // Zip the generated y values with the x values
      var res = []
      for (var i = 0; i < data.length; ++i) {
        res.push([i, data[i]])
      }

      return res
    }

    var interactive_plot = $.plot('#interactive', [
        {
          data: getRandomData(),
        }
      ],
      {
        grid: {
          borderColor: '#f3f3f3',
          borderWidth: 1,
          tickColor: '#f3f3f3'
        },
        series: {
          color: '#3c8dbc',
          lines: {
            lineWidth: 2,
            show: true,
            fill: true,
          },
        },
        yaxis: {
          min: 0,
          max: 100,
          show: true
        },
        xaxis: {
          show: true
        }
      }
    )

    var updateInterval = 500 //Fetch data ever x milliseconds
    var realtime       = 'on' //If == to on then fetch data every x seconds. else stop fetching
    function update() {

      interactive_plot.setData([getRandomData()])

      // Since the axes don't change, we don't need to call plot.setupGrid()
      interactive_plot.draw()
      if (realtime === 'on') {
        setTimeout(update, updateInterval)
      }
    }

    //INITIALIZE REALTIME DATA FETCHING
    if (realtime === 'on') {
      update()
    }
    //REALTIME TOGGLE
    $('#realtime .btn').click(function () {
      if ($(this).data('toggle') === 'on') {
        realtime = 'on'
      }
      else {
        realtime = 'off'
      }
      update()
    })
    /*
     * END INTERACTIVE CHART
     */


    /*
     * LINE CHART
     * ----------
     */
    //LINE randomly generated data

    var sin = [],
        cos = []
    for (var i = 0; i < 14; i += 0.5) {
      sin.push([i, Math.sin(i)])
      cos.push([i, Math.cos(i)])
    }
    var line_data1 = {
      data : sin,
      color: '#3c8dbc'
    }
    var line_data2 = {
      data : cos,
      color: '#00c0ef'
    }
    $.plot('#line-chart', [line_data1, line_data2], {
      grid  : {
        hoverable  : true,
        borderColor: '#f3f3f3',
        borderWidth: 1,
        tickColor  : '#f3f3f3'
      },
      series: {
        shadowSize: 0,
        lines     : {
          show: true
        },
        points    : {
          show: true
        }
      },
      lines : {
        fill : false,
        color: ['#3c8dbc', '#f56954']
      },
      yaxis : {
        show: true
      },
      xaxis : {
        show: true
      }
    })
    //Initialize tooltip on hover
    $('<div class="tooltip-inner" id="line-chart-tooltip"></div>').css({
      position: 'absolute',
      display : 'none',
      opacity : 0.8
    }).appendTo('body')
    $('#line-chart').bind('plothover', function (event, pos, item) {

      if (item) {
        var x = item.datapoint[0].toFixed(2),
            y = item.datapoint[1].toFixed(2)

        $('#line-chart-tooltip').html(item.series.label + ' of ' + x + ' = ' + y)
          .css({
            top : item.pageY + 5,
            left: item.pageX + 5
          })
          .fadeIn(200)
      } else {
        $('#line-chart-tooltip').hide()
      }

    })
    /* END LINE CHART */

    /*
     * FULL WIDTH STATIC AREA CHART
     * -----------------
     */
    var areaData = [[2, 88.0], [3, 93.3], [4, 102.0], [5, 108.5], [6, 115.7], [7, 115.6],
      [8, 124.6], [9, 130.3], [10, 134.3], [11, 141.4], [12, 146.5], [13, 151.7], [14, 159.9],
      [15, 165.4], [16, 167.8], [17, 168.7], [18, 169.5], [19, 168.0]]
    $.plot('#area-chart', [areaData], {
      grid  : {
        borderWidth: 0
      },
      series: {
        shadowSize: 0, // Drawing is faster without shadows
        color     : '#00c0ef',
        lines : {
          fill: true //Converts the line chart to area chart
        },
      },
      yaxis : {
        show: false
      },
      xaxis : {
        show: false
      }
    })

    /* END AREA CHART */

    /*
     * BAR CHART
     * ---------
     */

    var bar_data = {
      data : [[1,10], [2,8], [3,4], [4,13], [5,17], [6,9]],
      bars: { show: true }
    }
    $.plot('#bar-chart', [bar_data], {
      grid  : {
        borderWidth: 1,
        borderColor: '#f3f3f3',
        tickColor  : '#f3f3f3'
      },
      series: {
         bars: {
          show: true, barWidth: 0.5, align: 'center',
        },
      },
      colors: ['#3c8dbc'],
      xaxis : {
        ticks: [[1,'January'], [2,'February'], [3,'March'], [4,'April'], [5,'May'], [6,'June']]
      }
    })
    /* END BAR CHART */

    /*
     * DONUT CHART
     * -----------
     */

    var donutData = [
      {
        label: 'Series2',
        data : 30,
        color: '#3c8dbc'
      },
      {
        label: 'Series3',
        data : 20,
        color: '#0073b7'
      },
      {
        label: 'Series4',
        data : 50,
        color: '#00c0ef'
      }
    ]
    $.plot('#donut-chart', donutData, {
      series: {
        pie: {
          show       : true,
          radius     : 1,
          innerRadius: 0.5,
          label      : {
            show     : true,
            radius   : 2 / 3,
            formatter: labelFormatter,
            threshold: 0.1
          }

        }
      },
      legend: {
        show: false
      }
    })
    /*
     * END DONUT CHART
     */

}

/*
* Custom Label formatter
* ----------------------
*/
function labelFormatter(label, series) {
return '<div style="font-size:13px; text-align:center; padding:2px; color: #fff; font-weight: 600;">'
    + label
    + '<br>'
    + Math.round(series.percent) + '%</div>'
}

window.loadInline = function () {
    /* jQueryKnob */

    $('.knob').knob({
      /*change : function (value) {
       //console.log("change : " + value);
       },
       release : function (value) {
       console.log("release : " + value);
       },
       cancel : function () {
       console.log("cancel : " + this.value);
       },*/
      draw: function () {

        // "tron" case
        if (this.$.data('skin') == 'tron') {

          var a   = this.angle(this.cv)  // Angle
            ,
              sa  = this.startAngle          // Previous start angle
            ,
              sat = this.startAngle         // Start angle
            ,
              ea                            // Previous end angle
            ,
              eat = sat + a                 // End angle
            ,
              r   = true

          this.g.lineWidth = this.lineWidth

          this.o.cursor
          && (sat = eat - 0.3)
          && (eat = eat + 0.3)

          if (this.o.displayPrevious) {
            ea = this.startAngle + this.angle(this.value)
            this.o.cursor
            && (sa = ea - 0.3)
            && (ea = ea + 0.3)
            this.g.beginPath()
            this.g.strokeStyle = this.previousColor
            this.g.arc(this.xy, this.xy, this.radius - this.lineWidth, sa, ea, false)
            this.g.stroke()
          }

          this.g.beginPath()
          this.g.strokeStyle = r ? this.o.fgColor : this.fgColor
          this.g.arc(this.xy, this.xy, this.radius - this.lineWidth, sat, eat, false)
          this.g.stroke()

          this.g.lineWidth = 2
          this.g.beginPath()
          this.g.strokeStyle = this.o.fgColor
          this.g.arc(this.xy, this.xy, this.radius - this.lineWidth + 1 + this.lineWidth * 2 / 3, 0, 2 * Math.PI, false)
          this.g.stroke()

          return false
        }
      }
    })
    /* END JQUERY KNOB */

    //INITIALIZE SPARKLINE CHARTS
    $('.sparkline').each(function () {
      var $this = $(this)
      $this.sparkline('html', $this.data())
    })

    /* SPARKLINE DOCUMENTATION EXAMPLES http://omnipotent.net/jquery.sparkline/#s-about */
    //drawDocSparklines()
    //drawMouseSpeedDemo()

}

function drawDocSparklines() {

    // Bar + line composite charts
    $('#compositebar').sparkline('html', {
        type    : 'bar',
        barColor: '#aaf'
    })
    $('#compositebar').sparkline([4, 1, 5, 7, 9, 9, 8, 7, 6, 6, 4, 7, 8, 4, 3, 2, 2, 5, 6, 7],
        {
        composite: true,
        fillColor: false,
        lineColor: 'red'
        })


    // Line charts taking their values from the tag
    $('.sparkline-1').sparkline()

    // Larger line charts for the docs
    $('.largeline').sparkline('html',
        {
        type  : 'line',
        height: '2.5em',
        width : '4em'
        })

    // Customized line chart
    $('#linecustom').sparkline('html',
        {
        height      : '1.5em',
        width       : '8em',
        lineColor   : '#f00',
        fillColor   : '#ffa',
        minSpotColor: false,
        maxSpotColor: false,
        spotColor   : '#77f',
        spotRadius  : 3
        })

    // Bar charts using inline values
    $('.sparkbar').sparkline('html', { type: 'bar' })

    $('.barformat').sparkline([1, 3, 5, 3, 8], {
        type               : 'bar',
        tooltipFormat      : '{{value:levels}} - {{value}}',
        tooltipValueLookups: {
        levels: $.range_map({
            ':2' : 'Low',
            '3:6': 'Medium',
            '7:' : 'High'
        })
        }
    })

    // Tri-state charts using inline values
    $('.sparktristate').sparkline('html', { type: 'tristate' })
    $('.sparktristatecols').sparkline('html',
        {
        type    : 'tristate',
        colorMap: {
            '-2': '#fa7',
            '2' : '#44f'
        }
        })

    // Composite line charts, the second using values supplied via javascript
    $('#compositeline').sparkline('html', {
        fillColor     : false,
        changeRangeMin: 0,
        chartRangeMax : 10
    })
    $('#compositeline').sparkline([4, 1, 5, 7, 9, 9, 8, 7, 6, 6, 4, 7, 8, 4, 3, 2, 2, 5, 6, 7],
        {
        composite     : true,
        fillColor     : false,
        lineColor     : 'red',
        changeRangeMin: 0,
        chartRangeMax : 10
        })

    // Line charts with normal range marker
    $('#normalline').sparkline('html',
        {
        fillColor     : false,
        normalRangeMin: -1,
        normalRangeMax: 8
        })
    $('#normalExample').sparkline('html',
        {
        fillColor       : false,
        normalRangeMin  : 80,
        normalRangeMax  : 95,
        normalRangeColor: '#4f4'
        })

    // Discrete charts
    $('.discrete1').sparkline('html',
        {
        type     : 'discrete',
        lineColor: 'blue',
        xwidth   : 18
        })
    $('#discrete2').sparkline('html',
        {
        type          : 'discrete',
        lineColor     : 'blue',
        thresholdColor: 'red',
        thresholdValue: 4
        })

    // Bullet charts
    $('.sparkbullet').sparkline('html', { type: 'bullet' })

    // Pie charts
    $('.sparkpie').sparkline('html', {
        type  : 'pie',
        height: '1.0em'
    })

    // Box plots
    $('.sparkboxplot').sparkline('html', { type: 'box' })
    $('.sparkboxplotraw').sparkline([1, 3, 5, 8, 10, 15, 18],
        {
        type        : 'box',
        raw         : true,
        showOutliers: true,
        target      : 6
        })

    // Box plot with specific field order
    $('.boxfieldorder').sparkline('html', {
        type                     : 'box',
        tooltipFormatFieldlist   : ['med', 'lq', 'uq'],
        tooltipFormatFieldlistKey: 'field'
    })

    // click event demo sparkline
    $('.clickdemo').sparkline()
    $('.clickdemo').bind('sparklineClick', function (ev) {
        var sparkline = ev.sparklines[0],
            region    = sparkline.getCurrentRegionFields()
        value         = region.y
        alert('Clicked on x=' + region.x + ' y=' + region.y)
    })

    // mouseover event demo sparkline
    $('.mouseoverdemo').sparkline()
    $('.mouseoverdemo').bind('sparklineRegionChange', function (ev) {
        var sparkline = ev.sparklines[0],
            region    = sparkline.getCurrentRegionFields()
        value         = region.y
        $('.mouseoverregion').text('x=' + region.x + ' y=' + region.y)
    }).bind('mouseleave', function () {
        $('.mouseoverregion').text('')
    })
}

/**
** Draw the little mouse speed animated graph
** This just attaches a handler to the mousemove event to see
** (roughly) how far the mouse has moved
** and then updates the display a couple of times a second via
** setTimeout()
**/
function drawMouseSpeedDemo() {
    var mrefreshinterval = 500 // update display every 500ms
    var lastmousex       = -1
    var lastmousey       = -1
    var lastmousetime
    var mousetravel      = 0
    var mpoints          = []
    var mpoints_max      = 30
    $('html').mousemove(function (e) {
        var mousex = e.pageX
        var mousey = e.pageY
        if (lastmousex > -1) {
        mousetravel += Math.max(Math.abs(mousex - lastmousex), Math.abs(mousey - lastmousey))
        }
        lastmousex = mousex
        lastmousey = mousey
    })
    var mdraw = function () {
        var md      = new Date()
        var timenow = md.getTime()
        if (lastmousetime && lastmousetime != timenow) {
        var pps = Math.round(mousetravel / (timenow - lastmousetime) * 1000)
        mpoints.push(pps)
        if (mpoints.length > mpoints_max) {
            mpoints.splice(0, 1)
        }
        mousetravel = 0
        $('#mousespeed').sparkline(mpoints, {
            width        : mpoints.length * 2,
            tooltipSuffix: ' pixels per second'
        })
        }
        lastmousetime = timenow
        setTimeout(mdraw, mrefreshinterval)
    }
    // We could use setInterval instead, but I prefer to do it this way
    setTimeout(mdraw, mrefreshinterval);
}

window.loadSliders = function () {
    /* BOOTSTRAP SLIDER */
    $('.slider').bootstrapSlider()

    /* ION SLIDER */
    $('#range_1').ionRangeSlider({
      min     : 0,
      max     : 5000,
      from    : 1000,
      to      : 4000,
      type    : 'double',
      step    : 1,
      prefix  : '$',
      prettify: false,
      hasGrid : true
    })
    $('#range_2').ionRangeSlider()

    $('#range_5').ionRangeSlider({
      min     : 0,
      max     : 10,
      type    : 'single',
      step    : 0.1,
      postfix : ' mm',
      prettify: false,
      hasGrid : true
    })
    $('#range_6').ionRangeSlider({
      min     : -50,
      max     : 50,
      from    : 0,
      type    : 'single',
      step    : 1,
      postfix : '°',
      prettify: false,
      hasGrid : true
    })

    $('#range_4').ionRangeSlider({
      type      : 'single',
      step      : 100,
      postfix   : ' light years',
      from      : 55000,
      hideMinMax: true,
      hideFromTo: false
    })
    $('#range_3').ionRangeSlider({
      type    : 'double',
      postfix : ' miles',
      step    : 10000,
      from    : 25000000,
      to      : 35000000,
      onChange: function (obj) {
        var t = ''
        for (var prop in obj) {
          t += prop + ': ' + obj[prop] + '\r\n'
        }
        $('#result').html(t)
      },
      onLoad  : function (obj) {
        //
      }
    })
  }

window.showNotiSucc = (message) => {
  toastr.success(message);
}

window.loadModals = function() {
    const Toast = Swal.mixin({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000
    });

    $('.swalDefaultSuccess').click(function() {
      Toast.fire({
        icon: 'success',
        title: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.swalDefaultInfo').click(function() {
      Toast.fire({
        icon: 'info',
        title: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.swalDefaultError').click(function() {
      Toast.fire({
        icon: 'error',
        title: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.swalDefaultWarning').click(function() {
      Toast.fire({
        icon: 'warning',
        title: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.swalDefaultQuestion').click(function() {
      Toast.fire({
        icon: 'question',
        title: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });

    $('.toastrDefaultSuccess').click(function() {
      toastr.success('Lorem ipsum dolor sit amet, consetetur sadipscing elitr.')
    });
    $('.toastrDefaultInfo').click(function() {
      toastr.info('Lorem ipsum dolor sit amet, consetetur sadipscing elitr.')
    });
    $('.toastrDefaultError').click(function() {
      toastr.error('Lorem ipsum dolor sit amet, consetetur sadipscing elitr.')
    });
    $('.toastrDefaultWarning').click(function() {
      toastr.warning('Lorem ipsum dolor sit amet, consetetur sadipscing elitr.')
    });

    $('.toastsDefaultDefault').click(function() {
      $(document).Toasts('create', {
        title: 'Toast Title',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultTopLeft').click(function() {
      $(document).Toasts('create', {
        title: 'Toast Title',
        position: 'topLeft',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultBottomRight').click(function() {
      $(document).Toasts('create', {
        title: 'Toast Title',
        position: 'bottomRight',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultBottomLeft').click(function() {
      $(document).Toasts('create', {
        title: 'Toast Title',
        position: 'bottomLeft',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultAutohide').click(function() {
      $(document).Toasts('create', {
        title: 'Toast Title',
        autohide: true,
        delay: 750,
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultNotFixed').click(function() {
      $(document).Toasts('create', {
        title: 'Toast Title',
        fixed: false,
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultFull').click(function() {
      $(document).Toasts('create', {
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.',
        title: 'Toast Title',
        subtitle: 'Subtitle',
        icon: 'fas fa-envelope fa-lg',
      })
    });
    $('.toastsDefaultFullImage').click(function() {
      $(document).Toasts('create', {
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.',
        title: 'Toast Title',
        subtitle: 'Subtitle',
        image: '../../dist/img/user3-128x128.jpg',
        imageAlt: 'User Picture',
      })
    });
    $('.toastsDefaultSuccess').click(function() {
      $(document).Toasts('create', {
        class: 'bg-success', 
        title: 'Toast Title',
        subtitle: 'Subtitle',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultInfo').click(function() {
      $(document).Toasts('create', {
        class: 'bg-info', 
        title: 'Toast Title',
        subtitle: 'Subtitle',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultWarning').click(function() {
      $(document).Toasts('create', {
        class: 'bg-warning', 
        title: 'Toast Title',
        subtitle: 'Subtitle',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultDanger').click(function() {
      $(document).Toasts('create', {
        class: 'bg-danger', 
        title: 'Toast Title',
        subtitle: 'Subtitle',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
    $('.toastsDefaultMaroon').click(function() {
      $(document).Toasts('create', {
        class: 'bg-maroon', 
        title: 'Toast Title',
        subtitle: 'Subtitle',
        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
      })
    });
  };

window.loadRibbons = function () {
    /* BOOTSTRAP SLIDER */
    $('.slider').bootstrapSlider()

    /* ION SLIDER */
    $('#range_1').ionRangeSlider({
      min     : 0,
      max     : 5000,
      from    : 1000,
      to      : 4000,
      type    : 'double',
      step    : 1,
      prefix  : '$',
      prettify: false,
      hasGrid : true
    })
    $('#range_2').ionRangeSlider()

    $('#range_5').ionRangeSlider({
      min     : 0,
      max     : 10,
      type    : 'single',
      step    : 0.1,
      postfix : ' mm',
      prettify: false,
      hasGrid : true
    })
    $('#range_6').ionRangeSlider({
      min     : -50,
      max     : 50,
      from    : 0,
      type    : 'single',
      step    : 1,
      postfix : '°',
      prettify: false,
      hasGrid : true
    })

    $('#range_4').ionRangeSlider({
      type      : 'single',
      step      : 100,
      postfix   : ' light years',
      from      : 55000,
      hideMinMax: true,
      hideFromTo: false
    })
    $('#range_3').ionRangeSlider({
      type    : 'double',
      postfix : ' miles',
      step    : 10000,
      from    : 25000000,
      to      : 35000000,
      onChange: function (obj) {
        var t = ''
        for (var prop in obj) {
          t += prop + ': ' + obj[prop] + '\r\n'
        }
        $('#result').html(t)
      },
      onLoad  : function (obj) {
        //
      }
    })
}

window.loadFormsGeneral = function () {
  bsCustomFileInput.init();
};

window.loadFormsAdvanced = function () {
    //Initialize Select2 Elements
    $('.select2').select2()

    //Initialize Select2 Elements
    $('.select2bs4').select2({
      theme: 'bootstrap4'
    })

    //Datemask dd/mm/yyyy
    $('#datemask').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' })
    //Datemask2 mm/dd/yyyy
    $('#datemask2').inputmask('mm/dd/yyyy', { 'placeholder': 'mm/dd/yyyy' })
    //Money Euro
    $('[data-mask]').inputmask()

    //Date range picker
    $('#reservationdate').datetimepicker({
        format: 'L'
    });
    //Date range picker
    $('#reservation').daterangepicker()
    //Date range picker with time picker
    $('#reservationtime').daterangepicker({
      timePicker: true,
      timePickerIncrement: 30,
      locale: {
        format: 'MM/DD/YYYY hh:mm A'
      }
    })
    //Date range as a button
    $('#daterange-btn').daterangepicker(
      {
        ranges   : {
          'Today'       : [moment(), moment()],
          'Yesterday'   : [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
          'Last 7 Days' : [moment().subtract(6, 'days'), moment()],
          'Last 30 Days': [moment().subtract(29, 'days'), moment()],
          'This Month'  : [moment().startOf('month'), moment().endOf('month')],
          'Last Month'  : [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        startDate: moment().subtract(29, 'days'),
        endDate  : moment()
      },
      function (start, end) {
        $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'))
      }
    )

    //Timepicker
    $('#timepicker').datetimepicker({
      format: 'LT'
    })
    
    //Bootstrap Duallistbox
    $('.duallistbox').bootstrapDualListbox()

    //Colorpicker
    $('.my-colorpicker1').colorpicker()
    //color picker with addon
    $('.my-colorpicker2').colorpicker()

    $('.my-colorpicker2').on('colorpickerChange', function(event) {
      $('.my-colorpicker2 .fa-square').css('color', event.color.toString());
    });

    $("input[data-bootstrap-switch]").each(function(){
      $(this).bootstrapSwitch('state', $(this).prop('checked'));
    });

}

window.loadFormsEditors = function () {
    // Summernote
    $('.textarea').summernote()
}

window.loadFormsValidation = function () {
  $.validator.setDefaults({
    submitHandler: function () {
      alert( "Form successful submitted!" );
    }
  });
  $('#quickForm').validate({
    rules: {
      email: {
        required: true,
        email: true,
      },
      password: {
        required: true,
        minlength: 5
      },
      terms: {
        required: true
      },
    },
    messages: {
      email: {
        required: "Please enter a email address",
        email: "Please enter a vaild email address"
      },
      password: {
        required: "Please provide a password",
        minlength: "Your password must be at least 5 characters long"
      },
      terms: "Please accept our terms"
    },
    errorElement: 'span',
    errorPlacement: function (error, element) {
      error.addClass('invalid-feedback');
      element.closest('.form-group').append(error);
    },
    highlight: function (element, errorClass, validClass) {
      $(element).addClass('is-invalid');
    },
    unhighlight: function (element, errorClass, validClass) {
      $(element).removeClass('is-invalid');
    }
  });
};

window.loadTablesData = function () {
    $("#example1").DataTable({
      "responsive": true,
      "autoWidth": false,
    });
    $('#example2').DataTable({
      "paging": true,
      "lengthChange": false,
      "searching": false,
      "ordering": true,
      "info": true,
      "autoWidth": false,
      "responsive": true,
    });
};

window.loadTablesJsGrid = function () {
    $("#jsGrid1").jsGrid({
        height: "100%",
        width: "100%",
 
        sorting: true,
        paging: true,
 
        data: db.clients,
 
        fields: [
            { name: "Name", type: "text", width: 150 },
            { name: "Age", type: "number", width: 50 },
            { name: "Address", type: "text", width: 200 },
            { name: "Country", type: "select", items: db.countries, valueField: "Id", textField: "Name" },
            { name: "Married", type: "checkbox", title: "Is Married" }
        ]
    });
};

window.loadExamCalendar = function () {

    /* initialize the external events
     -----------------------------------------------------------------*/
    function ini_events(ele) {
      ele.each(function () {

        // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
        // it doesn't need to have a start or end
        var eventObject = {
          title: $.trim($(this).text()) // use the element's text as the event title
        }

        // store the Event Object in the DOM element so we can get to it later
        $(this).data('eventObject', eventObject)

        // make the event draggable using jQuery UI
        $(this).draggable({
          zIndex        : 1070,
          revert        : true, // will cause the event to go back to its
          revertDuration: 0  //  original position after the drag
        })

      })
    }

    ini_events($('#external-events div.external-event'))

    /* initialize the calendar
     -----------------------------------------------------------------*/
    //Date for the calendar events (dummy data)
    var date = new Date()
    var d    = date.getDate(),
        m    = date.getMonth(),
        y    = date.getFullYear()

    var Calendar = FullCalendar.Calendar;
    var Draggable = FullCalendarInteraction.Draggable;

    var containerEl = document.getElementById('external-events');
    var checkbox = document.getElementById('drop-remove');
    var calendarEl = document.getElementById('calendar');

    // initialize the external events
    // -----------------------------------------------------------------

    new Draggable(containerEl, {
      itemSelector: '.external-event',
      eventData: function(eventEl) {
        console.log(eventEl);
        return {
          title: eventEl.innerText,
          backgroundColor: window.getComputedStyle( eventEl ,null).getPropertyValue('background-color'),
          borderColor: window.getComputedStyle( eventEl ,null).getPropertyValue('background-color'),
          textColor: window.getComputedStyle( eventEl ,null).getPropertyValue('color'),
        };
      }
    });

    var calendar = new Calendar(calendarEl, {
      plugins: [ 'bootstrap', 'interaction', 'dayGrid', 'timeGrid' ],
      header    : {
        left  : 'prev,next today',
        center: 'title',
        right : 'dayGridMonth,timeGridWeek,timeGridDay'
      },
      'themeSystem': 'bootstrap',
      //Random default events
      events    : [
        {
          title          : 'All Day Event',
          start          : new Date(y, m, 1),
          backgroundColor: '#f56954', //red
          borderColor    : '#f56954', //red
          allDay         : true
        },
        {
          title          : 'Long Event',
          start          : new Date(y, m, d - 5),
          end            : new Date(y, m, d - 2),
          backgroundColor: '#f39c12', //yellow
          borderColor    : '#f39c12' //yellow
        },
        {
          title          : 'Meeting',
          start          : new Date(y, m, d, 10, 30),
          allDay         : false,
          backgroundColor: '#0073b7', //Blue
          borderColor    : '#0073b7' //Blue
        },
        {
          title          : 'Lunch',
          start          : new Date(y, m, d, 12, 0),
          end            : new Date(y, m, d, 14, 0),
          allDay         : false,
          backgroundColor: '#00c0ef', //Info (aqua)
          borderColor    : '#00c0ef' //Info (aqua)
        },
        {
          title          : 'Birthday Party',
          start          : new Date(y, m, d + 1, 19, 0),
          end            : new Date(y, m, d + 1, 22, 30),
          allDay         : false,
          backgroundColor: '#00a65a', //Success (green)
          borderColor    : '#00a65a' //Success (green)
        },
        {
          title          : 'Click for Google',
          start          : new Date(y, m, 28),
          end            : new Date(y, m, 29),
          url            : 'http://google.com/',
          backgroundColor: '#3c8dbc', //Primary (light-blue)
          borderColor    : '#3c8dbc' //Primary (light-blue)
        }
      ],
      editable  : true,
      droppable : true, // this allows things to be dropped onto the calendar !!!
      drop      : function(info) {
        // is the "remove after drop" checkbox checked?
        if (checkbox.checked) {
          // if so, remove the element from the "Draggable Events" list
          info.draggedEl.parentNode.removeChild(info.draggedEl);
        }
      }    
    });

    calendar.render();
    // $('#calendar').fullCalendar()

    /* ADDING EVENTS */
    var currColor = '#3c8dbc' //Red by default
    //Color chooser button
    var colorChooser = $('#color-chooser-btn')
    $('#color-chooser > li > a').click(function (e) {
      e.preventDefault()
      //Save color
      currColor = $(this).css('color')
      //Add color effect to button
      $('#add-new-event').css({
        'background-color': currColor,
        'border-color'    : currColor
      })
    })
    $('#add-new-event').click(function (e) {
      e.preventDefault()
      //Get value and make sure it is not null
      var val = $('#new-event').val()
      if (val.length == 0) {
        return
      }

      //Create events
      var event = $('<div />')
      event.css({
        'background-color': currColor,
        'border-color'    : currColor,
        'color'           : '#fff'
      }).addClass('external-event')
      event.html(val)
      $('#external-events').prepend(event)

      //Add draggable funtionality
      ini_events(event)

      //Remove event from text input
      $('#new-event').val('')
    })
}

window.loadExamGallery = function () {
    $(document).on('click', '[data-toggle="lightbox"]', function(event) {
      event.preventDefault();
      $(this).ekkoLightbox({
        alwaysShowClose: true
      });
    });

    $('.filter-container').filterizr({gutterPixels: 3});
    $('.btn[data-filter]').on('click', function() {
      $('.btn[data-filter]').removeClass('active');
      $(this).addClass('active');
    });
}

window.loadMailBoxInBox = function () {
    //Enable check and uncheck all functionality
    $('.checkbox-toggle').click(function () {
      var clicks = $(this).data('clicks')
      if (clicks) {
        //Uncheck all checkboxes
        $('.mailbox-messages input[type=\'checkbox\']').prop('checked', false)
        $('.checkbox-toggle .far.fa-check-square').removeClass('fa-check-square').addClass('fa-square')
      } else {
        //Check all checkboxes
        $('.mailbox-messages input[type=\'checkbox\']').prop('checked', true)
        $('.checkbox-toggle .far.fa-square').removeClass('fa-square').addClass('fa-check-square')
      }
      $(this).data('clicks', !clicks)
    })

    //Handle starring for glyphicon and font awesome
    $('.mailbox-star').click(function (e) {
      e.preventDefault()
      //detect type
      var $this = $(this).find('a > i')
      var glyph = $this.hasClass('glyphicon')
      var fa    = $this.hasClass('fa')

      //Switch states
      if (glyph) {
        $this.toggleClass('glyphicon-star')
        $this.toggleClass('glyphicon-star-empty')
      }

      if (fa) {
        $this.toggleClass('fa-star')
        $this.toggleClass('fa-star-o')
      }
    })
}

window.loadMailBoxCompose = function () {
    //Add text editor
    $('#compose-textarea').summernote()
}

window.loadPagePrint = () => {
  window.addEventListener("load", window.print());
}

window.loadFileReaderService = () => {
  var FileReaderComponent = (function () {
      function FileReaderComponent() {
          var _this = this;
          this.newFileStreamReference = 0;
          this.fileStreams = {};
          this.dragElements = new Map();
          this.dragTargetElements = new Set();
          this.elementDataTransfers = new Map();

          this.initFileReaderService = function (targetId) {

              var elementReal = this.GetDragTargetElement();
              if (elementReal != null) {
                  _this.elementDataTransfers.delete(elementReal);
                  _this.dragElements.delete(elementReal);
              }      
              //원래주석 제거함
              _this.elementDataTransfers.set(elementReal, null);

              this.newFileStreamReference = 0;
              this.fileStreams = {};
              this.dragElements = new Map();
              this.dragTargetElements = new Set();
              this.elementDataTransfers = new Map();
              return true;
          };

          this.SetDragTargetElement = function (targetId) {
              this.dragTargetElements.add(targetId);
              return true;
          };
          this.DelDragTargetElement = function (targetId) {
              this.dragTargetElements.delete(targetId);
              return true;
          };
          this.GetDragTargetElement = function () {
              var targetElement;
              for (var targetId of this.dragTargetElements) {
                if((targetElement = document.getElementById(targetId))) return targetElement;
              }
              return null;
          };
          this.RegisterDropEvents = function (element, additive) {
              var elementReal = this.GetDragTargetElement();
              if(elementReal == null) return false;

              _this.LogIfNull(elementReal);
              var handler = function (ev) {
                  //_this.PreventDefaultHandler(ev);
                  DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "Drop Handler Called...");
                  var items = ev.dataTransfer.items;
                  
                  if (items == null)
                      DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS OnDrop Folder] dataTransfer items is NULL" );
                  for (var i = 0, item; item = items[i]; ++i) {
                      //var entry = item.webkitGetAsEntry();
                      //if (entry.isDirectory) {

                      if (item.kind != "file") {
                          DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS OnDrop Folder]");
                          var folder = item.getAsString();
                          console.log("[JS OnDrop Folder] folder name:" + folder);
                      }
                      else
                      {
                          var file = item.getAsFile();
                          DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS OnDrop File]" + file.name);
                          console.log("[JS OnDrop File] file name:" + file.name);
                      }
                  }

                  if (ev.target instanceof HTMLElement) {
                      var list = ev.dataTransfer.files;

                      /*if (additive) {
                          var existing = _this.elementDataTransfers.get(elementReal);
                          if (existing !== undefined && existing.length > 0) {
                              list = new FileReaderComponent.ConcatFileList(existing, list);
                          }
                      }*/
                      //_this.elementDataTransfers.set(elementReal, list);
                  }
              };
              _this.dragElements.set(elementReal, handler);
              elementReal.addEventListener("drop", handler);
              elementReal.addEventListener("dragover", _this.PreventDefaultHandler);
              return true;
          };
          this.UnregisterDropEvents = function (element) {
              var elementReal = this.GetDragTargetElement();
              if(elementReal == null) return false;

              _this.LogIfNull(elementReal);
              var handler = _this.dragElements.get(elementReal);
              if (handler) {
                  elementReal.removeEventListener("drop", handler);
                  elementReal.removeEventListener("dragover", _this.PreventDefaultHandler);
              }
              _this.elementDataTransfers.delete(elementReal);
              _this.dragElements.delete(elementReal);
              return true;
          };
          this.GetFileCount = function (element) {
              var elementReal = this.GetDragTargetElement();
              if(elementReal == null) return 0;

              _this.LogIfNull(elementReal);
              var files = _this.GetFiles(elementReal);
              if (!files) {
                  return -1;
              }
              var result = files.length;
              //alert(result);
              return result;
          };
          this.ClearValue = function (element) {
              var elementReal = this.GetDragTargetElement();
              if(elementReal == null) return -1;

              _this.LogIfNull(elementReal);
              if (elementReal instanceof HTMLInputElement) {
                  elementReal.value = null;
              }
              else {
                  _this.elementDataTransfers.delete(elementReal);
              }

              _this.elementDataTransfers = null;
              _this.elementDataTransfers = new Map();
              return 0;
          };
          this.GetFileInfoFromElement = function (element, index) {
              var elementReal = this.GetDragTargetElement();
              if(elementReal == null) return null;

              _this.LogIfNull(elementReal);
              var files = _this.GetFiles(elementReal);
              if (!files) {
                  return null;
              }

              var file = null;
              console.log("배열여부:" + Array.isArray(files));
              if (Array.isArray(files)) {
                  file = files[index];
              }
              else {
                  file = files.item(index);
              }
              if (!file) {
                  return null;
              }

              var tarName = file.name;
              var isDir = "File";
              var entries = elementReal.webkitEntries;
              for (var i = 0; i < entries.length; ++i) {
                  if (entries[i].name != tarName)
                      continue;
                  else {
                      if (entries[i].isDirectory)
                          isDir = "Dir";
                  }
              }
              return _this.GetFileInfoFromFile(file, isDir);
          };
          this.Dispose = function (fileRef) {
              //return delete (_this.fileStreams[fileRef]);
              delete (_this.fileStreams[fileRef]);
              return 0;
          };
          this.OpenRead = function (element, fileIndex) {
              var elementReal = this.GetDragTargetElement();
              if(elementReal == null) return null;

              _this.LogIfNull(elementReal);
              var files = _this.GetFiles(elementReal);
              if (!files) {
                  throw 'No FileList available.';
              }
              var file = null;
              if (Array.isArray(files)) {
                  file = files[fileIndex];   
              }
              else {
                  file = files.item(fileIndex);
              }  

              if (!file) {
                  throw "No file with index " + fileIndex + " available.";
              }
              var fileRef = _this.newFileStreamReference++;
              _this.fileStreams[fileRef] = file;
              return fileRef;
          };
          this.ReadFileParamsPointer = function (readFileParamsPointer) {
              return {
                  taskId: Blazor.platform.readUint64Field(readFileParamsPointer, 0),
                  bufferOffset: Blazor.platform.readUint64Field(readFileParamsPointer, 8),
                  count: Blazor.platform.readInt32Field(readFileParamsPointer, 16),
                  fileRef: Blazor.platform.readInt32Field(readFileParamsPointer, 20),
                  position: Blazor.platform.readUint64Field(readFileParamsPointer, 24),
                  buffer: Blazor.platform.readInt32Field(readFileParamsPointer, 32)
              };
          };
          this.ReadFileUnmarshalledAsync = function (readFileParamsPointer) {
              var readFileParams = _this.ReadFileParamsPointer(readFileParamsPointer);
              var asyncCall = new Promise(function (resolve, reject) {
                  return _this.ReadFileSlice(readFileParams, function (r, b) { return r.readAsArrayBuffer(b); })
                      .then(function (r) {
                      try {
                          var dotNetBufferView = Blazor.platform.toUint8Array(readFileParams.buffer);
                          var arrayBuffer = r.result;
                          dotNetBufferView.set(new Uint8Array(arrayBuffer), readFileParams.bufferOffset);
                          var byteCount = Math.min(arrayBuffer.byteLength, readFileParams.count);
                          resolve(byteCount);
                      }
                      catch (e) {
                          reject(e);
                      }
                  }, function (e) { return reject(e); });
              });
              asyncCall.then(function (byteCount) { return DotNet.invokeMethodAsync("Blazor.FileReader", "EndReadFileUnmarshalledAsyncResult", readFileParams.taskId, byteCount); }, function (error) {
                  console.error("ReadFileUnmarshalledAsync error", error);
                  DotNet.invokeMethodAsync("Blazor.FileReader", "EndReadFileUnmarshalledAsyncError", readFileParams.taskId, error.toString());
              });
          };
          this.ReadFileMarshalledAsync = function (readFileParams) {
              return new Promise(function (resolve, reject) {
                  return _this.ReadFileSlice(readFileParams, function (r, b) { return r.readAsDataURL(b); })
                      .then(function (r) {
                      var contents = r.result;
                      var data = contents ? contents.split(";base64,")[1] : null;
                      resolve(data);
                  }, function (e) { return reject(e); });
              });
          };
          this.ReadFileSlice = function (readFileParams, method) {
              return new Promise(function (resolve, reject) {
                  var file = _this.fileStreams[readFileParams.fileRef];
                  try {
                      var reader = new FileReader();
                      reader.onload = (function (r) {
                          return function () {
                              try {
                                  resolve({ result: r.result, file: file });
                              }
                              catch (e) {
                                  reject(e);
                              }
                          };
                      })(reader);
                      method(reader, file.slice(readFileParams.position, readFileParams.position + readFileParams.count));
                  }
                  catch (e) {
                      reject(e);
                  }
              });
          };
          this.PreventDefaultHandler = function (ev) {
              ev.preventDefault();
          };
          this.IsDragTargetElement = function (targetId) {

              DirSubFiles.paths = [];
              DirSubFiles.items = [];
              DirSubFiles.use = [];
              var element = this.GetDragTargetElement();
              var entries = element.webkitEntries;
              DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS IsDragTargetElement]Start WebkitEntries");
              for (var i = 0; i < entries.length; ++i) {
                  if (entries[i].isDirectory) {
                      DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS IsDragTargetElement]디렉토리:" + entries[i].name);
                      traverseFileTree(entries[i]);
                  }
              }
              DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS IsDragTargetElement]End WebkitEntries");

              DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS IsDragTargetElement]Start HTMLInputElement");
              if (element instanceof HTMLInputElement) {
                  files = element.files;
                  for (var i = 0; i < files.length; i++) {
                      DirSubFiles.paths.push("");
                      DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS IsDragTargetElement]Root 개체" + files[i].name);
                      DirSubFiles.items.push(files[i]);
                      DirSubFiles.use.push("n");
                  }
              }
              DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS IsDragTargetElement]End HTMLInputElement");
              return this.dragTargetElements.has(targetId);
          };
          this.AppendDragTargetElement = function (targetId) {
              var elementReal = this.GetDragTargetElement();
              DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS AppendDragTargetElement]디렉토리 파일갯수:" + DirSubFiles.items.length);
              for (var i = 0; i < DirSubFiles.items.length; i++) {
                  DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS AppendDragTargetElement]파일정보:" + DirSubFiles.paths[i] + DirSubFiles.items[i].name);
              }  
              /*
              var existing = _this.elementDataTransfers.get(elementReal);
              if (existing !== undefined && existing.length > 0) {
                  list = new FileReaderComponent.ConcatFileList(existing, DirSubFiles.items);
                  _this.elementDataTransfers.set(elementReal, list);
              }
              else
              */
              _this.elementDataTransfers.set(elementReal, DirSubFiles.items);

              return true;
          };
      }

      var DirSubFiles =
      {
          paths : [],
          items: [],
          use: [],
          file:[]
      };
      function traverseFileTree(item, path) {
          path = path || "";
          if (item.isFile) {
              // Get file
              item.file(function (file) {
                  //console.log("File:", path + file.name);
                  DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS traverseFileTree]디렉토리 검색(파일):" + path + file.name + " File:YES");
                  DirSubFiles.paths.push(path);
                  DirSubFiles.items.push(file);
                  DirSubFiles.use.push("n");
                  DirSubFiles.file.push("y");
              });
          } else if (item.isDirectory) {
              // Get folder contents
              var dirReader = item.createReader();
              dirReader.readEntries(function (entries) {
                  DotNet.invokeMethodAsync("OpenNetLinkApp", "LogWrite", "[JS traverseFileTree]디렉토리 검색(디렉토리):" + path + item.name + "/" + " FILE:NO");
                  for (var i = 0; i < entries.length; i++) {
                      traverseFileTree(entries[i], path + item.name + "/");
                  }
              });
          }
      };

      FileReaderComponent.prototype.LogIfNull = function (element) {
          if (element == null) {
              console.log("BlazorFileReader HTMLElement is null. Can't access IFileReaderRef after HTMLElement was removed from DOM.");
          }
      };
                  
      FileReaderComponent.prototype.GetFiles = function (element) {
          var files = null;
          //Input 테그에서 값을 가져오면 Drop 하거나 폴더내부 파일처리에 문제가 있어서 주석처리하고 DataTransfer에서만 가져오게 수정
          var dataTransfer = this.elementDataTransfers.get(element);
          if (dataTransfer) {
              files = dataTransfer;
          }
          /*if (element instanceof HTMLInputElement) {
              files = element.files;
          }
          else
          {
              var dataTransfer = this.elementDataTransfers.get(element);
              if (dataTransfer) {
                  files = dataTransfer;
              }
          }*/
          return files;
      };
      FileReaderComponent.prototype.GetFileInfoFromFile = function (file, dir) {
          var filePath = "";
          for (var i = 0; i < DirSubFiles.items.length; i++) {
              if (file.name == DirSubFiles.items[i].name && DirSubFiles.use[i] == "n") {
                  DirSubFiles.use[i] = "y"
                  filePath = DirSubFiles.paths[i];
                  break;
              }
          }

          var result = {
              lastModified: file.lastModified,
              name: file.name,
              nonStandardProperties: null,
              size: file.size,
              type: file.type,
              Dir: dir,
              Path: filePath
          };
          var properties = {};
          for (var property in file) {
              if (Object.getPrototypeOf(file).hasOwnProperty(property) && !(property in result)) {
                  properties[property] = file[property];
              }
          }
          result.nonStandardProperties = properties;
          return result;
      };
      FileReaderComponent.ConcatFileList = (function () {
          function class_1(existing, additions) {
              for (var i = 0; i < existing.length; i++) {
                  this[i] = existing[i];
              }
              var eligebleAdditions = [];
              for (var i = 0; i < additions.length; i++) {
                  var exists = false;
                  var addition = additions[i];
                  for (var j = 0; j < existing.length; j++) {
                      if (existing[j].name == addition.name) {
                          exists = true;
                          break;
                      }
                      if (existing[j] === addition) {
                          exists = true;
                          break;
                      }
                  }
                  if (!exists) {
                      eligebleAdditions[eligebleAdditions.length] = addition;
                  }
              }
              for (var i = 0; i < eligebleAdditions.length; i++) {
                  this[i + existing.length] = eligebleAdditions[i];
              }
              this.length = existing.length + eligebleAdditions.length;
          }
          class_1.prototype.item = function (index) {
              return this[index];
          };
          return class_1;
      }());
      return FileReaderComponent;
  }());
window.FileReaderComponent = new FileReaderComponent();
//# sourceMappingURL=FileReaderComponent.js.map
}