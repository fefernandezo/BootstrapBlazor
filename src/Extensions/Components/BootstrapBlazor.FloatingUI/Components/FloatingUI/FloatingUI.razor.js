import { addLink, getDescribedElement } from '../../../BootstrapBlazor/modules/utility.js'
import { createPopper, computePosition as computePosition$1 } from '../../../BootstrapBlazor/modules/floating-ui.js?v=$version'
import Data from "../../../BootstrapBlazor/modules/data.js?v=$version"
import EventHandler from "../../../BootstrapBlazor/modules/event-handler.js?v=$version"

export async function init(id, options) {
    await addLink('./_content/BootstrapBlazor.FloatingUI/css/bb-floating-ui.css')

    const el = document.getElementById(id)
    const floating = getDescribedElement(el)
    if (el === null) {
        return
    }
    if (floating === null) {
        return
    }

    document.body.appendChild(floating)
    options = getOptions(options)

    const fl = { el, floating, options }
    Data.set(id, fl)

    if (options.trigger === 'hover') {
        EventHandler.on(el, 'click', e => toggle(fl))
        //EventHandler.on(el, 'mouseleave', e => hide(fl))
    }
}

export function dispose(id) {
    console.log('dispose')
}

const getOptions = options => {
    return {
        ...{
            trigger: 'hover',
            placement: 'bottom-start'
        },
        ...options
    }
}

const toggle = fl => {
    if (fl.popper) {
        hide(fl)
    }
    else {
        show(fl)
    }
}

const show = fl => {
    const reference = fl.el
    const floating = fl.floating

    if (fl.popper) {
        fl.popper()
    }

    fl.popper = createPopper(reference, floating, () => computePosition(reference, floating, fl.options))
}

const computePosition = async (reference, floating, options) => {
    const pos = await computePosition$1(reference, floating, {
        placement: options.placement
    })

    Object.assign(floating.style, {
        left: `${pos.x}px`,
        top: `${pos.y}px`,
    })
    floating.classList.remove('d-none')
}

const hide = fl => {
    const floating = fl.floating
    floating.classList.add('d-none')

    if (fl.popper) {
        fl.popper()
        delete fl.popper
    }
}
