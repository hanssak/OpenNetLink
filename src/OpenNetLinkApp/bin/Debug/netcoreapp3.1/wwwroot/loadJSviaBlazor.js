window.loadJSviaBlazor = function() {
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
            scriptElement.src = "plugins/chart.js/Chart.min.js";
            document.head.appendChild(scriptElement);
        }, 150);

        /*
        setTimeout(function() {
            var scriptElement=document.createElement('script');
            scriptElement.type = 'text/javascript';
            scriptElement.src = "js/adminlte/pages/dashboard2.js";
            document.head.appendChild(scriptElement);
        }, 150);
        */

    }, 300);
};

window.loadTreeView = function(){
        const trees = $('[data-widget="treeview"]');
        trees.Treeview('init');
};