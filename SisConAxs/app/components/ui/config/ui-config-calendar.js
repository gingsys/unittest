import { Component, html } from '../../../core/core.js';
import { CalendarResource, Calendar, CalendarFilter } from '../../resources/resource-calendar.js';
import { CommonValueResource, CommonValueSet } from '../../resources/resource-commonvalue.js';
import { SelectCommonValue } from '../commonvalues/ui-commonvalue.js';

export class CalendarComponent extends Component {
    constructor() {
        super(false);
        this.$resource = new CalendarResource();
        this.$entity = new Calendar();
    }

    get $template() {
        return html`
        <h3>Calendario</h3>
        <div class="row">
        <div class="form-group col-sm-3">
            <label for="cboCountry" class="col-form-label">País</label>
            <div class="input-group mb-3" style="margin: 0px !important;">
                <control-select id="cboCountry"
                    data-none-selected-text="Seleccione país"
                    data-live-search="true"
                    data-size="8"
                    @select=${this.loadCalendar}
                    >
                </control-select>
            </div>
        </div>
        <div class="form-group col-sm-1">
            <label for="cboYear" class="col-form-label">Año</label>
                <div class="input-group mb-3" style="margin: 0px !important;">
                <select id="cboYear" class="form-control" data-size="8" @change=${this.changeYear}>
                    ${
                        [0,1,2,3].map(x => {
                        let date = new Date();
                        let year = date.getFullYear() - x;
                        return html`<option value="${year}">${year}</option>`;
                        })
                    }
                </select>
            </div>
        </div>
        </div>
        <div id='calendar'></div>`;
    }

    connectedCallback() {
        super.connectedCallback();

        CommonValueResource.fillControlSelectData(this.$.cboCountry, CommonValueSet.SET_PAISES);
        this.createCalendar();
    }

    changeYear() {
        this.calendar.destroy();
        this.createCalendar();
        this.loadCalendar();
    }

    createCalendar() {
        const $this = this;
        const initialLocaleCode = 'es';
        this.calendar = new FullCalendar.Calendar(this.$.calendar, {
            height: 450,
            plugins: ['dayGrid', 'interaction'],
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'year,month,basicWeek,basicDay'
            },
            locale: initialLocaleCode,
            validRange: {
                start: $this.$.cboYear.value + '-01-01',
                end: $this.$.cboYear.value + '-12-31'
            },

            dateClick: function(info) {
                iziToast.info({
                    timeout: false,
                    overlay: true,
                    displayMode: 'once',
                    id: 'inputs',
                    zindex: 999,
                    title: 'Descripción: ' + info.dateStr,
                    message: '',
                    position: 'center',
                    drag: true,
                    inputs: [
                        ['<input type="text" name="questionInput">', 'keyup', function(instance, toast, input, e) {}, true],
                        ['<label>Es anual</label>', '', function(instance, toast, input, e) {}],
                        ['<input type="checkbox">', 'change', function(instance, toast, input, e) {}],
                        ['<input type="button" value="Guardar">', 'click', function(instance, toast, input, e, inputs) {
                            var value = $(input.parentElement)[0].children[0].value;
                            if (value != null && $this.$.cboCountry.value > 0) {
                                $this.$entity.CalDate = info.dateStr;
                                $this.$entity.CalDescription = value;
                                $this.$entity.CalAnual = $(input.parentElement)[0].children[2].checked;
                                $this.$entity.CalActive = 1;
                                $this.$entity.CalIdCountry = $this.$.cboCountry.value;

                                let $instance = instance;
                                $this.$presenter.showLoading();
                                $this.$resource.post($this.$entity).then(
                                    r => {
                                        $instance.hide({
                                            transitionOut: 'fadeOutUp',
                                            onClosing: function(instance, toast, closedBy) {}
                                        }, toast, 'buttonName');
                                        $this.$presenter.hideLoading();
                                        iziToast.success({ title: 'Grabado correctamente' });
                                        $this.loadCalendar();
                                    },
                                    err => {
                                        $this.$presenter.hideLoading();
                                        console.error('error!', err);
                                        iziToast.error({ title: err });
                                    }
                                );
                            }else{
                                iziToast.error({ title: "Debe seleccionar un país." });
                            }
                        }]
                    ]
                });
            },
            eventRender: function(info) {
                $(info.el).find(".fc-title").prepend("<i class='fa fa-times' style='float: right;cursor: pointer;'></i>");
            },
            eventClick: function(calEvent, jsEvent, view) {
                $this.$presenter.question(
                    'Borrar',
                    'Desea borrar este evento?',
                    (instance, toast) => {
                        if (calEvent.event.extendedProps.CalID != undefined) {
                            $this.$presenter.showLoading();
                            $this.$resource.delete(calEvent.event.extendedProps.CalID).then(
                                r => {
                                    $this.$presenter.hideLoading();
                                    console.log('borrado!');
                                    iziToast.success({ title: 'Borrado correctamente' });
                                    $this.loadCalendar();
                                },
                                err => {
                                    $this.$presenter.hideLoading();
                                    console.error('error!', err);
                                    iziToast.error({ title: err });
                                }
                            );
                        } else {
                            iziToast.success({ title: 'Borrado correctamente' });
                            calEvent.event.remove();
                        }
                    }
                );
            },
            selectable: true,
            editable: false,
            // droppable: true
        });
        this.calendar.render();
    }

    loadCalendar() {
        this.$presenter.showLoading();
        this.calendar.removeAllEvents();
        this.$resource.listCalendar({
                country: this.$.cboCountry.value,
                year: this.$.cboYear.value,
                status: 1
            }).then(r => {
                const list = Calendar.fromList(r);
                list.forEach(event => {
                    this.calendar.addEvent({
                        title: event.CalDescription,
                        // description: ($(input.parentElement)[0].children[2].checked ? "Anual" : ""),
                        start: event.CalDate,
                        allDay: true,
                        textColor: "#FFFFFF",
                        borderColor: (event.CalAnual ? "darkorange" : null),
                        backgroundColor: (event.CalAnual ? "darkorange" : null),
                        extendedProps: { CalID: event.CalID }
                    });
                });
                this.calendar.render();
                this.$presenter.hideLoading();
            })
            .catch(err => {
                this.$presenter.hideLoading();
                console.error(err);
            });
    }
}
customElements.define('config-calendar', CalendarComponent);