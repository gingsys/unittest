import { LocaleUtils } from "../../common/locale-utils.js";

export const InputNumber = new class {
    constructor() {

    }
    keypress(e) {
        const keyCode = e.which ? e.which : e.keyCode;
        const specialKeys = ['.',','].map(c => c.charCodeAt(0));
        const ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
        if(!ret) e.preventDefault();
        return ret;
    }
    change(e, digits = 2) {
        const number = LocaleUtils.parseNumber(e.target.value);
        digits = e.target.dataset.digits || digits;
        e.target.value = isNaN(number)? 0 : LocaleUtils.formatNumber(number, digits);
    }
}