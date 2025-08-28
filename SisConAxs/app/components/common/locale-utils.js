export const LocaleUtils = new class {
    constructor() {
        this.__setLocale('es-PE');
    }
    __setLocale(locale) {
        this.__locale = locale;
        const parts = new Intl.NumberFormat(locale).formatToParts(12345.6);
        const numerals = [...new Intl.NumberFormat(locale, {useGrouping: false}).format(9876543210)].reverse();
        const index = new Map(numerals.map((d, i) => [d, i]));
        this._group = new RegExp(`[${parts.find(d => d.type === "group").value}]`, "g");
        this._decimal = new RegExp(`[${parts.find(d => d.type === "decimal").value}]`);
        this._numeral = new RegExp(`[${numerals.join("")}]`, "g");
        this._index = d => index.get(d);
    }

    get locale() {
        return this.__locale;
    }
    set locale(value) {
        this.__setLocale(value);
    }
    
    formatNumber(number, digits = 2) {
        return new Intl.NumberFormat(this.__locale, {
            minimumFractionDigits: digits,
            maximumFractionDigits: digits
        }).format(number);
    }
    parseNumber(string) {
        return (string = string.trim()
        .replace(this._group, "")
        .replace(this._decimal, ".")
        .replace(this._numeral, this._index)) ? +string : NaN;
    }

    formatCurrency(number, currency = 'PEN', digits = 2) {
        return new Intl.NumberFormat(this.__locale, {
            style: 'currency',
            currency,
            minimumFractionDigits: digits,
            maximumFractionDigits: digits
        }).format(number);
    }
    parseCurrency(string, currency = 'PEN') {
        return (string = string.trim()
        .replace(this._group, "")
        .replace(this._decimal, ".")
        .replace(this._numeral, this._index)) ? +string : NaN;
    }
}