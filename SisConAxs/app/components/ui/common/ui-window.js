import { Component, ComponentView, html } from "../../../core/core.js";
 
export class UIWindowComponent extends Component {
    static get SIZE_SMALL() { return 'modal-sm'; }
    static get SIZE_NORMAL() { return ''; }
    static get SIZE_LARGE() { return 'modal-lg'; }
    static get SIZE_XLARGE() { return 'modal-xl'; }

    static get SCROLL_NORMAL() { return ''; }
    static get SCROLL_SCROLLABLE() { return 'modal-dialog-scrollable'; }

    static get VERTICAL_CENTERED_NO() { return ''; }
    static get VERTICAL_CENTERED() { return 'modal-dialog-centered'; }

    constructor(shadow = false, options = {}) {
        super(shadow);
        // const options = { size: UIWindowComponent.SIZE_XLARGE, centered: UIWindowComponent.VERTICAL_CENTERED, scroll: UIWindowComponent.SCROLL_SCROLLABLE };
        this.__static     = options.static || true;
        this.__size       = options.size || UIWindowComponent.SIZE_NORMAL;
        this.__scroll     = options.scroll || UIWindowComponent.SCROLL_NORMAL;
        this.__centered   = options.centered || UIWindowComponent.VERTICAL_CENTERED_NO;
    }

    get $template() {
        return new UIWindowComponentView(this);
    }
    get __modal() {
        return this.$one('[data-window-modal]');
    }

    connectedCallback() {
        super.connectedCallback();
    }

    show() {
        $(this.__modal).modal('show'); //{ backdrop: 'static' });
    }
    hide() {
        $(this.__modal).modal('hide');
    }
}
customElements.define('ui-window', UIWindowComponent);


