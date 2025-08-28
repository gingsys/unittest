export class DOM {
    static replaceContent(container, template, eventContext) {
        container.innerHTML = '';
        if(typeof template === 'string') {
            container.innerHTML = template;
        } else if(template instanceof HTMLElement) {
            container.appendChild(template);
        // } else if(template instanceof TemplateResult) {
        //     if(eventContext == undefined) {
        //         render(template, container.innerHTML);
        //     } else {
        //         render(template, container.innerHTML, { eventContext });
        //     }
        } else {
            throw new Error(`> DOM.replaceElement: [Error] >> template ins't a string, HTMLElement or TemplateResult.`);
        }
        return container.firstChild;
    }
    static replaceContentTag(container, tag, attrs = {}) {
        if(typeof tag === 'string') {
            container.innerHTML = '';
            let element = document.createElement(tag);
            for(let key in attrs) {
                element.setAttribute(key, attrs[key]);
            }
            return DOM.replaceContent(container, element);
        } else {
            throw new Error(`> DOM.replaceElementTag: [Error] >> tag ins't a string.`);
        }
    }
}