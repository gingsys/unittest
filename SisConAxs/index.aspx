<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SisConAxs.index" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="icon" type="image/png" href="content/images/favicon.png" />
    <title>Interfaz Control de Accesos y Registro Único de Solicitudes</title>

    <link rel="stylesheet" href="dist/libraries.min.css" />

    <!-- <link rel="stylesheet" href="dist/app.min.css" /> -->
    <link rel="stylesheet" href="content/app.css" />

    <!-- msal.js with a fallback to backup CDN -->
    <script type="text/javascript" src="https://alcdn.msauth.net/lib/1.2.1/js/msal.js" integrity="sha384-9TV1245fz+BaI+VvCjMYL0YDMElLBwNS84v3mY57pXNOt6xcUYch2QLImaTahcOP" crossorigin="anonymous"></script>
    <script type="text/javascript">
        if (typeof Msal === 'undefined') document.write(unescape("%3Cscript src='https://alcdn.msftauth.net/lib/1.2.1/js/msal.js' type='text/javascript' integrity='sha384-m/3NDUcz4krpIIiHgpeO0O8uxSghb+lfBTngquAo2Zuy2fEF+YgFeP08PWFo5FiJ' crossorigin='anonymous'%3E%3C/script%3E"));

        // global
        <% if (this.isRelease)
        { %> 
        var __ENVIROMENT__ = "RELEASE";
        <% }
        else
        {%>
        var __ENVIROMENT__ = "DEBUG";
        <% } %>
    </script>
</head>
<body data-presenter="app">
    <div class="wrapper">
        <app-presenter id="app"></app-presenter>

        <script src="dist/libraries.min.js"></script>
        <script src="libs/ckeditor/ckeditor.js"></script>
        <script src="libs/ckeditor/ckeditor-plugins.min.js"></script>
        <% if (this.isRelease)
        { %> 
            <script src="dist/app.min.js" type="module"></script>
        <% }
            else
        { %>
            <script src="app/index.js" type="module"></script>
        <% } %>

        <!-- importing app scripts (load order is important) -->
        <!-- <script type="text/javascript" src="app/components/authConfig.js"></script> -->
        <%--<script type="text/javascript" src="app/components/graphConfig.js"></script>--%>
        <!-- <script type="text/javascript" src="app/components/ui.js"></script> -->

        <!-- replace next line with authRedirect.js if you would like to use the redirect flow -->
        <!-- <script type="text/javascript" src="./authRedirect.js"></script> -->
        <!-- <script type="text/javascript" src="app/components/authPopup.js"></script> -->
        <%--<script type="text/javascript" src="app/components/graph.js"></script>--%>
</body>
</html>