export class UIWindowComponentView extends ComponentView {
    get header() {
        return html`<h4 class="modal-title">Window Test</h4>`;
    }
    get body() {
        return html`
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris purus erat, tempor ornare euismod sed, hendrerit nec nisl. Fusce varius faucibus eros, sit amet auctor felis rutrum vestibulum. Quisque orci orci, interdum ac massa in, malesuada euismod augue. Pellentesque vitae est at purus tempor dignissim et eget nisi. Suspendisse maximus, velit et placerat volutpat, velit libero condimentum nulla, quis viverra ante massa in orci. Duis aliquam magna imperdiet pellentesque tempus. Nunc eleifend in nunc id auctor. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nunc nec tellus in nisi mattis vulputate. Pellentesque ac fringilla eros. Cras et elit vitae quam cursus rhoncus. Donec at ex ex.

Cras leo nulla, aliquam vel mi ac, congue feugiat tellus. Phasellus elementum eu dui a elementum. Maecenas sed condimentum massa. Pellentesque eget facilisis diam, a ultricies erat. Integer vitae aliquet lectus. Aliquam nulla leo, vestibulum ac lobortis et, ornare vitae nunc. Duis at suscipit ex. Duis aliquam orci velit, vel placerat mauris faucibus vel. Cras id mattis felis. Quisque fringilla orci quis quam dignissim faucibus. Fusce sem arcu, vulputate non tellus vel, fermentum congue nulla. Phasellus hendrerit, enim vitae bibendum fringilla, diam arcu pellentesque purus, vel egestas felis dui nec urna. Nulla varius aliquam dui, ac pulvinar purus interdum quis. Quisque lobortis dictum lorem ut dignissim. Mauris tincidunt metus luctus quam bibendum facilisis.

Nulla condimentum tempor augue, in sodales augue ultrices vitae. Morbi posuere ipsum at urna pulvinar ornare. Praesent in efficitur lectus, ut tempus nulla. Sed in pretium diam. Vestibulum mollis pharetra velit volutpat lobortis. Aenean egestas nulla non purus mattis, sit amet pharetra tellus laoreet. Donec quis nibh mi. Praesent sit amet ante leo. Maecenas fermentum est ut vehicula varius. Quisque rutrum efficitur felis, condimentum vehicula felis aliquet in. Fusce malesuada tristique pulvinar. Aenean eget finibus risus, a interdum quam. Nullam sed fringilla erat. Suspendisse sollicitudin neque at arcu semper, vitae dapibus ligula venenatis. Praesent pretium vestibulum nibh in fringilla. Nunc elit massa, sagittis sed egestas id, consectetur ut lorem.

Morbi dignissim orci odio, in rutrum odio convallis sed. Duis tincidunt nulla sed rutrum porttitor. Mauris fermentum placerat bibendum. Sed ac sem erat. Duis ac risus condimentum, egestas tortor vel, dictum mi. Cras vel magna non diam rhoncus blandit. Phasellus ac varius lectus, in venenatis dolor. Pellentesque venenatis sapien et metus commodo, sed posuere tellus dictum. Sed dapibus tellus varius, tristique elit pretium, rutrum odio. Ut quam tortor, imperdiet non egestas id, ultrices non erat. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Integer at semper neque. Fusce pulvinar eget nunc et dictum. Sed nec leo scelerisque, sagittis augue eu, hendrerit lacus. Sed varius ante vel nunc dapibus suscipit a non nunc.

Mauris feugiat neque sit amet massa tempor vulputate. Maecenas id consectetur nisi, at interdum neque. Quisque sapien libero, elementum non posuere eu, condimentum et urna. Sed auctor porta nunc id vulputate. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nunc risus sapien, aliquet at nisi quis, pharetra iaculis odio. In semper eros sed sem tristique, sed imperdiet lectus mollis. Proin ullamcorper mauris ut lectus hendrerit, sit amet varius nisl ullamcorper. Nullam auctor urna ut enim vehicula, et efficitur tellus scelerisque. Vivamus aliquam finibus sapien et rutrum. Sed sollicitudin aliquam velit. Nam tincidunt ipsum eu pulvinar tempor. Sed malesuada, sem a vulputate dictum, odio arcu rutrum nulla, egestas lobortis est odio non lectus. Quisque lectus purus, interdum id ipsum eu, congue feugiat mauris. Nunc lacinia nec tortor vel ornare.

Etiam pulvinar commodo mauris, vel tincidunt ipsum molestie sed. Praesent viverra nisl mauris, vitae ornare lacus ornare ac. Quisque et tortor felis. Maecenas augue ex, rhoncus at venenatis ultricies, pharetra in tortor. Nulla vitae tortor vel elit vulputate ornare. Nullam dui elit, imperdiet non tincidunt ac, iaculis vel urna. Aliquam enim nisi, imperdiet sed pretium et, tristique id lacus.

Ut odio purus, laoreet non risus euismod, vehicula viverra augue. Donec nisl justo, cursus at pretium ac, efficitur vel dui. Nam sem velit, sagittis at quam eu, tempus feugiat sapien. Phasellus in consectetur tellus, ut tincidunt lectus. Morbi sit amet tincidunt tellus. Quisque commodo est id tellus facilisis rhoncus. Nunc eleifend ex turpis, eget porttitor magna faucibus ut. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Pellentesque scelerisque nulla eu lorem aliquam, sed volutpat diam interdum. Mauris sollicitudin lorem non tellus tincidunt malesuada quis et ante. Nulla eget ex pulvinar lacus suscipit pulvinar. Ut convallis tortor eu risus aliquam bibendum. Nulla facilisi. Donec convallis nulla non eleifend condimentum. Vivamus vehicula diam eu risus sodales commodo. Donec facilisis eleifend egestas.

Nulla vel justo id sapien sodales finibus vitae eu turpis. Morbi a molestie orci. Maecenas sed imperdiet lorem. Nulla odio arcu, eleifend eget pellentesque vel, pulvinar sed risus. Suspendisse at condimentum ipsum. Nam molestie nunc id nunc porttitor rutrum. Aenean ultricies aliquet eros, eget euismod eros fringilla in. Pellentesque nec varius dolor.

Aenean ultricies neque vitae malesuada consequat. Fusce interdum, felis in volutpat iaculis, magna metus mollis dui, non scelerisque augue augue vel enim. Nullam consectetur leo et ante efficitur, vitae rutrum leo viverra. Sed ligula sapien, pulvinar quis scelerisque in, gravida eu eros. Proin sed pharetra eros. Nam pharetra mattis turpis sed imperdiet. Duis tincidunt lorem sed ullamcorper accumsan. Curabitur posuere scelerisque ante id sodales. Aliquam sit amet mi vitae elit tempor ultricies eu ut lacus. Vestibulum non porttitor sapien. Cras porta consectetur blandit. Vivamus dui quam, eleifend consequat lorem hendrerit, fermentum convallis sapien. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec sed convallis nunc, et vulputate ligula.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque sit amet interdum risus, eu vulputate sem. Integer lacinia urna vel est maximus, vel sagittis leo dictum. Donec lacinia massa vitae est bibendum, in blandit libero luctus. Quisque commodo, lorem in imperdiet sollicitudin, eros nisl sollicitudin orci, eget congue velit nunc eget sapien. Morbi sed risus mauris. Cras ut maximus libero. Cras dapibus nulla et nibh dignissim, vel condimentum lorem bibendum. Mauris maximus aliquam ligula ac ornare. Cras faucibus sapien non eros malesuada, a sodales velit viverra. Ut iaculis mauris nec odio mattis, sit amet condimentum magna accumsan. Aliquam at nibh blandit, pharetra quam a, viverra purus. Quisque faucibus placerat urna.

Integer feugiat molestie sagittis. Vivamus id lacus eget augue euismod rutrum eget ut nisl. Maecenas non convallis lorem. Morbi quis magna magna. Donec vel arcu nisi. Curabitur scelerisque, leo vitae aliquam sodales, nulla ipsum suscipit ipsum, vel gravida nunc sapien hendrerit justo. Nullam cursus, metus at semper commodo, turpis ipsum semper diam, vel fringilla massa ex vehicula metus. Nullam congue velit tincidunt dolor sollicitudin tristique.

Etiam viverra enim a pellentesque placerat. Duis erat velit, sollicitudin in pretium sit amet, iaculis sit amet ante. Suspendisse placerat velit ipsum, at porta justo pretium sit amet. Donec ornare eros eu condimentum sollicitudin. Nulla facilisi. In euismod dolor in sodales scelerisque. Quisque et turpis convallis nibh ultrices accumsan. Fusce facilisis sit amet augue ut porttitor. Phasellus lacinia bibendum egestas. Maecenas et lorem nec urna iaculis lacinia in sed magna. Phasellus sollicitudin purus vel mauris fermentum, vitae venenatis sapien pellentesque.

Nullam posuere ullamcorper felis ut lobortis. Sed lacinia malesuada nunc ut interdum. Aliquam lobortis lacus sed augue dapibus scelerisque. Fusce vitae lobortis odio. Nam non tempus lacus, a varius nunc. Suspendisse accumsan ligula in erat feugiat, eget dignissim nisi tincidunt. Aliquam erat volutpat. Donec egestas, leo imperdiet commodo aliquam, est nibh facilisis felis, pretium ultrices ex ipsum a leo.

Duis eu justo nunc. Maecenas aliquam justo urna, nec ultricies nisi egestas id. Mauris quis risus tincidunt, varius massa nec, interdum nisi. Curabitur vel leo massa. Etiam suscipit elit ligula, a pretium dui facilisis a. In facilisis vestibulum tortor sit amet dignissim. Nam aliquet magna non mollis laoreet. Vestibulum venenatis consequat orci, nec pulvinar nisi ornare quis. Cras sodales lobortis lacinia. Curabitur tempor, dolor id tincidunt euismod, lorem ipsum consectetur purus, ac consequat nisl ante vitae nibh. Donec mollis feugiat aliquam. Sed convallis felis non sem cursus laoreet ac id mi.

Phasellus enim ex, tincidunt ut diam congue, ultrices vehicula velit. Aliquam vel elit venenatis, malesuada sem a, congue nulla. Donec eu massa nec nisl semper viverra sed a quam. Duis ac lacinia ligula. Nullam placerat ante eu hendrerit posuere. Curabitur condimentum, justo at aliquam fermentum, nunc risus efficitur erat, nec porttitor mi tortor et tellus. Fusce porta pharetra vulputate. Cras nec tempor massa. Ut pellentesque efficitur condimentum. Ut blandit non nibh et pellentesque. Vivamus ut auctor ante, nec ornare dui.
        `;
    }
    get footer() {
        return html`
            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Close</button>
        `;
    }

    render() {
        const $component = this.$component;
        return html`
        <div class="modal" tabindex="-1" role="dialog" aria-hidden="true" data-window-modal data-backdrop="${$component.__static? 'static' : ''}">
            <div class="modal-dialog ${$component.__size} ${$component.__scroll} ${$component.__centered}" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        ${this.header}
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">${this.body}</div>
                    ${!!this.footer? html`<div class="modal-footer">${this.footer}</div>` : ''}
                </div>
            </div>
        </div>`;
    }
}