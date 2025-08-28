CKEDITOR.plugins.add('axscontokens', {
    requires: ['richcombo'], //, 'styles' ],
    init: function(editor) {
        var config = editor.config,
            lang = editor.lang.format;

        // Gets the list of tags from the settings.
        //this.add('value', 'drop_text', 'drop_label');
        const tags = [
            ["[[solicitud_numero]]", "Número Solicitud", "Número de la Solicitud"],
            ["[[solicitud_fecha]]", "Fecha de Solicitud", "Fecha en la que se emitió la Solicitud"],
            ["[[solicitud_solicitante]]", "Solicitante", "Nombre del Solicitante"],

            ["[[solicitud_para]]", "Solicitado para", "Nombre del Destinatario"],
            ["[[solicitud_para_documento_identidad]]", "Documento Identidad", "Documento Identidad del Destinatario"],
            ["[[solicitud_para_fecha_ingreso]]", "Solicitado para Fecha Ingreso", "Fecha de ingreso del Destinatario (Colaborador)"],
            ["[[solicitud_para_cargo]]", "Solicitante Cargo", "Cargo del Destinatario"],
            
            ["[[solicitud_observacion]]", "Observaciones de Solicitud", "Observaciones de la Solicitud"],
            ["[[solicitud_aprobacion_enlace]]", "Enlace a Aprobación de Solicitud", "Enlace a la página de Aprobación de Solicitud"],
            ["[[solicitud_enlace]]", "Enlace a la Solicitud", "Enlace a la Solicitud"],
            ["[[solicitud_tipo]]", "Tipo de Solicitud", "Tipo de Solicitud (ALTA/MODIF./BAJA)"],
            ["[[solicitud_detalle]]", "Detalle de Solicitud", "Lista del detalle de la Solicitud"],
            ["[[solicitud_detalle_aprobados]]", "Detalle de Solicitud Aprobados", "Lista de los detalles aprobados de la Solicitud"],
            ["[[solicitud_historia]]", "Historial de Solicitud", "Historia detallada de la Solicitud"],
            ["[[solicitud_ticket]]", "Ticket Atención", "Ticket Atención"],
            ["[[solicitud_usuario_oracle]]", "Usuario Oracle", "Usuario Oracle"],
            ["[[solicitud_menu_oracle]]", "Menu Oracle", "Menu Oracle"],
            ["[[archivo_adjunto]]", "Archivo Adjunto", "Archivo Adjunto"],
            ["[[solicitud_empresa]]", "Empresa", "Empresa"],            
            ["[[solicitud_pais]]", "País", "País"]
        ]; //new Array();

        // Create style objects for all defined styles.
        editor.ui.addRichCombo('axscontokens', {
            label: "Solicitud",
            title: "Insertar información de Solicitud",
            voiceLabel: "Insertar información de Solicitud",
            className: 'cke_format',
            multiSelect: false,

            panel: {
                //css: [config.contentsCss, CKEDITOR.getUrl(editor.skinPath + 'editor.css')],
                css: [config.contentsCss, CKEDITOR.getUrl(CKEDITOR.skin.getPath('editor'))],
                voiceLabel: lang.panelVoiceLabel
            },

            init: function() {
                this.startGroup("Info. Solicitud");
                //this.add('value', 'drop_text', 'drop_label');
                for (var this_tag in tags) {
                    this.add(
                        tags[this_tag][0],
                        tags[this_tag][1],
                        tags[this_tag][2],
                        tags[this_tag][3], tags[this_tag][4], tags[this_tag][5], tags[this_tag][6]
                    );
                }
            },

            onClick: function(value) {
                editor.focus();
                editor.fire('saveSnapshot');
                editor.insertHtml(value);
                editor.fire('saveSnapshot');
            }
        });
    }
});